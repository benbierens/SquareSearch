using SquareSearchMain;

Console.WriteLine("Hello, World!");

var queue = new CrawlQueue();
queue.Push("http://bencc.nl");

var logHandler = new LogHandler();
var contentQueue = new ContentQueue(logHandler);

var visitor = new Visitor(queue, contentQueue);

contentQueue.Start();
visitor.Start();

Thread.Sleep(10000);

visitor.Stop();
contentQueue.Stop();

Console.WriteLine("Done!");

