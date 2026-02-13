namespace Common
{
    public class Tokenize
    {
        public static string[] This(string input)
        {
            return input.ToLowerInvariant()
                .Replace(". ", " ")
                .Replace(", ", " ")
                .Replace("? ", " ")
                .Replace("! ", " ")
                .Replace("'", " ")
                .Replace("\"", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.Trim())
                .ToArray();
        }
    }
}
