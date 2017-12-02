using System;
using System.Text;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder>>
    {
        private MethodFrame<Connection.StartOk> _startOkMethod;
        private MethodFrame<Channel.Flow> _channelFlowMessage;
        private MethodFrame<Connection.Close> _closeMethod;

        protected override TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);

        public When_publishing_a_message(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Given(IServiceContainer container)
        {
            var testServer = new AmqpTestFramework();
            testServer.On<Connection.StartOk>(startOkMethod =>
            {
                _startOkMethod = startOkMethod;
                ServiceController.Stop();
            });

            testServer.On<Channel.Flow, Channel.FlowOk>(openMessage =>
            {
                _channelFlowMessage = openMessage;
                return openMessage.Method.Respond(new Channel.FlowOk());
            });
            
            testServer.OnProtocolHeader(header => new Connection.Start
            {
                VersionMajor = new Octet((byte)header.Version.Major),
                VersionMinor = new Octet((byte)header.Version.Minor),
                ServerProperties = new PeerProperties(),
                Locales = new Longstr(Encoding.UTF8.GetBytes("en_US")),
                Mechanisms = new Longstr(Encoding.UTF8.GetBytes("PLAIN"))
            });

            testServer.On<Connection.Close, Connection.CloseOk>(frame =>
            {
                _closeMethod = frame;
                return frame.Method.Respond(new Connection.CloseOk());
            });

            container.RegisterSingleton(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
        }

        [Fact]
        public void It_should_have_published_the_message()
        {
            _startOkMethod.Should().Not.Be.Null();
        }
    }
}
