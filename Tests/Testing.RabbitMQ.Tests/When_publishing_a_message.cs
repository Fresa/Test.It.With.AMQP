using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client.Framing.Impl;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Xunit;
using Xunit.Abstractions;
using Connection = Test.It.With.Amqp.Connection;

namespace Test.It.With.RabbitMQ.Tests
{
    public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder>>
    {
        private MethodFrame<Connection.Close> _closeMethod;
        private MethodFrame<Connection.SecureOk> _secureOk;
        private MethodFrame<Connection.StartOk> _startOk;
        private MethodFrame<Connection.TuneOk> _tuneOk;
        private MethodFrame<Connection.Open> _open;
        private readonly List<HeartbeatFrame<Heartbeat>> _heartbeats = new List<HeartbeatFrame<Heartbeat>>();
        private MethodFrame<Channel.Open> _channelOpen;
        private MethodFrame<Connection.CloseOk> _closeOk;
        private MethodFrame<Exchange.Declare> _exchangeDeclare;
        private MethodFrame<Basic.Publish> _basicPublish;
        private MethodFrame<Channel.Close> _channelClose;

        protected override TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

        public When_publishing_a_message(ITestOutputHelper output) : base(output)
        {
        }

        protected override void Given(IServiceContainer container)
        {
            var testServer = new AmqpTestFramework();
            
            testServer.OnProtocolHeader(header => new Connection.Start
            {
                VersionMajor = new Octet((byte)header.Version.Major),
                VersionMinor = new Octet((byte)header.Version.Minor),
                ServerProperties = new PeerProperties(),
                Locales = new Longstr(Encoding.UTF8.GetBytes("en_US")),
                Mechanisms = new Longstr(Encoding.UTF8.GetBytes("PLAIN")),
            });

            testServer.On<Connection.Close, Connection.CloseOk>(frame =>
            {
                _closeMethod = frame;
                return new Connection.CloseOk();
            });

            testServer.On<Connection.StartOk>(frame =>
            {
                _startOk = frame;
                testServer.Send(new MethodFrame<Connection.Secure>(frame.Channel, new Connection.Secure
                {
                    Challenge = new Longstr(Encoding.UTF8.GetBytes("challenge"))
                }));
            });

            testServer.On<Connection.SecureOk>(frame =>
            {
                _secureOk = frame;
                testServer.Send(new MethodFrame<Connection.Tune>(frame.Channel, new Connection.Tune
                {
                    ChannelMax = new Short(0),
                    FrameMax = new Long(0),
                    Heartbeat = new Short(5)
                }));
            });

            testServer.On<Connection.TuneOk>(frame =>
            {
                _tuneOk = frame;
            });

            testServer.On<Connection.Open, Connection.OpenOk>(frame =>
            {
                _open = frame;
                return new Connection.OpenOk();
            });

            testServer.On<Channel.Open, Channel.OpenOk>(frame =>
            {
                _channelOpen = frame;
                return new Channel.OpenOk();
            });

            testServer.On<Exchange.Declare, Exchange.DeclareOk>(frame =>
            {
                _exchangeDeclare = frame;
                return new Exchange.DeclareOk();
            });

            testServer.On<Basic.Publish>(frame =>
            {
                _basicPublish = frame;
            });

            testServer.On<Channel.Close>(frame =>
            {
                _channelClose = frame;
                testServer.Send(new MethodFrame<Channel.CloseOk>(frame.Channel, new Channel.CloseOk()));

                ServiceController.Stop();
            });

            testServer.On<Heartbeat>(frame =>
            {
                _heartbeats.Add(frame);
            });

            container.RegisterSingleton(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
        }

        [Fact]
        public void It_should_have_published_the_message()
        {
            _open.Should().Not.Be.Null();
        }
    }
}
