using System;

namespace Test.It.With.RabbitMQ.Integration.Tests.TestApplication
{
    internal interface IMessagePublisherFactory : IDisposable
    {
        IMessagePublisher Create(string exchange);
    }
}