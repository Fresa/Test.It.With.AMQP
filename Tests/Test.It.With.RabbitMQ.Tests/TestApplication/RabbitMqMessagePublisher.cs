using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace Test.It.With.RabbitMQ.Tests.TestApplication
{
    internal class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly IModel _model;
        private readonly string _exchange;
        private readonly ISerializer _serializer;

        public RabbitMqMessagePublisher(IModel model, string exchange, ISerializer serializer)
        {
            _model = model;
            _exchange = exchange;
            _serializer = serializer;
        }

        public string Publish<TMessage>(string key, TMessage message)
        {
            var correlationId = Guid.NewGuid().ToString();
            _model.BasicPublish(_exchange, key, new BasicProperties
            {
                Type = message.GetType().FullName,
                CorrelationId = correlationId
            }, _serializer.Serialize(message));
            return correlationId;
        }

        public void Dispose()
        {
            _model.Dispose();
        }
    }
}