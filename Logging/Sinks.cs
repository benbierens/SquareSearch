namespace Logging
{
    public interface ILogSink
    {
        void Write(string msg);
    }

    public class MuxingSink : ILogSink
    {
        private readonly ILogSink[] sinks;

        public MuxingSink(params ILogSink[] sinks)
        {
            this.sinks = sinks;
        }

        public void Write(string msg)
        {
            foreach (var s in sinks) s.Write(msg);
        }
    }

    public class ConsoleSink : ILogSink
    {
        public void Write(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    public class FileSink : ILogSink
    {
        private readonly string filepath;

        public FileSink(string filepath)
        {
            this.filepath = filepath;
            var dir = Path.GetDirectoryName(filepath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            else throw new Exception("Invalid filepath: " + filepath);
        }

        public void Write(string msg)
        {
            try
            {
                File.AppendAllLines(filepath, [msg]);
            }
            catch
            {
                Console.WriteLine("ERROR: Log failed to write to file sink.");
            }
        }
    }
}
