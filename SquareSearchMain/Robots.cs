namespace SquareSearchMain
{
    public class Robots : JsonFile<RobotsModel>
    {
        protected override string Filename => "robots.json";

        public bool IsAllowed(string url)
        {
            var robotsUrl = ToRobotsUrl(url);
            if (string.IsNullOrEmpty(robotsUrl)) return false;

            var entry = GetEntry(robotsUrl);
            // todo is allowed?!
            return true; 
        }

        private RobotsEntry GetEntry(string robotsUrl)
        {
            var e = Model.Entries.FirstOrDefault(e => e.SourceUrl == robotsUrl);
            if (e != null) return e;

            var content = Web.Get(robotsUrl);
            var newEntry = new RobotsEntry
            {
                SourceUrl = robotsUrl,
                // Disallowed  todo
            };
            Model.Entries.Add(newEntry);
            return newEntry;
        }

        private string ToRobotsUrl(string url)
        {
            if (!url.EndsWith("/")) url += "/";

            if (url.StartsWith("http://"))
            {
                var idx = 7 + url.Substring(7).IndexOf('/');
                return url.Substring(0, idx) + "/robots.txt";
            }
            if (url.StartsWith("https://"))
            {
                var idx = 8 + url.Substring(8).IndexOf('/');
                return url.Substring(0, idx) + "/robots.txt";
            }
            return string.Empty;
        }
    }

    public class RobotsModel
    {
        public List<RobotsEntry> Entries { get; set; } = new List<RobotsEntry>();
    }

    public class RobotsEntry
    {
        public string SourceUrl { get; set; } = string.Empty;
        public string[] Disallowed { get; set; } = Array.Empty<string>();
    }
}
