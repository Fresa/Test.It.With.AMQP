using System;
using System.Text;
using RabbitMQ.Client;
using Testing.Framework;
using Testing.Framework.Fixtures;
using Testing.Framework.Specifications;

namespace Testing.RabbitMQ.Tests
{
    public class When_publishing_a_message : WindowsServiceSpecification<WindowsServiceHostingFixture<TestApplicationBuilder>>
    {
        private RabbitMqTestFramework.ClientEnvelope<TestMessage> _testMessagePublished;

        protected override void Given(IServiceContainer container)
        {
            var rabbitMqTestServer = new RabbitMqTestFramework(new NewtonsoftSerializer(Encoding.UTF8), new Lazy<IConnectionFactory>(() => new ConnectionFactory()));
            rabbitMqTestServer.OnPublish<TestMessage>(envelope => _testMessagePublished = envelope);

            container.Register(() => rabbitMqTestServer.ConnectionFactory);
        }

        protected override void When()
        {
        }
    }
}
