using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.Impl;
using Test.It.NetworkClient;

namespace Test.It.With.RabbitMQ
{
    internal class TestConnectionFactoryDecorator : ConnectionFactoryBase, IConnectionFactory
    {
        private IConnectionFactory ConnectionFactory => _lazyConnectionFactory.Value;
        private readonly Lazy<IConnectionFactory> _lazyConnectionFactory;
        private readonly INetworkClientFactory _networkClientFactory;

        public TestConnectionFactoryDecorator(Lazy<IConnectionFactory> lazyConnectionFactory, INetworkClientFactory networkClientFactory)
        {
            _lazyConnectionFactory = lazyConnectionFactory;
            _networkClientFactory = networkClientFactory;
        }

        public AuthMechanismFactory AuthMechanismFactory(IList<string> mechanismNames)
        {
            return ConnectionFactory.AuthMechanismFactory(mechanismNames);
        }

        public IConnection CreateConnection()
        {
            return CreateConnection(new List<string>());
        }

        public IConnection CreateConnection(string clientProvidedName)
        {
            return CreateConnection(new List<string>(), clientProvidedName);
        }

        public IConnection CreateConnection(IList<string> hostnames)
        {
            return CreateConnection(hostnames, "TestProvider");
        }

        public IConnection CreateConnection(IList<string> hostnames, string clientProvidedName)
        {
            return new Connection(this, false, new TestFrameHandler(_networkClientFactory.Create()), clientProvidedName);
        }

        public IDictionary<string, object> ClientProperties
        {
            get => ConnectionFactory.ClientProperties;
            set => ConnectionFactory.ClientProperties = value;
        }

        public string Password
        {
            get => ConnectionFactory.Password;
            set => ConnectionFactory.Password = value;
        }

        public ushort RequestedChannelMax
        {
            get => ConnectionFactory.RequestedChannelMax;
            set => ConnectionFactory.RequestedChannelMax = value;
        }

        public uint RequestedFrameMax
        {
            get => ConnectionFactory.RequestedFrameMax;
            set => ConnectionFactory.RequestedFrameMax = value;
        }

        public ushort RequestedHeartbeat
        {
            get => ConnectionFactory.RequestedHeartbeat;
            set => ConnectionFactory.RequestedHeartbeat = value;
        }

        public bool UseBackgroundThreadsForIO
        {
            get => ConnectionFactory.UseBackgroundThreadsForIO;
            set => ConnectionFactory.UseBackgroundThreadsForIO = value;
        }

        public string UserName
        {
            get => ConnectionFactory.UserName;
            set => ConnectionFactory.UserName = value;
        }

        public string VirtualHost
        {
            get => ConnectionFactory.VirtualHost;
            set => ConnectionFactory.VirtualHost = value;
        }

        public TaskScheduler TaskScheduler
        {
            get => ConnectionFactory.TaskScheduler;
            set => ConnectionFactory.TaskScheduler = value;
        }

        public TimeSpan HandshakeContinuationTimeout
        {
            get => ConnectionFactory.HandshakeContinuationTimeout;
            set => ConnectionFactory.HandshakeContinuationTimeout = value;
        }

        public TimeSpan ContinuationTimeout
        {
            get => ConnectionFactory.ContinuationTimeout;
            set => ConnectionFactory.ContinuationTimeout = value;
        }
    }
}