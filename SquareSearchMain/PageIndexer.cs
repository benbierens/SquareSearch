namespace SquareSearchMain
{
    public class PageIndexer : IContentHandler
    {
        private readonly SearchIndex searchIndex;

        public PageIndexer(SearchIndex searchIndex)
        {
            this.searchIndex = searchIndex;
        }

        public void OnContent(RawPage rawPage)
        {
            var tokens = Tokenize.This(rawPage.Content);
                
            var counts = new Dictionary<string, int>();
            foreach (var token in tokens)
            {
                if (token.Length > 2) AddOrAdd(counts, token);
            }
            searchIndex.Save(rawPage.Url, counts);
        }

        private void AddOrAdd(Dictionary<string, int> counts, string token)
        {
            if (counts.ContainsKey(token)) 
            {
                counts[token]++;
            }
            else
            {
                counts.Add(token, 1);
            }
        }
    }
}
