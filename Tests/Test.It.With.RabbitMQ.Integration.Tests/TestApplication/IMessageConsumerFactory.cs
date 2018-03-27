using System;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    internal interface IMessageConsumerFactory
    {
        IMessageConsumer Consume<TMessage>(string exchange, string queue, string routingKey, Action<TMessage> subscription);
    }
}