using Common;

namespace SquareSearchMain
{
    public interface IContentHandler
    {
        void OnContent(RawPage rawPage);
    }

    public class Visitor : ThreadedComponent
    {
        private readonly CrawlQueue queue;
        private readonly Robots robots;
        private readonly IContentHandler handler;

        public Visitor(CrawlQueue queue, Robots robots, IContentHandler handler)
        {
            this.queue = queue;
            this.robots = robots;
            this.handler = handler;
        }

        protected override void DoWork()
        {
            var url = queue.Pop(Ct);

            if (!robots.IsAllowed(url)) return;

            var content = Web.Get(url);

            handler.OnContent(new RawPage(url, content));
        }
    }
}
