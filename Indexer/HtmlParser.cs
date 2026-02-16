using HtmlAgilityPack;
using Logging;

namespace Indexer
{
    public class HtmlParser
    {
        private readonly ILogger logger;
        private readonly string[] disgardHtmlNodeNames = [
            "script",
            "style",
            "meta",
            "template"
        ];

        public HtmlParser(ILogger logger)
        {
            this.logger = logger;
        }

        public List<string> Parse(string content)
        {
            var html = new HtmlDocument();
            html.LoadHtml(content);

            var output = new List<string>();
            Traverse(output, html.DocumentNode);

            logger.Trace(string.Join(Environment.NewLine, output));
            return output;
        }

        private void Traverse(List<string> output, HtmlNode node)
        {
            if (IsWanted(node) && !HasWantedChildren(node))
            {
                var processed = node.InnerText;
                output.Add(processed.Trim());
                //output.Add($"{Indented(indent)}[name:{node.Name}] = '{processed}'");
            }

            foreach (var c in node.ChildNodes)
            {
                Traverse(output, c);
            }
        }

        private bool HasWantedChildren(HtmlNode node)
        {
            return node.ChildNodes.Any(IsWantedOrHasWantedChildren);
        }

        private bool IsWantedOrHasWantedChildren(HtmlNode node)
        {
            if (IsWanted(node)) return true;
            return node.ChildNodes.Any(IsWantedOrHasWantedChildren);
        }

        private bool IsWanted(HtmlNode node)
        {
            var name = node.Name;
            if (name.StartsWith("#")) return false;
            if (disgardHtmlNodeNames.Contains(name)) return false;

            return true;
        }
    }
}
