using System.Collections.Concurrent;

namespace SquareSearchMain
{
    public class ContentQueue : ThreadedComponent, IContentHandler
    {
        private readonly BlockingCollection<RawPage> queue = new BlockingCollection<RawPage>();
        private readonly IContentHandler backingHandler;

        public ContentQueue(IContentHandler backingHandler)
        {
            this.backingHandler = backingHandler;
        }

        public void OnContent(RawPage rawPage)
        {
            queue.Add(rawPage);
        }

        protected override void DoWork()
        {
            var page = queue.Take(Ct);
            backingHandler.OnContent(page);
        }
    }
}
