using Common;
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

    public IndexerApp()
    {
    }

    public async Task Run()
    {
        await Hub.PageToIndex.Receive(this);
    }

    public async Task OnMessage(MsgRawPage message, IMsMessageAcknowledger ack)
    {
        Logger.Trace($"Indexing '{message.Url}'...");


    }
}
