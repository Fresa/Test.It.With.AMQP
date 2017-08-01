using System;
using System.Text;
using RabbitMQ.Client;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.RabbitMQ.Tests.TestApplication;
using Xunit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder>>
    {
        private RabbitMqTestFramework2.ClientEnvelope<TestMessage> _testMessagePublished;

        protected override void Given(IServiceContainer container)
        {
            var rabbitMqTestServer = new RabbitMqTestFramework2(new NewtonsoftSerializer(Encoding.UTF8), new Lazy<IConnectionFactory>(() => new ConnectionFactory()));
            rabbitMqTestServer.Consume<TestMessage>(envelope =>
            {
                _testMessagePublished = envelope;
                Client.Disconnect();
            });

            container.Register(() => rabbitMqTestServer.ConnectionFactory);
        }

        [Fact]
        public void It_should_have_published_the_message()
        {
            _testMessagePublished.Should().Not.Be.Null();
        }
    }
}
