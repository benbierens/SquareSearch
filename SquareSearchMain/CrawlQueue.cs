using System.Collections.Concurrent;

namespace SquareSearchMain
{
    public class CrawlQueue : JsonFile<QueueModel>
    {
        protected override string Filename => "crawlqueue.json";
        private readonly BlockingCollection<string> queue = new BlockingCollection<string>();

        public void Push(string url)
        {
            if (queue.Contains(url)) return;
            queue.Add(url);
            queue.Shuffle();
        }

        public string Pop(CancellationToken ct)
        {
            return queue.Take(ct);
        }

        protected override void OnStartFinished()
        {
            foreach (var e in Model.Queue) Push(e);
        }

        protected override void OnStopStarted()
        {
            Model.Queue = queue.ToList();
        }
    }

    public class QueueModel
    {
        public List<string> Queue = new List<string>();
    }
}
