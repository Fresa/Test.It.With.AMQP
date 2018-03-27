using System.Collections.Concurrent;
using RabbitMQ.Client;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    internal class RabbitMqMessagePublisherFactory : IMessagePublisherFactory
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ISerializer _serializer;
        private readonly ConcurrentDictionary<string, IConnection> _connections = new ConcurrentDictionary<string, IConnection>();

        public RabbitMqMessagePublisherFactory(IConnectionFactory connectionFactory, ISerializer serializer)
        {
            _connectionFactory = connectionFactory;
            _serializer = serializer;
        }

        public IMessagePublisher Create(string exchange)
        {
            var connection = _connections.GetOrAdd(exchange, ex => _connectionFactory.CreateConnection());
            var model = connection.CreateModel();
            model.ExchangeDeclare(exchange, "topic");
            return new RabbitMqMessagePublisher(model, exchange, _serializer);
        }

        public void Dispose()
        {
            foreach (var connection in _connections.Values)
            {
                connection.Dispose();
            }
        }
    }
}