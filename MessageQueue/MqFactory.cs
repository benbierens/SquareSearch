using Logging;
using RabbitMQ.Client;

namespace MessageQueue
{
    internal class MqFactory
    {
        private readonly ILogger logger;
        private readonly string hostname;
        private readonly ILogger local;

        internal MqFactory(ILogger logger, string hostname)
        {
            this.logger = logger;
            this.hostname = hostname;
            local = logger.Prefix(nameof(MqFactory));
        }

        internal async Task<IMqSender> OpenSender(string topic)
        {
            local.Info($"{nameof(OpenSender)}({topic})");
            var channel = await CreateChannel();
            await channel.QueueDeclareAsync(queue: topic, durable: false, exclusive: false, autoDelete: false, arguments: null);
            return new MqSender(logger, channel, topic);
        }

        internal async Task<IMqReceiver> OpenReceiver<T>(string topic, IMqMessageHandler<T> handler) where T : class
        {
            local.Info($"{nameof(OpenReceiver)}({topic}, handler)");
            var channel = await CreateChannel();
            var receiver = new MqReceiver<T>(logger, channel, topic, handler);
            await receiver.Start();
            return receiver;
        }

        private async Task<IChannel> CreateChannel()
        {
            var factory = new ConnectionFactory { HostName = hostname };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
            return channel;
        }
    }
}
