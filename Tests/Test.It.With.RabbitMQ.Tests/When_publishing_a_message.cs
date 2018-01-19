using System.Collections.Concurrent;
using System.Text;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091;
using Test.It.With.RabbitMQ.Tests.Assertion;
using Test.It.With.RabbitMQ.Tests.TestApplication;
using Test.It.With.RabbitMQ.Tests.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests
{
    namespace Given_a_rabbitmq_client
    {
        public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder>>
        {
            private readonly ConcurrentBag<MethodFrame<Exchange.Declare>> _exchangeDeclare = new ConcurrentBag<MethodFrame<Exchange.Declare>>();
            private readonly ConcurrentBag<MethodFrame<Basic.Publish>> _basicPublish = new ConcurrentBag<MethodFrame<Basic.Publish>>();

            public When_publishing_a_message(ITestOutputHelper output) : base(output)
            {
            }

            protected override string[] StartParameters { get; } = { "4" };

            protected override void Given(IServiceContainer container)
            {
                var testServer = new AmqpTestFramework(ProtocolVersion.AMQP091);

                testServer.On<ProtocolHeader>((clientId, handler) =>
                {
                    testServer.Send(clientId, new MethodFrame<Connection.Start>(handler.Channel, new Connection.Start
                    {
                        VersionMajor = Octet.From((byte) ((IProtocolHeader) handler.ProtocolHeader).Version.Major),
                        VersionMinor = Octet.From((byte) ((IProtocolHeader) handler.ProtocolHeader).Version.Minor),
                        Locales = Longstr.From(Encoding.UTF8.GetBytes("en_US")),
                        Mechanisms = Longstr.From(Encoding.UTF8.GetBytes("PLAIN")),
                    }));
                });
                testServer.On<Connection.StartOk>((client, frame) =>
                {
                    testServer.Send(client, new MethodFrame<Connection.Secure>(frame.Channel, new Connection.Secure
                    {
                        Challenge = Longstr.From(Encoding.UTF8.GetBytes("challenge"))
                    }));
                });
                testServer.On<Connection.SecureOk>((clientId, frame) =>
                {
                    testServer.Send(clientId, new MethodFrame<Connection.Tune>(frame.Channel, new Connection.Tune
                    {
                        ChannelMax = Short.From(0),
                        FrameMax = Long.From(0),
                        Heartbeat = Short.From(5)
                    }));
                });
                testServer.On<Connection.TuneOk>((__, _) => { });
                testServer.On<Connection.Open, Connection.OpenOk>((clientId, frame) => new Connection.OpenOk());
                testServer.On<Connection.Close, Connection.CloseOk>((clientId, frame) => new Connection.CloseOk());
                testServer.On<Channel.Open, Channel.OpenOk>((clientId, frame) => new Channel.OpenOk());
                testServer.On<Channel.Close>((clientId, frame) =>
                {
                    testServer.Send(clientId, new MethodFrame<Channel.CloseOk>(frame.Channel, new Channel.CloseOk()));

                    if (_basicPublish.Count == 4)
                    {
                        ServiceController.Stop();
                    }
                });
                testServer.On<Exchange.Declare, Exchange.DeclareOk>((clientId, frame) =>
                {
                    _exchangeDeclare.Add(frame);
                    return new Exchange.DeclareOk();
                });
                testServer.On<Basic.Publish>((clientId, frame) =>
                {
                    _basicPublish.Add(frame);
                });
                testServer.On<Heartbeat>((__, _) => { });

                container.RegisterSingleton(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
            }

            [Fact]
            public void It_should_have_published_messages()
            {
                _basicPublish.Should().Count.Exactly(4);
            }

            [Fact]
            public void It_should_have_published_the_correct_message_type()
            {
                _basicPublish.Should().Contain().Four(frame =>
                    frame.Method.ContentHeader.Type.Equals(Shortstr.From(typeof(TestMessage).FullName)));
            }

            [Fact]
            public void It_should_have_published_the_correct_message()
            {
                _basicPublish.Should().Contain.Any(frame => frame.Method.ContentBody.Deserialize<TestMessage>().Message.Equals("Testing sending a message using RabbitMQ"));
            }

            [Fact]
            public void It_should_have_published_the_message_on_the_correct_exchange()
            {
                _basicPublish.Should().Contain().Two(frame => frame.Method.Exchange.Equals(ExchangeName.From("myExchange0")));
                _basicPublish.Should().Contain().Two(frame => frame.Method.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_exchanges()
            {
                _exchangeDeclare.Should().Count.Exactly(4);
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_name()
            {
                _exchangeDeclare.Should().Contain().Two(frame => frame.Method.Exchange.Equals(ExchangeName.From("myExchange0")));
                _exchangeDeclare.Should().Contain().Two(frame => frame.Method.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_type()
            {
                _exchangeDeclare.Should().Contain.Any(frame => frame.Method.Type.Equals(Shortstr.From("topic")));
            }
        }
    }

    // todo: add test that sends content and heartbeats
}
