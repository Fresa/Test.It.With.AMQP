using System;
using System.Text;
using RabbitMQ.Client;
using Should.Fluent;
using Test.It.Hosting.A.WindowsService;
using Test.It.With.RabbitMQ.Tests.TestApplication;
using Xunit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceFixture<TestApplicationBuilder>>
    {
        private RabbitMqTestFramework.ClientEnvelope<TestMessage> _testMessagePublished;

        protected override void Given(IServiceContainer container)
        {
            var rabbitMqTestServer = new RabbitMqTestFramework(new NewtonsoftSerializer(Encoding.UTF8), new Lazy<IConnectionFactory>(() => new ConnectionFactory()));
            rabbitMqTestServer.OnPublish<TestMessage>(envelope =>
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
