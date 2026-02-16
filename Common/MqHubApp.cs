using Logging;
using MessageQueue;

namespace Common
{
    public abstract class MqHubApp
    {
        protected ILogger Logger {get; private set;}
        protected MqHub Hub { get; private set; }

        protected abstract string Name { get; }

        public MqHubApp()
        {
            var sink = new MuxingSink(
                new ConsoleSink(),
                new FileSink($"/logs/{Environment.MachineName}/{Name.ToLowerInvariant()}.log")
            );
                
            Logger = new PrefixingLoggger(
                new SinkingLogger(sink), Name);

            var queueHost = Environment.GetEnvironmentVariable("MQHOST");
            Logger.Info($"MQHOST = {queueHost}");
            if (string.IsNullOrEmpty(queueHost)) throw new ArgumentNullException();

            Hub = new MqHub(Logger, queueHost);
        }
    }
}
