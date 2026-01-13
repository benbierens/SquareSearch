namespace SquareSearchMain
{
    public class RawPage
    {
        public RawPage(string url, string content)
        {
            Url = url;
            Content = content;
        }

        public string Url { get; }
        public string Content { get; }
    }
}
