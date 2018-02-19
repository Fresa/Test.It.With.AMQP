using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
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
    namespace Given_a_rabbitmq_client
    {
        public class When_publishing_a_message : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder<MessageSendingApplicationSpecification>>>
        {
            private readonly ConcurrentBag<MethodFrame<Exchange.Declare>> _exchangeDeclare = new ConcurrentBag<MethodFrame<Exchange.Declare>>();
            private readonly ConcurrentBag<MethodFrame<Basic.Publish>> _basicPublish = new ConcurrentBag<MethodFrame<Basic.Publish>>();

            public When_publishing_a_message(ITestOutputHelper output) : base(output)
            {
            }

            private const int NumberOfPublishes = 4;

            protected override string[] StartParameters { get; } = { NumberOfPublishes.ToString() };

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
                _basicPublish.Should().Contain.Any(frame => frame.Message.ContentBody.Deserialize<TestMessage>().Message.Equals("Testing sending a message using RabbitMQ"));
            }

            [Fact]
            public void It_should_have_published_the_message_on_the_correct_exchange()
            {
                _basicPublish.Should().Contain().Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
                _basicPublish.Should().Contain().Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_exchanges()
            {
                _exchangeDeclare.Should().Count.Exactly(4);
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_name()
            {
                _exchangeDeclare.Should().Contain().Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
                _exchangeDeclare.Should().Contain().Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_type()
            {
                _exchangeDeclare.Should().Contain.Any(frame => frame.Message.Type.Equals(Shortstr.From("topic")));
            }
        }

        public class When_consuming_messages : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder<MessageConsumingApplicationSpecification>>>
        {
            private readonly ConcurrentBag<MethodFrame<Exchange.Declare>> _exchangesDeclared = new ConcurrentBag<MethodFrame<Exchange.Declare>>();
            private readonly ConcurrentBag<MethodFrame<Queue.Declare>> _queuesDeclared = new ConcurrentBag<MethodFrame<Queue.Declare>>();
            private readonly ConcurrentBag<MethodFrame<Queue.Bind>> _queuesBound = new ConcurrentBag<MethodFrame<Queue.Bind>>();
            private readonly ConcurrentBag<MethodFrame<Basic.Publish>> _basicPublishes = new ConcurrentBag<MethodFrame<Basic.Publish>>();
            private readonly ConcurrentDictionary<string, MethodFrame<Basic.Consume>> _basicConsumes = new ConcurrentDictionary<string, MethodFrame<Basic.Consume>>();

            public When_consuming_messages(ITestOutputHelper output) : base(output)
            {
            }

            private const int Parallelism = 4;

            protected override string[] StartParameters { get; } = { Parallelism.ToString() };

            protected override TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

            protected override void Given(IServiceContainer container)
            {
                var closedChannels = new ConcurrentBag<short>();
                void TryStop()
                {
                    if (closedChannels.Count == Parallelism && _basicPublishes.Count == Parallelism)
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
                testServer.On<Basic.Consume, Basic.ConsumeOk>((connectionId, frame) =>
                {
                    var consumerTag = Guid.NewGuid().ToString();
                    _basicConsumes.TryAdd(consumerTag, frame);
                    Task.Run(() => testServer.Send<Basic.Publish>(connectionId, new MethodFrame<Basic.Publish>(frame.Channel, new Basic.Publish()
                    {
                        
                    })))
                    return new Basic.ConsumeOk { ConsumerTag = ConsumerTag.From(consumerTag) };
                });
                testServer.On<Basic.Cancel, Basic.CancelOk>((connectionId, frame) =>
                {
                    _basicConsumes.TryRemove(frame.Message.ConsumerTag.Value, out _);
                    return new Basic.CancelOk { ConsumerTag = frame.Message.ConsumerTag };
                });

                container.RegisterSingleton(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
            }

            [Fact]
            public void It_should_have_published_messages()
            {
                _basicPublishes.Should().Count.Exactly(4);
            }

            [Fact]
            public void It_should_have_published_the_correct_message_type()
            {
                _basicPublishes.Should().Contain().Four(frame =>
                    frame.Message.ContentHeader.Type.Equals(Shortstr.From(typeof(TestMessage).FullName)));
            }

            [Fact]
            public void It_should_have_published_the_correct_message()
            {
                _basicPublishes.Should().Contain.Any(frame => frame.Message.ContentBody.Deserialize<TestMessage>().Message.Equals("Testing sending a message using RabbitMQ"));
            }

            [Fact]
            public void It_should_have_published_the_message_on_the_correct_exchange()
            {
                _basicPublishes.Should().Contain().Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
                _basicPublishes.Should().Contain().Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_exchanges()
            {
                _exchangesDeclared.Should().Count.Exactly(4);
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_name()
            {
                _exchangesDeclared.Should().Contain().Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange0")));
                _exchangesDeclared.Should().Contain().Two(frame => frame.Message.Exchange.Equals(ExchangeName.From("myExchange1")));
            }

            [Fact]
            public void It_should_have_declared_an_exchange_with_type()
            {
                _exchangesDeclared.Should().Contain.Any(frame => frame.Message.Type.Equals(Shortstr.From("topic")));
            }
        }

        public class When_sending_and_receiving_heartbeats : XUnitWindowsServiceSpecification<DefaultWindowsServiceHostStarter<TestApplicationBuilder<HeartbeatApplicationSpecification>>>
        {
            private readonly ConcurrentBag<HeartbeatFrame<Heartbeat>> _heartbeats = new ConcurrentBag<HeartbeatFrame<Heartbeat>>();
            private CancellationTokenSource _heartbeatCancelationToken = new CancellationTokenSource();
            private bool _missingHeartbeat;

            public When_sending_and_receiving_heartbeats(ITestOutputHelper output) : base(output)
            {
            }

            protected override TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

            protected override void Given(IServiceContainer container)
            {
                var testServer = new AmqpTestFramework(ProtocolVersion.AMQP091);

                testServer
                    .WithDefaultProtocolHeaderNegotiation()
                    .WithDefaultSecurityNegotiation(heartbeatInterval: TimeSpan.FromSeconds(1))
                    .WithDefaultConnectionOpenNegotiation()
                    .WithHeartbeats(interval: TimeSpan.FromSeconds(1))
                    .WithDefaultConnectionCloseNegotiation();

                testServer.On<Heartbeat>((connectionId, frame) =>
                {
                    _heartbeatCancelationToken.Cancel(true);
                    _heartbeats.Add(frame);
                    _heartbeatCancelationToken = new CancellationTokenSource();
                    Task.Delay(4000)
                        .ContinueWith(task =>
                        {
                            _missingHeartbeat = true;
                            ServiceController.Stop();
                        }, _heartbeatCancelationToken.Token);
                });

                Task.Delay(TimeSpan.FromSeconds(10)).ContinueWith(task =>
                {
                    ServiceController.Stop();
                });

                container.RegisterSingleton(() => testServer.ConnectionFactory.ToRabbitMqConnectionFactory());
            }

            [Fact]
            public void It_should_have_received_heartbeats()
            {
                _heartbeats.Should().Count.AtLeast(1);
            }

            [Fact] // todo: Doubtful quality of test. It's impossible to test that something does not happen. What is the purpose?
            public void It_should_not_stop_receiving_heartbeats()
            {
                _missingHeartbeat.Should().Be.False();
            }
        }
    }

    // todo: add test that sends content
}
