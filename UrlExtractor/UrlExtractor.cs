using Common;
using DbUrls;
using MessageQueue;
using System.Text.RegularExpressions;

var urlExtract = new UrlExtractApp();
await urlExtract.Init();
await urlExtract.Run();

while (true)
{
    Thread.Sleep(10000);
}

public class UrlExtractApp : MqHubApp, IMqMessageHandler<MsgRawPage>
{
    private readonly UrlsDb urls;
    protected override string Name => "UrlExtract";

    public UrlExtractApp()
    {
        urls = new UrlsDb(Logger);
    }

    public async Task Init()
    {
        await urls.Initialize();
    }

    public async Task Run()
    {
        await Hub.PageToUrls.Receive(this);
    }

    public async Task OnMessage(MsgRawPage message, IMsMessageAcknowledger ack)
    {
        await ProcessMessage(message);
        await ack.AckMessage();
    }

    private async Task ProcessMessage(MsgRawPage message)
    {
        if (string.IsNullOrEmpty(message.Content))
        {
            // Visitor failed to reach this url. Do nothing.
            return;
        }

        var matches = Regex.Matches(message.Content, @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");

        var hits = new List<string>();
        foreach (var m in matches)
        {
            if (m != null)
            {
                var s = m.ToString();
                if (!string.IsNullOrEmpty(s))
                {
                    if (Consider(hits, s))
                    {
                        hits.Add(s);
                    }
                }
            }
        }

        await urls.LearnNewUrls(hits);

        Logger.Info($"Page '{message.Url}' yielded {hits.Count} urls.");
    }

    private bool Consider(List<string> hits, string s)
    {
        if (s.EndsWith(".js")) return false;
        if (s.EndsWith(".svg")) return false;
        if (s.EndsWith(".png")) return false;
        if (s.EndsWith(".jpg")) return false;
        if (s.EndsWith(".css")) return false;
        if (hits.Contains(s)) return false;

        Logger.Trace($"Discovered: '{s}'");
        return true;
    }
}
