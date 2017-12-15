using System;
using System.Text;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Test.It.With.RabbitMQ.Tests.TestApplication;
using Xunit;
using Xunit.Abstractions;
using Connection = Test.It.With.Amqp.Connection;

namespace Test.It.With.RabbitMQ.Tests
{
    namespace Given_a_rabbitmq_client
    {
        public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder>>
        {
            private MethodFrame<Exchange.Declare> _exchangeDeclare;
            private MethodFrame<Basic.Publish> _basicPublish;

            public When_publishing_a_message(ITestOutputHelper output) : base(output)
            {
            }

            protected override void Given(IServiceContainer container)
            {
                var testServer = new AmqpTestFramework(ProtocolVersion.AMQP091);
                
                testServer.On((Func<ProtocolHeaderFrame<ProtocolHeader>, Connection.Start>) (handler => new Connection.Start
                {
                    VersionMajor = new Octet((byte)handler.ProtocolHeader.Version.Major),
                    VersionMinor = new Octet((byte)handler.ProtocolHeader.Version.Minor),
                    Locales = new Longstr(Encoding.UTF8.GetBytes("en_US")),
                    Mechanisms = new Longstr(Encoding.UTF8.GetBytes("PLAIN")),
                }));
                testServer.On<Connection.StartOk>(frame =>
                {
                    testServer.Send(new MethodFrame<Connection.Secure>(frame.Channel, new Connection.Secure
                    {
                        Challenge = new Longstr(Encoding.UTF8.GetBytes("challenge"))
                    }));
                });
                testServer.On<Connection.SecureOk>(frame =>
                {
                    testServer.Send(new MethodFrame<Connection.Tune>(frame.Channel, new Connection.Tune
                    {
                        ChannelMax = new Short(0),
                        FrameMax = new Long(0),
                        Heartbeat = new Short(5)
                    }));
                });
                testServer.On<Connection.TuneOk>(_ => { });
                testServer.On<Connection.Open, Connection.OpenOk>(frame => new Connection.OpenOk());
                testServer.On<Connection.Close, Connection.CloseOk>(frame => new Connection.CloseOk());
                testServer.On<Channel.Open, Channel.OpenOk>(frame => new Channel.OpenOk());
                testServer.On<Channel.Close>(frame =>
                {
                    testServer.Send(new MethodFrame<Channel.CloseOk>(frame.Channel, new Channel.CloseOk()));

                    ServiceController.Stop();
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
                testServer.On<Heartbeat>(_ => { });

                container.RegisterSingleton(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
            }

            [Fact]
            public void It_should_have_published_the_correct_message_type()
            {
                _basicPublish.Method.ContentHeader.Type.Should().Equal(Shortstr.From(typeof(TestMessage).FullName));
            }

            [Fact]
            public void It_should_have_published_the_correct_message()
            {
                _basicPublish.Method.ContentBody.Deserialize<TestMessage>().Message.Should().Equal("Testing sending a message using RabbitMQ");
            }

            [Fact]
            public void It_should_have_published_the_message_on_the_correct_exchange()
            {
                _basicPublish.Method.Exchange.Should().Equal(ExchangeName.From("myExchange"));
            }

            [Fact]
            public void It_should_have_declared_an_exchange()
            {
                _exchangeDeclare.Should().Not.Be.Null();
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_name()
            {
                _exchangeDeclare.Method.Exchange.Should().Equal(ExchangeName.From("myExchange"));
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_type()
            {
                _exchangeDeclare.Method.Type.Should().Equal(Shortstr.From("topic"));
            }
        }
    }
}
