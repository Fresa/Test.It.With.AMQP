using RabbitMQ.Client;

namespace Test.It.With.RabbitMQ.Tests.TestApplication
{
    internal class RabbitMqMessagePublisherFactory : IMessagePublisherFactory
    {
        private readonly IConnection _connection;
        private readonly ISerializer _serializer;

        public RabbitMqMessagePublisherFactory(IConnection connection, ISerializer serializer)
        {
            _connection = connection;
            _serializer = serializer;
        }

        public IMessagePublisher Create(string exchange)
        {
            var model = _connection.CreateModel();
            model.ExchangeDeclare(exchange, "topic");
            return new RabbitMqMessagePublisher(model, exchange, _serializer);
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}