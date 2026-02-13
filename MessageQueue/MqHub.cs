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
            PageToIndex = new Queue<MsgRawPage>(factory, "pagetoindex");
            PageToUrls = new Queue<MsgRawPage>(factory, "pagetourls");
        }

        public IQueue<MsgUrlToVisit> UrlToVisit { get; }
        public IQueue<MsgRawPage> PageToIndex { get; }
        public IQueue<MsgRawPage> PageToUrls { get; }
    }
}
