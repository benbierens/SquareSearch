using Common;
using DbUrls;

var urlQueuer = new UrlQueuer();
await urlQueuer.Init();

while (true)
{
    await urlQueuer.Run();
}

public class UrlQueuer : MqHubApp
{
    private readonly UrlsDb urls;
    private readonly TimeSpan revisitDelay;
    protected override string Name => "UrlQueuer";

    public UrlQueuer()
    {
        urls = new UrlsDb(Logger);

        var minutes = Environment.GetEnvironmentVariable("REVISIT_MINUTES");
        if (string.IsNullOrEmpty(minutes)) throw new Exception("Missing envvar REVISIT_MINUTES");
        revisitDelay = TimeSpan.FromMinutes(Convert.ToInt32(minutes));
        if (revisitDelay < TimeSpan.FromMinutes(1)) revisitDelay = TimeSpan.FromMinutes(1);
    }

    public async Task Init()
    {
        await urls.Initialize();
    }

    public async Task Run()
    {
        var hits = await urls.QueryUrlsBefore(DateTime.UtcNow - revisitDelay);

        foreach (var hit in hits)
        {
            await Hub.UrlToVisit.Send(new MessageQueue.MsgUrlToVisit(hit));
        }
        Logger.Info($"Queued {hits.Length} URLs");

        await Task.Delay(revisitDelay / 3);
    }
}
