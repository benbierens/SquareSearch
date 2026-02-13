using Common;
using MessageQueue;
using System.Text.RegularExpressions;

var urlExtract = new UrlExtractApp();
await urlExtract.Run();

while (true)
{
    Thread.Sleep(10000);
}

public class UrlExtractApp : MqHubApp, IMqMessageHandler<MsgRawPage>
{
    protected override string Name => "UrlExtract";

    public async Task Run()
    {
        await Hub.PageToUrls.Receive(this);
    }

    public async Task OnMessage(MsgRawPage message, IMsMessageAcknowledger ack)
    {
        var matches = Regex.Matches(message.Content, @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");

        var hits = 0;
        foreach (var m in matches)
        {
            if (m != null)
            {
                var s = m.ToString();
                if (!string.IsNullOrEmpty(s))
                {
                    if (await Consider(s)) hits++;
                }
            }
        }

        Logger.Info($"Page '{message.Url}' yielded {hits} urls.");
    }

    private async Task<bool> Consider(string s)
    {
        if (s.EndsWith(".js")) return false;
        if (s.EndsWith(".svg")) return false;
        if (s.EndsWith(".png")) return false;
        if (s.EndsWith(".jpg")) return false;
        if (s.EndsWith(".css")) return false;

        Logger.Trace($"Discovered: '{s}'");
        // how to stop immediate re-visits?
        await Hub.UrlToVisit.Send(new MsgUrlToVisit(s));
        return true;
    }
}
