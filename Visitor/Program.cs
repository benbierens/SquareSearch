using Common;
using Logging;
using MessageQueue;

var visitor = new VisitorApp();
await visitor.Run();

while (true)
{
    Thread.Sleep(10000);
}

public class VisitorApp : IMqMessageHandler<MsgUrlToVisit>
{
    private readonly ILogger logger;
    private readonly MqHub hub;
    private readonly Random random = new Random();
    private int checkDelay = 100;

    public VisitorApp()
    {
        var sink = new ConsoleSink();
        logger = new PrefixingLoggger(
            new SinkingLogger(sink), "Visitor");

        var queueHost = Environment.GetEnvironmentVariable("MQHOST");
        logger.Info($"MQHOST = {queueHost}");
        if (string.IsNullOrEmpty(queueHost)) throw new ArgumentNullException();

        hub = new MqHub(logger, queueHost);
    }

    public async Task Run()
    {
        await hub.UrlToVisit.Receive(this);
    }

    public async Task OnMessage(MsgUrlToVisit message, IMsMessageAcknowledger ack)
    {
        await CheckDelay();

        var page = Web.Get(message.Url);
        var rawPage = new MsgRawPage(message.Url, page);

        await hub.RawPage.Send(rawPage);
    }

    private async Task CheckDelay()
    {
        checkDelay--;
        if (checkDelay > 0) return;

        var queueLength = await hub.RawPage.GetQueueLength();
        if (queueLength > 1000)
        {
            logger.Info("Applying delay...");
            await Task.Delay(TimeSpan.FromMinutes(10 + random.Next(0, 10)));
        }
        else
        {
            checkDelay = 100 + random.Next(0, 100);
        }
    }
}
