using Logging;

namespace MessageQueue
{
    public class MqHub
    {
        private readonly MqFactory factory;

        public MqHub(ILogger logger, string hostname)
        {
            if (string.IsNullOrEmpty(hostname)) throw new ArgumentNullException(nameof(hostname));

            factory = new MqFactory(logger, hostname);

            UrlToVisit = new Queue<MsgUrlToVisit>(factory, "urltovisit");
            RawPage = new Queue<MsgRawPage>(factory, "rawpage");
        }

        public IQueue<MsgUrlToVisit> UrlToVisit { get; }
        public IQueue<MsgRawPage> RawPage { get; }
    }
}
