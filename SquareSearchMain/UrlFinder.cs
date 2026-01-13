using System.Text.RegularExpressions;

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
            var matches = Regex.Matches(rawPage.Content, @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");

            foreach (var m in matches)
            {
                if (m != null)
                {
                    var s = m.ToString();
                    if (!string.IsNullOrEmpty(s))
                    {
                        Consider(s);
                    }
                }
            }
        }

        private void Consider(string s)
        {
            if (s.EndsWith(".js")) return;
            if (s.EndsWith(".svg")) return;
            if (s.EndsWith(".png")) return;
            if (s.EndsWith(".jpg")) return;
            if (s.EndsWith(".css")) return;

            queue.Push(s);
        }
    }
}
