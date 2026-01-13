using System.Collections.Concurrent;
using System.Text.Json;

namespace SquareSearchMain
{
    public class CrawlQueue : IStartStop
    {
        private const string filename = "crawlqueue.json";
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

        public void Start()
        {
            if (File.Exists(filename))
            {
                try
                {
                    var entries = JsonSerializer.Deserialize<string[]>(File.ReadAllText(filename));
                    if (entries != null)
                    {
                        foreach (var e in entries) Push(e);
                    }
                }
                catch { }
            }
        }

        public void Stop()
        {
            File.WriteAllText(filename, JsonSerializer.Serialize(queue.ToArray()));
        }
    }
}
