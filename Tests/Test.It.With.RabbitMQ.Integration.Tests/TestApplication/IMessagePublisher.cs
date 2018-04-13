using System;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    internal interface IMessagePublisher : IDisposable
    {
        PublishResult Publish<TMessage>(string key, TMessage message);
    }
}