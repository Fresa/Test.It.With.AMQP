using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.While.Hosting.Your.Windows.Service.Delegates;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091;
using Test.It.With.RabbitMQ.Tests.Assertion;
using Test.It.With.RabbitMQ.Tests.FrameworkExtensions;
using Test.It.With.RabbitMQ.Tests.TestApplication;
using Test.It.With.RabbitMQ.Tests.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests
{
    namespace Given_a_client_application_receiving_messages_over_rabbitmq
    {
        public class When_consuming_messages : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<
            TestApplicationBuilder<MessageConsumingApplicationSpecification>>>
        {
            private readonly ConcurrentBag<MethodFrame<Exchange.Declare>> _exchangesDeclared = new ConcurrentBag<MethodFrame<Exchange.Declare>>();
            private readonly ConcurrentBag<MethodFrame<Queue.Declare>> _queuesDeclared = new ConcurrentBag<MethodFrame<Queue.Declare>>();
            private readonly ConcurrentBag<MethodFrame<Queue.Bind>> _queuesBound = new ConcurrentBag<MethodFrame<Queue.Bind>>();
            private readonly ConcurrentBag<MethodFrame<Basic.Publish>> _basicPublishes = new ConcurrentBag<MethodFrame<Basic.Publish>>();
            private readonly ConcurrentBag<MethodFrame<Basic.Consume>> _basicConsumes = new ConcurrentBag<MethodFrame<Basic.Consume>>();
            private readonly ConcurrentBag<MethodFrame<Basic.Ack>> _acks = new ConcurrentBag<MethodFrame<Basic.Ack>>();

            public When_consuming_messages(ITestOutputHelper output) : base(output)
            {
            }

            private const int Parallelism = 2;

            protected override string[] StartParameters { get; } = { Parallelism.ToString() };

            protected override TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(50);

            protected override void Given(IServiceContainer container)
            {
                var closedChannels = new ConcurrentBag<short>();

                void TryStop()
                {
                    if (closedChannels.Count == Parallelism && _basicPublishes.Count == Parallelism && _acks.Count == Parallelism)
                    {
                        ServiceController.Stop();
                    }
                }

                var testServer = new AmqpTestFramework(Amqp091.ProtocolResolver);
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
                    _exchangesDeclared.Add(frame);
                    return new Exchange.DeclareOk();
                });
                testServer.On<Queue.Declare, Queue.DeclareOk>((connectionId, frame) =>
                {
                    _queuesDeclared.Add(frame);
                    return new Queue.DeclareOk();
                });
                testServer.On<Queue.Bind, Queue.BindOk>((connectionId, frame) =>
                {
                    _queuesBound.Add(frame);
                    return new Queue.BindOk();
                });

                testServer.On<Channel.Close>((id, frame) =>
                {
                    closedChannels.Add(frame.Channel);
                    TryStop();
                });
                testServer.On<Basic.Publish>((connectionId, frame) =>
                {
                    _basicPublishes.Add(frame);
                });
                testServer.On<Basic.Consume>((connectionId, frame) =>
                {
                    var consumerTag = Guid.NewGuid().ToString();
                    _basicConsumes.Add(frame);

                    // We need to respond with ConsumeOk before we can start delivering messages to the client
                    testServer.Send(connectionId, new MethodFrame<Basic.ConsumeOk>(frame.Channel,
                        new Basic.ConsumeOk
                        {
                            ConsumerTag = ConsumerTag.From(consumerTag)
                        }));

                    Task.Run(() =>
                    {
                        var testMessage = new TestMessage("This is sent from server.");
                        var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(testMessage));

                        testServer.Send<Basic.Deliver, Basic.ContentHeader>(connectionId,
                            new MethodFrame<Basic.Deliver>(frame.Channel,
                                new Basic.Deliver
                                {
                                    ConsumerTag = ConsumerTag.From(consumerTag),
                                    ContentHeader = new Basic.ContentHeader
                                    {
                                        BodySize = payload.Length
                                    }
                                }
                                .AddContentBodyFragments(new ContentBody
                                {
                                    Payload = payload
                                })));
                    });
                });
                testServer.On<Basic.Cancel, Basic.CancelOk>((connectionId, frame) =>
                    new Basic.CancelOk { ConsumerTag = frame.Message.ConsumerTag });
                testServer.On<Basic.Ack>((connectionId, frame) =>
                {
                    _acks.Add(frame);
                    TryStop();
                });

                ServiceController.OnStopped += code =>
                {
                    testServer.Dispose();
                };

                container.RegisterSingleton(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
            }

            [Fact]
            public void It_should_have_published_messages()
            {
                _basicPublishes.Should().Count.Exactly(2);
            }

            [Fact]
            public void It_should_have_published_the_correct_message_type()
            {
                _basicPublishes.Should().Contain().Two(frame =>
                    frame.Message.ContentHeader.Type.Equals(Shortstr.From(typeof(TestMessage).FullName)));
            }

            [Fact]
            public void It_should_have_published_the_correct_message()
            {
                _basicPublishes.Should().Contain.One(frame =>
                    frame.Message.ContentBody.Deserialize<TestMessage>().Message
                        .Equals("0: This is sent from server."));
                _basicPublishes.Should().Contain.One(frame =>
                    frame.Message.ContentBody.Deserialize<TestMessage>().Message
                        .Equals("1: This is sent from server."));
            }

            [Fact]
            public void It_should_have_published_the_message_on_the_correct_exchange()
            {
                _basicPublishes.Should().Contain()
                    .One(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
                _basicPublishes.Should().Contain()
                    .One(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_exchanges()
            {
                _exchangesDeclared.Should().Count.Exactly(4);
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_name()
            {
                _exchangesDeclared.Should().Contain()
                    .Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
                _exchangesDeclared.Should().Contain()
                    .Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_queues()
            {
                _queuesDeclared.Should().Count.Exactly(2);
            }

            [Fact]
            public void It_should_have_declared_queues_with_name()
            {
                _queuesDeclared.Should().Contain()
                    .One(frame => frame.Message.Queue.Equals(QueueName.From("queue0")));
                _queuesDeclared.Should().Contain()
                    .One(frame => frame.Message.Queue.Equals(QueueName.From("queue1")));
            }

            [Fact]
            public void It_should_have_bound_the_queues()
            {
                _queuesBound.Should().Count.Exactly(2);
            }

            [Fact]
            public void It_should_have_bound_queues_to_exchanges()
            {
                _queuesBound.Should().Contain()
                    .One(frame => 
                        frame.Message.Queue.Equals(QueueName.From("queue0")) &&
                        frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")) &&
                        frame.Message.RoutingKey.Equals(Shortstr.From("routing0")));

                _queuesBound.Should().Contain()
                    .One(frame => 
                        frame.Message.Queue.Equals(QueueName.From("queue1")) &&
                        frame.Message.Exchange.Equals(ExchangeName.From("myExchange1")) &&
                        frame.Message.RoutingKey.Equals(Shortstr.From("routing1")));
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_type()
            {
                _exchangesDeclared.Should().Contain.Any(frame => frame.Message.Type.Equals(Shortstr.From("topic")));
            }

            [Fact]
            public void It_should_have_starting_consuming_messages()
            {
                _basicConsumes.Should().Count.Exactly(2);
            }

            [Fact]
            public void It_should_have_starting_consuming_messages_from_queues()
            {
                _basicConsumes.Should().Contain.One(frame => frame.Message.Queue.Equals(QueueName.From("queue0")));
                _basicConsumes.Should().Contain.One(frame => frame.Message.Queue.Equals(QueueName.From("queue1")));
            }

            [Fact]
            public void It_should_have_acked_consumed_messages()
            {
                _acks.Should().Count.Exactly(2);
            }
        }
    }
}
