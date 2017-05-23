using System;
using RabbitMQ.Client;

namespace Test.It.With.RabbitMQ.Tests
{
    public class TestApplication : IDisposable, IMessageQueueApplication
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ISerializer _serializer;
        private IConnection _connection;
        private IModel _channel;

        public TestApplication(IConnectionFactory connectionFactory, ISerializer serializer)
        {
            _connectionFactory = connectionFactory;
            _serializer = serializer;
        }

        public void Start()
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void DeclareExchange(string exchange)
        {
            _channel.ExchangeDeclare(exchange, "topic");
        }

        public void DeclareQueue(string queue)
        {
            _channel.QueueDeclare(queue,
                false,
                false,
                false,
                null);
        }

        public void BindQueueToExchange(string queue, string exchange, string routingkey)
        {
            _channel.QueueBind(queue, exchange, routingkey);
        }

        public void Send<TMessage>(TMessage message)
        {
            _channel.BasicPublish("",
                "hello",
                null,
                _serializer.Serialize(message));
            Console.WriteLine($"Sent {message}");
        }

        public void Dispose()
        {
            _channel.Close();
            _channel.Dispose();
            
            _connection.Close();
            _connection.Dispose();
        }
    }
}
