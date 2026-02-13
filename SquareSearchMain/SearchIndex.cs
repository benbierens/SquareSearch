using Common;

namespace SquareSearchMain
{
    public class SearchIndex : JsonFile<IndexModel>
    {
        protected override string Filename => "index.json";

        public void Save(string url, Dictionary<string, int> counts)
        {
            Model.Entries.Add(new IndexEntry
            {
                Url = url,
                Counts = counts.Select(pair => new EntryCount
                {
                    Token = pair.Key,
                    Count = pair.Value,
                }).ToList()
            });
        }

        public string[] Search(string query)
        {
            var tokens = Tokenize.This(query);

            var bestCount = 0;
            var bestEntry = new IndexEntry();

            foreach (var entry in Model.Entries)
            {
                var score = GetScore(entry, tokens);
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
                    if (c.Token == t)
                    {
                        Console.WriteLine($"token '{t}' found in {entry.Url} for a score of {c.Count}");
                        score += c.Count;
                    }
                }
            }
            return score;
        }
    }

    public class IndexModel
    {
        public List<IndexEntry> Entries { get; set; } = new List<IndexEntry>();
    }

    public class IndexEntry
    {
        public string Url { get; set; } = string.Empty;
        public List<EntryCount> Counts { get; set; } = new List<EntryCount>();
    }

    public class EntryCount
    {
        public string Token { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
