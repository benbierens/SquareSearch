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

    }
}
