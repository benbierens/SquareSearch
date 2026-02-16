using Logging;

namespace DbUrls
{
    public class IndexDb : DbCommon<IndexContext>
    {
        public IndexDb(ILogger logger)
            : base(logger, "dbindex")
        {
        }
    }

    public class IndexContext : BaseContext
    {
        // useful_words uniqueness with weight
        // count: link word to scan
        // scan: utc and url
        // url uniqueness
    }
}
