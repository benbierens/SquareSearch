namespace SquareSearchMain
{
    public class MuxingContentHandler : IContentHandler
    {
        private readonly IContentHandler[] handlers;

        public MuxingContentHandler(params IContentHandler[] handlers)
        {
            this.handlers = handlers;
        }

        public void OnContent(RawPage rawPage)
        {
            foreach (var h in handlers) h.OnContent(rawPage);
        }
    }
}
