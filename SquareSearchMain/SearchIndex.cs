using System.Text.Json;

namespace SquareSearchMain
{
    public class SearchIndex : IStartStop
    {
        private readonly List<IndexEntry> entries = new List<IndexEntry>();
        private const string filename = "index.json";

        public void Save(string url, Dictionary<string, int> counts)
        {
            entries.Add(new IndexEntry
            {
                Url = url,
                Counts = counts.Select(pair => new EntryCount
                {
                    Token = pair.Key,
                    Count = pair.Value,
                }).ToArray()
            });
        }

        public string[] Search(string query)
        {
            var tokens = query.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var bestCount = 0;
            var bestEntry = new IndexEntry();

            foreach (var entry in entries)
            {
                var score = GetScore(entry, tokens);
                Console.WriteLine($" '{entry.Url}' = {score}");
                if (score > bestCount)
                {
                    bestCount = score;
                    bestEntry = entry;
                }
            }

            return [bestEntry.Url];
        }

        private int GetScore(IndexEntry entry, string[] tokens)
        {
            var score = 0;
            foreach (var t in tokens)
            {
                foreach (var c in entry.Counts)
                {
                    if (c.Token == t) score += c.Count;
                }
            }
            return score;
        }

        public void Start()
        {
            if (File.Exists(filename))
            {
                try
                {
                    var model = JsonSerializer.Deserialize<IndexModel>(File.ReadAllText(filename));
                    if (model != null)
                    {
                        foreach (var e in model.Entries) entries.Add(e);
                    }
                }
                catch { }
            }
        }

        public void Stop()
        {
            File.WriteAllText(filename, JsonSerializer.Serialize(new IndexModel
            {
                Entries = entries.ToArray()
            }));
        }
    }

    public class IndexModel
    {
        public IndexEntry[] Entries { get; set; } = Array.Empty<IndexEntry>();
    }

    public class IndexEntry
    {
        public string Url { get; set; } = string.Empty;
        public EntryCount[] Counts { get; set; } = Array.Empty<EntryCount>();
    }

    public class EntryCount
    {
        public string Token { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
