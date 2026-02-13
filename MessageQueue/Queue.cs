namespace MessageQueue
{
    public interface IQueue<T> where T : class
    {
        Task Send(T msg);
        Task<uint> GetQueueLength();
        Task Receive(IMqMessageHandler<T> handler);
    }

    internal class Queue<T> : IQueue<T> where T : class
    {
        private readonly MqFactory factory;
        private readonly string topic;

        public Queue(MqFactory factory, string topic)
        {
            this.factory = factory;
            this.topic = topic;
        }

        private IMqSender? sender = null;
        public async Task Send(T msg)
        {
            if (sender == null) sender = await factory.OpenSender(topic);
            await sender.Send(msg);
        }

        public async Task Receive(IMqMessageHandler<T> handler)
        {
            await factory.OpenReceiver(topic, handler);
        }

        public async Task<uint> GetQueueLength()
        {
            if (sender == null) sender = await factory.OpenSender(topic);
            return await sender.GetQueueLength();
        }
    }

    public class MsgUrlToVisit
    {
        public MsgUrlToVisit(string url)
        {
            Url = url;
        }

        public string Url { get; }
    }

    public class MsgRawPage
    {
        public MsgRawPage(string url, string content)
        {
            Url = url;
            Content = content;
        }

        public string Url { get; }
        public string Content { get; }
    }
}
