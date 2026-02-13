using Common;
using MessageQueue;

var visitor = new VisitorApp();
await visitor.Run();

while (true)
{
    Thread.Sleep(10000);
}

public class VisitorApp : MqHubApp, IMqMessageHandler<MsgUrlToVisit>
{
    protected override string Name => "Visitor";

    private readonly Random random = new Random();
    private int checkDelay = 100;

    public async Task Run()
    {
        await Hub.UrlToVisit.Send(new MsgUrlToVisit("http://bencc.nl"));

        await Hub.UrlToVisit.Receive(this);
    }

    public async Task OnMessage(MsgUrlToVisit message, IMsMessageAcknowledger ack)
    {
        await CheckDelay();

        var start = DateTime.UtcNow;
        Logger.Trace($"Visiting '{message.Url}'...");

        var page = await Web.Get(message.Url);
        var rawPage = new MsgRawPage(message.Url, page);

        await Hub.PageToIndex.Send(rawPage);
        await Hub.PageToUrls.Send(rawPage);
        await ack.AckMessage();

        var span = DateTime.UtcNow - start;
        Logger.Info($"Visited '{message.Url}' in {span.TotalSeconds} seconds.");
    }

    private async Task CheckDelay()
    {
        checkDelay--;
        if (checkDelay > 0) return;

        var queueLength = await Hub.PageToIndex.GetQueueLength();
        if (queueLength > 1000)
        {
            Logger.Info("Applying delay...");
            await Task.Delay(TimeSpan.FromMinutes(10 + random.Next(0, 10)));
        }
        else
        {
            checkDelay = 100 + random.Next(0, 100);
        }
    }
}
