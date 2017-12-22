using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;

namespace Test.It.With.Amqp
{
    public class AmqpTestFramework : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly List<Type> _subscribedMethods = new List<Type>();

        private readonly Dictionary<Type, Action<ConnectionId, MethodFrame>> _methodSubscriptions = new Dictionary<Type, Action<ConnectionId, MethodFrame>>();
        private readonly Dictionary<Type, Action<ConnectionId, ProtocolHeaderFrame>> _protocolHeaderSubscriptions = new Dictionary<Type, Action<ConnectionId, ProtocolHeaderFrame>>();
        private readonly Dictionary<Type, Action<ConnectionId, HeartbeatFrame>> _heartbeatSubscriptions = new Dictionary<Type, Action<ConnectionId, HeartbeatFrame>>();

        private readonly ConcurrentDictionary<ConnectionId, AmqpTestServer> _connections = new ConcurrentDictionary<ConnectionId, AmqpTestServer>();

        public AmqpTestFramework(ProtocolVersion protocolVersion)
        {
            var networkClientFactory = new NetworkClientFactory(protocolVersion);
            networkClientFactory.OnNetworkClientCreated(server =>
            {
                _disposables.Add(server);

                var connectionId = new ConnectionId(Guid.NewGuid());
                if (_connections.TryAdd(connectionId, server) == false)
                {
                    throw new NotSupportedException($"Client with id {connectionId} has already been registered.");
                }

                foreach (var methodSubscription in _methodSubscriptions)
                {
                    server.On(methodSubscription.Key, frame => methodSubscription.Value(connectionId, frame));
                }

                foreach (var protocolHeaderSubscription in _protocolHeaderSubscriptions)
                {
                    server.On(protocolHeaderSubscription.Key, frame => protocolHeaderSubscription.Value(connectionId, frame));
                }

                foreach (var heartbeatSubscription in _heartbeatSubscriptions)
                {
                    server.On(heartbeatSubscription.Key, frame => heartbeatSubscription.Value(connectionId, frame));
                }
            });
            ConnectionFactory = networkClientFactory;
        }

        private void AssertNoDuplicateSubscriptions<TMethod>()
        {
            if (_subscribedMethods.Contains(typeof(TMethod)))
            {
                throw new InvalidOperationException($"There is already a subscription on {typeof(TMethod).GetPrettyFullName()}. There can only be one subscription per method type.");
            }

            _subscribedMethods.Add(typeof(TMethod));
        }

        public INetworkClientFactory ConnectionFactory { get; }

        public void Send<TMessage>(ConnectionId connectionId, MethodFrame<TMessage> frame) where TMessage : class, IServerMethod
        {
            _connections[connectionId].Send(new MethodFrame(frame.Channel, frame.Method));
        }

        public void On<TClientMethod>(Action<ConnectionId, MethodFrame<TClientMethod>> messageHandler)
            where TClientMethod : class, IClientMethod
        {
            AssertNoDuplicateSubscriptions<TClientMethod>();

            _methodSubscriptions.Add(typeof(TClientMethod),
                (clientId, method) => messageHandler(clientId,
                    new MethodFrame<TClientMethod>(method.Channel, (TClientMethod)method.Method)));
        }

        public void On<TClientMethod, TServerMethod>(Func<ConnectionId, MethodFrame<TClientMethod>, TServerMethod> messageHandler)
            where TClientMethod : class, IClientMethod, INonContentMethod, IRespond<TServerMethod>
            where TServerMethod : class, IServerMethod
        {
            On<TClientMethod>((clientId, frame) =>
            {
                var response = messageHandler(clientId, frame);
                Send(clientId, new MethodFrame<TServerMethod>(frame.Channel, response));
            });
        }

        public void On<TProtocolHeader>(Action<ConnectionId, ProtocolHeaderFrame<TProtocolHeader>> messageHandler)
            where TProtocolHeader : class, IProtocolHeader
        {
            AssertNoDuplicateSubscriptions<TProtocolHeader>();

            _protocolHeaderSubscriptions.Add(typeof(TProtocolHeader),
                (clientId, method) => messageHandler(clientId,
                    new ProtocolHeaderFrame<TProtocolHeader>(method.Channel, (TProtocolHeader)method.ProtocolHeader)));
        }

        public void On<TProtocolHeader>(Func<ConnectionId, ProtocolHeaderFrame<TProtocolHeader>, Connection.Start> messageHandler)
            where TProtocolHeader : class, IProtocolHeader
        {
            On<TProtocolHeader>((clientId, header) =>
            {
                var response = messageHandler(clientId, header);
                Send(clientId, new MethodFrame<Connection.Start>(0, response));
            });
        }

        public void On<THeartbeat>(Action<ConnectionId, HeartbeatFrame<THeartbeat>> messageHandler)
            where THeartbeat : class, IHeartbeat
        {
            AssertNoDuplicateSubscriptions<THeartbeat>();

            _heartbeatSubscriptions.Add(typeof(THeartbeat),
                (clientId, method) => messageHandler(clientId,
                    new HeartbeatFrame<THeartbeat>(method.Channel, (THeartbeat)method.Heartbeat)));
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}