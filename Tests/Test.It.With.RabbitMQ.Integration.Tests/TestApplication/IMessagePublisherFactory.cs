using System;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    public interface IMessagePublisherFactory : IDisposable
    {
        IMessagePublisher Create(string exchange);
    }
}