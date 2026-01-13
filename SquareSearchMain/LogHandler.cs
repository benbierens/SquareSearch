using SquareSearchMain;

public class LogHandler : IContentHandler
{
    public void OnContent(RawPage rawPage)
    {
        Console.WriteLine($"   '{rawPage.Url}'");
    }
}
