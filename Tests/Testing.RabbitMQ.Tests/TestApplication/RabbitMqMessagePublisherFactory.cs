using System;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Test.It.With.RabbitMQ.Tests.TestApplication
{
    internal class RabbitMqMessagePublisherFactory : IMessagePublisherFactory
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ISerializer _serializer;
        private readonly Dictionary<string, IConnection> _connections = new Dictionary<string, IConnection>();

        public RabbitMqMessagePublisherFactory(IConnectionFactory connectionFactory, ISerializer serializer)
        {
            _connectionFactory = connectionFactory;
            _serializer = serializer;
        }

        public IMessagePublisher Create(string exchange)
        {
            var connection = _connections.GetOrAdd(exchange, () => _connectionFactory.CreateConnection());
            var model = connection.CreateModel();
            model.ExchangeDeclare(exchange, "topic");
            return new RabbitMqMessagePublisher(model, exchange, _serializer);
        }

        public void Dispose()
        {
            foreach (var connection in _connections.Values)
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }

    internal static class DictionaryExtensions
    {
        internal static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> valueFactory)
        {
            if (dictionary.ContainsKey(key) == false)
            {
                dictionary.Add(key, valueFactory());
            }
            
            return dictionary[key];
        }
    }
}