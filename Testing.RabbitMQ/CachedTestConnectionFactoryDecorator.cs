using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Test.It.With.RabbitMQ
{
    internal class CachedTestConnectionFactoryDecorator : IConnectionFactory
    {
        private readonly ConcurrentDictionary<Guid, CachedConnectionDecorator> _connections = new ConcurrentDictionary<Guid, CachedConnectionDecorator>();
        private readonly IConnectionFactory _connectionFactory;

        public CachedTestConnectionFactoryDecorator(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public AuthMechanismFactory AuthMechanismFactory(IList<string> mechanismNames)
        {
            return _connectionFactory.AuthMechanismFactory(mechanismNames);
        }

        public IConnection CreateConnection()
        {
            return Store(new CachedConnectionDecorator(_connectionFactory.CreateConnection()));
        }

        public IConnection CreateConnection(string clientProvidedName)
        {
            return Store(new CachedConnectionDecorator(_connectionFactory.CreateConnection(clientProvidedName)));
        }

        public IConnection CreateConnection(IList<string> hostnames)
        {
            return Store(new CachedConnectionDecorator(_connectionFactory.CreateConnection(hostnames)));
        }

        public IConnection CreateConnection(IList<string> hostnames, string clientProvidedName)
        {
            return Store(new CachedConnectionDecorator(_connectionFactory.CreateConnection(hostnames, clientProvidedName)));
        }

        public IDictionary<string, object> ClientProperties
        {
            get => _connectionFactory.ClientProperties;
            set => _connectionFactory.ClientProperties = value;
        }

        public string Password
        {
            get => _connectionFactory.Password;
            set => _connectionFactory.Password = value;
        }

        public ushort RequestedChannelMax
        {
            get => _connectionFactory.RequestedChannelMax;
            set => _connectionFactory.RequestedChannelMax = value;
        }

        public uint RequestedFrameMax
        {
            get => _connectionFactory.RequestedFrameMax;
            set => _connectionFactory.RequestedFrameMax = value;
        }

        public ushort RequestedHeartbeat
        {
            get => _connectionFactory.RequestedHeartbeat;
            set => _connectionFactory.RequestedHeartbeat = value;
        }

        public bool UseBackgroundThreadsForIO
        {
            get => _connectionFactory.UseBackgroundThreadsForIO;
            set => _connectionFactory.UseBackgroundThreadsForIO = value;
        }

        public string UserName
        {
            get => _connectionFactory.UserName;
            set => _connectionFactory.UserName = value;
        }

        public string VirtualHost
        {
            get => _connectionFactory.VirtualHost;
            set => _connectionFactory.VirtualHost = value;
        }

        public TaskScheduler TaskScheduler
        {
            get => _connectionFactory.TaskScheduler;
            set => _connectionFactory.TaskScheduler = value;
        }

        public TimeSpan HandshakeContinuationTimeout
        {
            get => _connectionFactory.HandshakeContinuationTimeout;
            set => _connectionFactory.HandshakeContinuationTimeout = value;
        }

        public TimeSpan ContinuationTimeout
        {
            get => _connectionFactory.ContinuationTimeout;
            set => _connectionFactory.ContinuationTimeout = value;
        }

        public IReadOnlyList<CachedConnectionDecorator> CurrentConnections => _connections.Select(pair => pair.Value).ToList();

        private IConnection Store(CachedConnectionDecorator connection)
        {
            var connectionId = Guid.NewGuid();
            _connections.AddOrUpdate(connectionId, connection, (storedConnectionId, storedConnection) => storedConnection);
            connection.OnDispose(() => _connections.TryRemove(connectionId, out var _));
            return connection;
        }
    }
}