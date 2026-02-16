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
    private readonly string[] allowedDomains;
    protected override string Name => "UrlQueuer";

    public UrlQueuer()
    {
        urls = new UrlsDb(Logger);

        var minutes = Environment.GetEnvironmentVariable("REVISIT_MINUTES");
        if (string.IsNullOrEmpty(minutes)) throw new Exception("Missing envvar REVISIT_MINUTES");
        revisitDelay = TimeSpan.FromMinutes(Convert.ToInt32(minutes));
        if (revisitDelay < TimeSpan.FromMinutes(1)) revisitDelay = TimeSpan.FromMinutes(1);

        var d = Environment.GetEnvironmentVariable("ALLOWED_DOMAINS");
        if (string.IsNullOrEmpty(d)) throw new Exception("Missing envvar: ALLOWED_DOMAINS");
        allowedDomains = d.Split(';', StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);
        if (allowedDomains.Length == 0) throw new Exception("No ALLOWED_DOMAINS");
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
            if (IsAllowed(hit))
            {
                await Hub.UrlToVisit.Send(new MessageQueue.MsgUrlToVisit(hit));
            }
        }

        await urls.UpdateLastQueued(hits);

        Logger.Info($"Queued {hits.Length} URLs");

        await Task.Delay(revisitDelay / 3);
    }

    private bool IsAllowed(string hit)
    {
        try
        {
            var u = new Uri(hit);
            var host = u.Host; // for example: skills.github.com
            foreach (var a in allowedDomains)
            {
                if (host.EndsWith(a))
                {
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Logger.Error($"Unable to parse URL '{hit}'", ex);
            return false;
        }
    }
}
