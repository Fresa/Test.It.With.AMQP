using System;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    public interface IMessagePublisher : IDisposable
    {
        string Publish<TMessage>(string key, TMessage message);
    }
}