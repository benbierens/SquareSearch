using SquareSearchMain;

Console.WriteLine("Hello, World!");
Console.WriteLine("1. build index");
Console.WriteLine("2. perform search");

var urlQueue = new CrawlQueue();
urlQueue.Start();
urlQueue.Push("http://bencc.nl");

var searchIndex = new SearchIndex();
searchIndex.Start();

var indexer = new PageIndexer(searchIndex);

var indexingQueue = new ContentQueue(indexer);
indexingQueue.Start();

var mainHandlers = new MuxingContentHandler(
    new LogHandler(),
    new UrlFinder(urlQueue),
    indexingQueue
);

var robots = new Robots();
robots.Start();
var ingestionQueue = new ContentQueue(mainHandlers);
var visitor = new Visitor(urlQueue, robots, ingestionQueue);

var option = Console.ReadLine();
var loop = true;
while (loop)
{
    if (option == "1")
    {
        ingestionQueue.Start();
        visitor.Start();

        Thread.Sleep(10000);

        visitor.Stop();
        ingestionQueue.Stop();
        break;
    }
    else if (option == "2")
    {
        Console.WriteLine("Enter query:");
        var query = Console.ReadLine()!.ToLowerInvariant();
        Console.WriteLine($"Query: '{query}'");
        if (query.Length < 3) break;

        var hits = searchIndex.Search(query);
        Console.WriteLine($"{hits.Length} hits!");
        foreach (var h in hits)
        {
            Console.WriteLine("   " + h);
        }
    }
}

robots.Stop();
indexingQueue.Stop();
searchIndex.Stop();
urlQueue.Stop();

Console.WriteLine("Done!");

