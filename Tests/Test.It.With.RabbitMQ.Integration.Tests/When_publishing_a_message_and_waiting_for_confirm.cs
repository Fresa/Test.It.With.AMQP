using System;
using System.Collections.Concurrent;
using System.Linq;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol._091;
using Test.It.With.RabbitMQ.Integration.Tests.Assertion;
using Test.It.With.RabbitMQ.Integration.Tests.FrameworkExtensions;
using Test.It.With.RabbitMQ.Integration.Tests.TestApplication;
using Test.It.With.RabbitMQ.Integration.Tests.TestApplication.Specifications;
using Test.It.With.RabbitMQ.Integration.Tests.XUnit;
using Test.It.With.RabbitMQ._091;
using Xunit;
using Xunit.Abstractions;
using Basic = Test.It.With.RabbitMQ._091.Basic;

namespace Test.It.With.RabbitMQ.Integration.Tests
{
    namespace Given_a_client_application_sending_messages_over_rabbitmq
    {
        public class When_publishing_a_message_and_waiting_for_confirm : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<
            TestApplicationBuilder<ConfirmSelectApplication>>>
        {
            private readonly ConcurrentBag<MethodFrame<Exchange.Declare>> _exchangeDeclare =
                new ConcurrentBag<MethodFrame<Exchange.Declare>>();

            private readonly ConcurrentBag<MethodFrame<Amqp.Protocol._091.Basic.Publish>> _basicPublish =
                new ConcurrentBag<MethodFrame<Amqp.Protocol._091.Basic.Publish>>();

            private bool _selectOkSent;

            public When_publishing_a_message_and_waiting_for_confirm(ITestOutputHelper output) : base(output)
            {
            }

            protected override TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

            protected override void Given(IServiceContainer container)
            {
                var channelClosed = false;

                void TryStop()
                {
                    if (channelClosed && _basicPublish.Count == 2)
                    {
                        ServiceController.Stop();
                    }
                }

                var testServer = new AmqpTestFramework(RabbitMq091.ProtocolResolver);
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
                    channelClosed = true;
                    TryStop();
                });
                testServer.On<Amqp.Protocol._091.Basic.Publish>((connectionId, frame) =>
                {
                    _basicPublish.Add(frame);
                    testServer.Send(connectionId,
                        new MethodFrame<Basic.Ack>(frame.Channel,
                            new Basic.Ack
                            {
                                DeliveryTag =
                                    DeliveryTag.From(_basicPublish.Count(methodFrame =>
                                        methodFrame.Channel == frame.Channel))
                            }));
                });
                testServer.On<Confirm.Select, Confirm.SelectOk>((connectionId, frame) =>
                {
                    _selectOkSent = true;
                    return new Confirm.SelectOk();
                });

                container.RegisterSingleton(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
            }

            [Fact]
            public void It_should_have_published_messages()
            {
                _basicPublish.Should().Count.Exactly(2);
            }

            [Fact]
            public void It_should_have_published_the_correct_message_type()
            {
                _basicPublish.Should().Contain().Two(frame =>
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
            public void It_should_have_published_a_confirming_message()
            {
                _basicPublish.Should().Contain.Any(frame =>
                    frame.Message.ContentBody.Deserialize<TestMessage>().Message
                        .Equals("Test message has been confirmed"));
            }

            [Fact]
            public void It_should_have_published_the_message_on_the_correct_exchange()
            {
                _basicPublish.Should().Contain()
                    .Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
            }

            [Fact]
            public void It_should_have_declared_exchanges()
            {
                _exchangeDeclare.Should().Count.Exactly(1);
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_name()
            {
                _exchangeDeclare.Should().Contain()
                    .One(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_type()
            {
                _exchangeDeclare.Should().Contain.Any(frame => frame.Message.Type.Equals(Shortstr.From("topic")));
            }

            [Fact]
            public void It_should_have_sent_confirm_select_ok()
            {
                _selectOkSent.Should().Be.True();
            }
        }
    }
}
