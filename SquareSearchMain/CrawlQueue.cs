using System.Collections.Concurrent;

namespace SquareSearchMain
{
    public class CrawlQueue
    {
        private readonly BlockingCollection<string> queue = new BlockingCollection<string>();

        public void Push(string url)
        {
            if (queue.Contains(url)) return;
            queue.Add(url);
        }

        public string Pop(CancellationToken ct)
        {
            return queue.Take(ct);
        }
    }
}
