using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.Impl;
using Test.It.With.Amqp.NetworkClient;

namespace Test.It.With.RabbitMQ._091
{
    internal class TestConnectionFactory : IConnectionFactory
    {
        private readonly INetworkClientFactory _networkClientFactory;
        private readonly ConnectionFactory _realConnectionFactory;

        public TestConnectionFactory(INetworkClientFactory networkClientFactory)
        {
            _networkClientFactory = networkClientFactory;

            _realConnectionFactory = new ConnectionFactory();
            Password = _realConnectionFactory.Password;
            RequestedChannelMax = _realConnectionFactory.RequestedChannelMax;
            RequestedFrameMax = _realConnectionFactory.RequestedFrameMax;
            RequestedHeartbeat = _realConnectionFactory.RequestedHeartbeat;
            UseBackgroundThreadsForIO = _realConnectionFactory.UseBackgroundThreadsForIO;
            UserName = _realConnectionFactory.UserName;
            VirtualHost = _realConnectionFactory.VirtualHost;
            HandshakeContinuationTimeout = _realConnectionFactory.HandshakeContinuationTimeout;
            ContinuationTimeout = _realConnectionFactory.ContinuationTimeout;
        }

        public Uri Uri
        {
            get => _realConnectionFactory.Uri;
            set => _realConnectionFactory.Uri = value;
        }

        public AuthMechanismFactory AuthMechanismFactory(IList<string> mechanismNames)
        {
            return _realConnectionFactory.AuthMechanismFactory(mechanismNames);
        }

        public IConnection CreateConnection()
        {
            return CreateConnection(new List<string>());
        }

        public IConnection CreateConnection(string clientProvidedName)
        {
            return CreateConnection(new List<string>());
        }

        public IConnection CreateConnection(IList<string> hostnames)
        {
            return CreateConnection(hostnames, null);
        }

        public IConnection CreateConnection(IList<string> hostnames, string clientProvidedName)
        {
            return new Connection(this, false, new TestFrameHandler(_networkClientFactory.Create()),
                clientProvidedName);
        }

        public IConnection CreateConnection(IList<AmqpTcpEndpoint> endpoints)
        {
            return CreateConnection(new List<string>());
        }

        public IDictionary<string, object> ClientProperties { get; set; } = Connection.DefaultClientProperties();
        public string Password { get; set; }
        public ushort RequestedChannelMax { get; set; }
        public uint RequestedFrameMax { get; set; }
        public ushort RequestedHeartbeat { get; set; }
        public bool UseBackgroundThreadsForIO { get; set; }
        public string UserName { get; set; }
        public string VirtualHost { get; set; }
        [Obsolete]
        public TaskScheduler TaskScheduler { get; set; }
        public TimeSpan HandshakeContinuationTimeout { get; set; }
        public TimeSpan ContinuationTimeout { get; set; }
    }
}