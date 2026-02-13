namespace Logging
{
    public interface ILogger
    {
        void Error(string msg, Exception ex);
        void Error(string msg);
        void Info(string msg);
        void Trace(string msg);
        ILogger Prefix(string prefix);
    }

    public interface ILogSink
    {
        void Write(string msg);
    }

    public class ConsoleSink : ILogSink
    {
        public void Write(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    public class PrefixingLoggger : ILogger
    {
        private readonly ILogger backingLogger;
        private readonly string p;

        public PrefixingLoggger(ILogger backingLogger, string prefix)
        {
            this.backingLogger = backingLogger;
            p = $"({prefix})";
        }

        public void Error(string msg, Exception ex)
        {
            backingLogger.Error(p + msg, ex);
        }

        public void Error(string msg)
        {
            backingLogger.Error(p + msg);
        }

        public void Info(string msg)
        {
            backingLogger.Info(p + msg);
        }

        public ILogger Prefix(string prefix)
        {
            return new PrefixingLoggger(this, prefix);
        }

        public void Trace(string msg)
        {
            backingLogger.Trace(p + msg);
        }
    }

    public class SinkingLogger : ILogger
    {
        private readonly ILogSink sink;

        public SinkingLogger(ILogSink sink)
        {
            this.sink = sink;
        }

        public void Error(string msg, Exception ex)
        {
            sink.Write($"ERROR {msg}={ex}");
        }

        public void Error(string msg)
        {
            sink.Write($"ERROR {msg}");
        }

        public void Info(string msg)
        {
            sink.Write($"INFO  {msg}");
        }

        public ILogger Prefix(string prefix)
        {
            return new PrefixingLoggger(this, prefix);
        }

        public void Trace(string msg)
        {
            sink.Write($"TRACE {msg}");
        }
    }
}
