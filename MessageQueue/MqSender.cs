using Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MessageQueue
{
    public interface IMqSender
    {
        Task<uint> GetQueueLength();
        Task Send<T>(T message) where T : class;
    }

    internal class MqSender : IMqSender
    {
        private readonly ILogger logger;
        private readonly IChannel channel;
        private readonly string topic;

        internal MqSender(ILogger logger, IChannel channel, string topic)
        {
            this.logger = logger.Prefix(nameof(MqSender));
            this.channel = channel;
            this.topic = topic;
        }

        public async Task<uint> GetQueueLength()
        {
            try
            {
                var result = await channel.QueueDeclareAsync(queue: topic, durable: false, exclusive: false, autoDelete: false, passive: true);
                logger.Trace($"{nameof(GetQueueLength)}={result.MessageCount}");
                return result.MessageCount;
            }
            catch (Exception ex)
            {
                logger.Error(nameof(GetQueueLength), ex);
                return uint.MaxValue;
            }            
        }

        public async Task Send<T>(T message) where T : class
        {
            try
            {
                logger.Trace($"{nameof(Send)}<{typeof(T).Name}>");
                var str = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(str);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: topic, body: body);
            }
            catch (Exception e)
            {
                logger.Error(nameof(Send), e);
                throw;
            }
        }
    }
}
