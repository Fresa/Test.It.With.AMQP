using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    internal class RabbitMqMessageConsumerFactory : IMessageConsumerFactory
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ISerializer _serializer;
        private readonly ConcurrentDictionary<string, IConnection> _connections = new ConcurrentDictionary<string, IConnection>();

        public RabbitMqMessageConsumerFactory(IConnectionFactory connectionFactory, ISerializer serializer)
        {
            _connectionFactory = connectionFactory;
            _serializer = serializer;
        }

        public IMessageConsumer Consume<TMessage>(string exchange, string queue, string routingKey, Action<TMessage> subscription)
        {
            var connection = _connections.GetOrAdd(exchange, ex => _connectionFactory.CreateConnection());
            var model = connection.CreateModel();
            model.ExchangeDeclare(exchange, "topic");
            model.QueueDeclare(queue, false, false, false, new Dictionary<string, object>());
            model.QueueBind(queue, exchange, routingKey);
            return new RabbitMqMessageConsumer<TMessage>(model, _serializer, queue, subscription);
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