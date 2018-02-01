using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol._091;
using Test.It.With.RabbitMQ.Tests.Common;

namespace Test.It.With.RabbitMQ.Tests.FrameworkExtensions
{
    public static class AmqpTestFrameworkExtensions
    {
        public const short DefaultHeartbeatIntervalInSeconds = 1;

        public static AmqpTestFramework WithDefaultProtocolHeaderNegotiation(this AmqpTestFramework testFramework)
        {
            return testFramework
                .On<ProtocolHeader>((connectionId, frame) => testFramework.Send(connectionId,
                    new MethodFrame<Connection.Start>(frame.Channel, new Connection.Start
                    {
                        VersionMajor = Octet.From((byte)frame.Message.Version.Major),
                        VersionMinor = Octet.From((byte)frame.Message.Version.Minor),
                        Locales = Longstr.From(Encoding.UTF8.GetBytes("en_US")),
                        Mechanisms = Longstr.From(Encoding.UTF8.GetBytes("PLAIN")),
                    })));
        }

        public static AmqpTestFramework WithDefaultSecurityNegotiation(this AmqpTestFramework testFramework, TimeSpan heartbeatInterval = default)
        {
            heartbeatInterval = heartbeatInterval == default ? TimeSpan.FromSeconds(DefaultHeartbeatIntervalInSeconds) : heartbeatInterval;
            return testFramework.On<Connection.StartOk>((connectionId, frame) =>
                    testFramework.Send(connectionId, new MethodFrame<Connection.Secure>(frame.Channel,
                        new Connection.Secure
                        {
                            Challenge = Longstr.From(Encoding.UTF8.GetBytes("challenge"))
                        })))
                .On<Connection.SecureOk>((connectionId, frame) => testFramework.Send(connectionId,
                    new MethodFrame<Connection.Tune>(frame.Channel, new Connection.Tune
                    {
                        ChannelMax = Short.From(0),
                        FrameMax = Long.From(0),
                        Heartbeat = Short.From((short)(heartbeatInterval.TotalMilliseconds / 1000))
                    })));
        }

        public static AmqpTestFramework WithHeartbeats(this AmqpTestFramework testFramework, TimeSpan interval = default)
        {
            interval = interval == default ? TimeSpan.FromSeconds(DefaultHeartbeatIntervalInSeconds) : interval;
            IDisposable heartbeatRunner = null;
            return testFramework.On<Connection.TuneOk>((connectionId, frame) =>
                {
                    heartbeatRunner = testFramework.StartSendingHeartbeats(connectionId, interval);
                })
                .On<Connection.Close>((connectionId, frame) =>
                {
                    heartbeatRunner?.Dispose();
                })
                .On<Connection.CloseOk>((connectionId, frame) =>
                {
                    heartbeatRunner?.Dispose();
                });
        }

        public static AmqpTestFramework WithDefaultConnectionOpenNegotiation(this AmqpTestFramework testFramework)
        {
            return testFramework.On<Connection.Open, Connection.OpenOk>((connectionId, frame) =>
                new Connection.OpenOk());
        }

        public static AmqpTestFramework WithDefaultConnectionCloseNegotiation(this AmqpTestFramework testFramework)
        {
            return testFramework.On<Connection.Close, Connection.CloseOk>((connectionId, frame) => new Connection.CloseOk());
        }

        public static IDisposable StartSendingHeartbeats(this AmqpTestFramework testFramework, ConnectionId connectionId, TimeSpan interval)
        {
            var cancelationTokenSource = new CancellationTokenSource();
            void SendHeartbeat()
            {
                testFramework
                    .Send(connectionId, new HeartbeatFrame<Heartbeat>(0, new Heartbeat()));
            }

            void Schedule()
            {
                try
                {
                    Task.Delay(interval, cancelationTokenSource.Token)
                        .ContinueWith(task => SendHeartbeat(), cancelationTokenSource.Token)
                        .ContinueWith(task => Schedule(), cancelationTokenSource.Token);
                }
                catch (OperationCanceledException) { }
            }
            Schedule();

            return new Disposable(() => cancelationTokenSource.Cancel());
        }
    }
}