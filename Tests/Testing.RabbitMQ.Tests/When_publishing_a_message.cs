using NLog.Config;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder>>
    {
        private MethodFrame<Connection.StartOk> _testMessagePublished;
        private MethodFrame<Connection.Open> _openMessage;

        public When_publishing_a_message(ITestOutputHelper output) : base(output)
        {

        }

        protected override void Given(IServiceContainer container)
        {
            var rabbitMqTestServer = new RabbitMqTestFramework2();
            rabbitMqTestServer.On<Connection.StartOk>(envelope =>
            {
                _testMessagePublished = envelope;
                Client.Disconnect();
            });

            rabbitMqTestServer.On<Connection.Open, Connection.OpenOk>(envelope =>
            {
                _openMessage = envelope;
                return envelope.Method.Respond(new Connection.OpenOk());
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
