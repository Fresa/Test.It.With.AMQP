using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Test.It.With.RabbitMQ.Tests.TestApplication
{
    internal class RabbitMqMessageConsumer<TMessage> : IMessageConsumer
    {
        private readonly ISerializer _serializer;
        private readonly Action<TMessage> _subscription;
        private readonly EventingBasicConsumer _consumer;
        private readonly string _consumerTag;

        public RabbitMqMessageConsumer(IModel model, ISerializer serializer, string queue, Action<TMessage> subscription)
        {
            _serializer = serializer;
            _subscription = subscription;

            _consumer = new EventingBasicConsumer(model);
            _consumer.Received += OnReceived;
            _consumerTag = _consumer.Model.BasicConsume(queue, false, _consumer);
        }

        private void OnReceived(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                _subscription.Invoke(_serializer.Deserialize<TMessage>(args.Body));
                _consumer.Model.BasicAck(args.DeliveryTag, false);
            }
            catch
            {
                _consumer.Model.BasicNack(args.DeliveryTag, false, false);
            }
        }

        public void Dispose()
        {
            _consumer.Model.BasicCancel(_consumerTag);
            _consumer.Received -= OnReceived;
            _consumer.Model.Dispose();
        }
    }
}