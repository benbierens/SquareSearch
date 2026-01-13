namespace SquareSearchMain
{
    public interface IContentHandler
    {
        void OnContent(RawPage rawPage);
    }

    public class Visitor : ThreadedComponent
    {
        private readonly CrawlQueue queue;
        private readonly IContentHandler handler;
        private readonly HttpClient client = new HttpClient();

        public Visitor(CrawlQueue queue, IContentHandler handler)
        {
            this.queue = queue;
            this.handler = handler;
        }

        protected override void DoWork()
        {
            var url = queue.Pop(Ct);

            var task1 = client.GetAsync(url);
            task1.Wait();
            var a = task1.Result;
            var task2 = a.Content.ReadAsStringAsync();
            task2.Wait();
            var content = task2.Result;

            handler.OnContent(new RawPage(url, content));
        }
    }
}
