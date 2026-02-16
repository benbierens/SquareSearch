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

        var rawPage = await TryVisit(message.Url);

        await Hub.PageToIndex.Send(rawPage);
        await Hub.PageToUrls.Send(rawPage);
        await ack.AckMessage();

        var span = DateTime.UtcNow - start;
        Logger.Info($"Visited '{message.Url}' in {span.TotalSeconds} seconds.");
    }

    private async Task<MsgRawPage> TryVisit(string url, int retry = 0)
    {
        if (retry > 2) return new MsgRawPage(url, string.Empty);

        var page = await InternalTryVisit(url);
        if (page != null) return page;

        await Task.Delay(TimeSpan.FromMinutes(1));
        return await TryVisit(url, retry + 1);
    }

    private async Task<MsgRawPage?> InternalTryVisit(string url)
    {
        try
        {
            var page = await Web.Get(url);
            return new MsgRawPage(url, page);
        }
        catch
        {
            return null;
        }
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
