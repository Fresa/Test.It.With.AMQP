using NLog.Config;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Test.It.With.RabbitMQ.Messages;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder>>
    {
        private MethodFrame<Connection.StartOk> _startOkMethod;
        private MethodFrame<Channel.Flow> _channelFlowMessage;

        public When_publishing_a_message(ITestOutputHelper output) : base(output)
        {

        }

        protected override void Given(IServiceContainer container)
        {
            var testServer = new AmqpTestFramework();
            testServer.On<Connection.StartOk>(startOkMethod =>
            {
                _startOkMethod = startOkMethod;
                Client.Disconnect();
            });

            testServer.On<Channel.Flow, Channel.FlowOk>(openMessage =>
            {
                _channelFlowMessage = openMessage;
                return openMessage.Method.Respond(new Channel.FlowOk());
            });


            container.Register(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
        }

        [Fact]
        public void It_should_have_published_the_message()
        {
            _startOkMethod.Should().Not.Be.Null();
        }
    }
}
