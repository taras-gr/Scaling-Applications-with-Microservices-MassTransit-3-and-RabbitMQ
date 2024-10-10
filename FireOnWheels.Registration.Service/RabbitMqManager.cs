using FireOnWheels.Messaging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace FireOnWheels.Registration.Service
{
    public class RabbitMqManager : IDisposable
    {
        private readonly IModel channel;

        public RabbitMqManager()
        {
            var connectionFactory = new ConnectionFactory { Uri = new Uri(RabbitMqConstants.RabbitMqUri) };
            var connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void ListenForRegisterOrderCommand()
        {
            channel.QueueDeclare(
                queue: RabbitMqConstants.RegisterOrderQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 0,
                global: false);

            var consumer = new RegisteredOrderCommandConsumer(this);
            channel.BasicConsume(
                queue: RabbitMqConstants.RegisterOrderQueue,
                autoAck: true,
                consumerTag: "registerOrderConsumer",
                consumer: consumer,
                noLocal: false,
                exclusive: true,
                arguments: null);
        }

        public void SendOrderRegisteredEvent(IOrderRegisteredEvent command)
        {
            channel.ExchangeDeclare(
                exchange: RabbitMqConstants.OrderRegisteredExchange,
                type: ExchangeType.Fanout);
            channel.QueueDeclare(
                queue: RabbitMqConstants.OrderRegisteredNotificationQueue,
                durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            channel.QueueBind(
                queue: RabbitMqConstants.OrderRegisteredNotificationQueue,
                exchange: RabbitMqConstants.OrderRegisteredExchange,
                routingKey: "");

            var serializedCommand = JsonConvert.SerializeObject(command);

            var messageProperties = channel.CreateBasicProperties();
            messageProperties.ContentType = RabbitMqConstants.JsonMimeType;

            channel.BasicPublish(
                exchange: RabbitMqConstants.OrderRegisteredExchange,
                routingKey: "",
                basicProperties: messageProperties,
                body: Encoding.UTF8.GetBytes(serializedCommand));
        }

        public void SendAck(ulong deliveryTag)
        {
            channel.BasicAck(deliveryTag: deliveryTag, multiple: false);
        }

        public void Dispose()
        {
            if (channel.IsOpen)
                channel.Close();
        }
    }

}
