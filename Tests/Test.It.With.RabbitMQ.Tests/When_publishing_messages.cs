using System;
using System.Collections.Concurrent;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol._091;
using Test.It.With.RabbitMQ.Tests.Assertion;
using Test.It.With.RabbitMQ.Tests.FrameworkExtensions;
using Test.It.With.RabbitMQ.Tests.TestApplication;
using Test.It.With.RabbitMQ.Tests.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests
{
    namespace Given_a_client_application_sending_messages_over_rabbitmq
    {
        public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<
            TestApplicationBuilder<MessageSendingApplicationSpecification>>>
        {
            private readonly ConcurrentBag<MethodFrame<Exchange.Declare>> _exchangeDeclare =
                new ConcurrentBag<MethodFrame<Exchange.Declare>>();

            private readonly ConcurrentBag<MethodFrame<Basic.Publish>> _basicPublish =
                new ConcurrentBag<MethodFrame<Basic.Publish>>();

            public When_publishing_a_message(ITestOutputHelper output) : base(output)
            {
            }

            private const int NumberOfPublishes = 4;

            protected override string[] StartParameters { get; } = {NumberOfPublishes.ToString()};

            protected override TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

            protected override void Given(IServiceContainer container)
            {
                var closedChannels = new ConcurrentBag<short>();

                void TryStop()
                {
                    if (closedChannels.Count == NumberOfPublishes && _basicPublish.Count == NumberOfPublishes)
                    {
                        ServiceController.Stop();
                    }
                }

                var testServer = new AmqpTestFramework(ProtocolVersion.AMQP091);
                testServer
                    .WithDefaultProtocolHeaderNegotiation()
                    .WithDefaultSecurityNegotiation(heartbeatInterval: TimeSpan.FromSeconds(5))
                    .WithDefaultConnectionOpenNegotiation()
                    .WithHeartbeats(interval: TimeSpan.FromSeconds(5))
                    .WithDefaultConnectionCloseNegotiation();

                testServer.On<Channel.Open, Channel.OpenOk>((connectionId, frame) => new Channel.OpenOk());
                testServer.On<Channel.Close, Channel.CloseOk>((connectionId, frame) => new Channel.CloseOk());
                testServer.On<Exchange.Declare, Exchange.DeclareOk>((connectionId, frame) =>
                {
                    _exchangeDeclare.Add(frame);
                    return new Exchange.DeclareOk();
                });
                testServer.On<Channel.Close>((id, frame) =>
                {
                    closedChannels.Add(frame.Channel);
                    TryStop();
                });
                testServer.On<Basic.Publish>((connectionId, frame) =>
                {
                    _basicPublish.Add(frame);
                });

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
                    frame.Message.ContentHeader.Type.Equals(Shortstr.From(typeof(TestMessage).FullName)));
            }

            [Fact]
            public void It_should_have_published_the_correct_message()
            {
                _basicPublish.Should().Contain.Any(frame =>
                    frame.Message.ContentBody.Deserialize<TestMessage>().Message
                        .Equals("Testing sending a message using RabbitMQ"));
            }

            [Fact]
            public void It_should_have_published_the_message_on_the_correct_exchange()
            {
                _basicPublish.Should().Contain()
                    .Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
                _basicPublish.Should().Contain()
                    .Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_exchanges()
            {
                _exchangeDeclare.Should().Count.Exactly(4);
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_name()
            {
                _exchangeDeclare.Should().Contain()
                    .Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
                _exchangeDeclare.Should().Contain()
                    .Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_type()
            {
                _exchangeDeclare.Should().Contain.Any(frame => frame.Message.Type.Equals(Shortstr.From("topic")));
            }
        }
    }
}
