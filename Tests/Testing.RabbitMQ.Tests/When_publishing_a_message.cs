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
        private MethodFrame<Connection.Start> _startMethod;
        private MethodFrame<Channel.Flow> _channelFlowMessage;

        public When_publishing_a_message(ITestOutputHelper output) : base(output)
        {

        }

        protected override void Given(IServiceContainer container)
        {
            var rabbitMqTestServer = new RabbitMqTestFramework2();
            rabbitMqTestServer.On<Connection.Start>(startMethod =>
            {
                _startMethod = startMethod;
                Client.Disconnect();
            });

            rabbitMqTestServer.On<Channel.Flow, Channel.FlowOk>(openMessage =>
            {
                _channelFlowMessage = openMessage;
                return openMessage.Method.Respond(new Channel.FlowOk());
            });


            container.Register(() => rabbitMqTestServer.ConnectionFactory);
        }

        [Fact]
        public void It_should_have_published_the_message()
        {
            _startMethod.Should().Not.Be.Null();
        }
    }
}
