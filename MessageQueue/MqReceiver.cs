using Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MessageQueue
{
    public interface IMqReceiver
    {
    }

    public interface IMqMessageHandler<T> where T : class
    {
        Task OnMessage(T message, IMsMessageAcknowledger ack);
    }

    public interface IMsMessageAcknowledger
    {
        Task AckMessage();
    }

    internal class MqReceiver<T> : IMqReceiver where T : class
    {
        private readonly ILogger logger;
        private readonly IChannel channel;
        private readonly string topic;
        private readonly IMqMessageHandler<T> handler;

        internal MqReceiver(ILogger logger, IChannel channel, string topic, IMqMessageHandler<T> handler)
        {
            this.logger = logger.Prefix(nameof(MqReceiver<T>));
            this.channel = channel;
            this.topic = topic;
            this.handler = handler;
        }

        internal async Task Start()
        {
            var workConsumer = new AsyncEventingBasicConsumer(channel);
            workConsumer.ReceivedAsync += HandleQueueMessage;
            await channel.BasicConsumeAsync(topic, autoAck: false, consumer: workConsumer);
        }

        private async Task HandleQueueMessage(object sender, BasicDeliverEventArgs @event)
        {
            var message = DecodeMessage(@event);
            if (message == null) return;

            var ack = new Acknowledger(channel, @event.DeliveryTag);

            try
            {
                await handler.OnMessage(message, ack);
            }
            catch (Exception ex)
            {
                logger.Error(nameof(HandleQueueMessage), ex);
            }
        }

        private T? DecodeMessage(BasicDeliverEventArgs @event)
        {
            try
            {
                var body = @event.Body.ToArray();
                var str = Encoding.UTF8.GetString(body);
                var msg = JsonConvert.DeserializeObject<T>(str);
                if (msg == null) throw new Exception("Message decoded to null");
                return msg;
            }
            catch (Exception ex)
            {
                logger.Error(nameof(DecodeMessage), ex);
                return null;
            }
        }

        private class Acknowledger : IMsMessageAcknowledger
        {
            private readonly IChannel channel;
            private readonly ulong deliveryTag;

            public Acknowledger(IChannel channel, ulong deliveryTag)
            {
                this.channel = channel;
                this.deliveryTag = deliveryTag;
            }

            public async Task AckMessage()
            {
                await channel.BasicAckAsync(deliveryTag: deliveryTag, multiple: false);
            }
        }
    }
}
