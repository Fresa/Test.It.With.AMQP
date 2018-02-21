using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Should.Fluent;
using Test.It.While.Hosting.Your.Windows.Service;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol._091;
using Test.It.With.RabbitMQ.Tests.FrameworkExtensions;
using Test.It.With.RabbitMQ.Tests.TestApplication;
using Test.It.With.RabbitMQ.Tests.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests
{
    namespace Given_a_client_application_sending_and_receiving_heartbeats_over_rabbitmq
    {
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
}
