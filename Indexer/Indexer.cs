using Common;
using Indexer;
using MessageQueue;

var indexer = new IndexerApp();
await indexer.Run();

while (true)
{
    Thread.Sleep(10000);
}

public class IndexerApp : MqHubApp, IMqMessageHandler<MsgRawPage>
{
    protected override string Name => "Indexer";
    private readonly HtmlParser parser;

    public IndexerApp()
    {
        parser = new HtmlParser(Logger);
    }

    public async Task Run()
    {
        await Hub.PageToIndex.Receive(this);
    }

    public async Task OnMessage(MsgRawPage message, IMsMessageAcknowledger ack)
    {
        Logger.Trace($"Indexing '{message.Url}'...");

        var segments = parser.Parse(message.Content);
        // todo: tokenize, and store!

        Logger.Info("done!");
        await ack.AckMessage();
    }
}
