namespace SquareSearchMain
{
    public class UrlFinder : IContentHandler
    {
        private readonly CrawlQueue queue;

        public UrlFinder(CrawlQueue queue)
        {
            this.queue = queue;
        }

        public void OnContent(RawPage rawPage)
        {

        }
    }
}
