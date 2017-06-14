using System;

namespace Test.It.With.RabbitMQ.Tests.TestApplication
{
    public interface IMessagePublisherFactory : IDisposable
    {
        IMessagePublisher Create(string exchange);
    }
}