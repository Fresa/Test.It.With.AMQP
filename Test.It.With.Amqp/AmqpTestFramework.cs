using System;
using System.Collections.Concurrent;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;

namespace Test.It.With.Amqp
{
    public class AmqpTestFramework : IDisposable
    {
        private readonly ConcurrentBag<IDisposable> _disposables = new ConcurrentBag<IDisposable>();

        private readonly ConcurrentDictionary<Type, Action<ConnectionId, MethodFrame>> _methodSubscriptions = new ConcurrentDictionary<Type, Action<ConnectionId, MethodFrame>>();
        private readonly ConcurrentDictionary<Type, Action<ConnectionId, ProtocolHeaderFrame>> _protocolHeaderSubscriptions = new ConcurrentDictionary<Type, Action<ConnectionId, ProtocolHeaderFrame>>();
        private readonly ConcurrentDictionary<Type, Action<ConnectionId, HeartbeatFrame>> _heartbeatSubscriptions = new ConcurrentDictionary<Type, Action<ConnectionId, HeartbeatFrame>>();

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

                lock (_methodSubscriptions)
                {
                    foreach (var methodSubscription in _methodSubscriptions)
                    {
                        server.On(methodSubscription.Key, frame => methodSubscription.Value(connectionId, frame));
                    }
                }

                lock (_protocolHeaderSubscriptions)
                {
                    foreach (var protocolHeaderSubscription in _protocolHeaderSubscriptions)
                    {
                        server.On(protocolHeaderSubscription.Key, frame => protocolHeaderSubscription.Value(connectionId, frame));
                    }
                }

                lock (_heartbeatSubscriptions)
                {
                    foreach (var heartbeatSubscription in _heartbeatSubscriptions)
                    {
                        server.On(heartbeatSubscription.Key, frame => heartbeatSubscription.Value(connectionId, frame));
                    }
                }
            });
            ConnectionFactory = networkClientFactory;
        }

        public INetworkClientFactory ConnectionFactory { get; }

        public void Send<TMessage>(ConnectionId connectionId, MethodFrame<TMessage> frame) where TMessage : class, IServerMethod
        {
            _connections[connectionId].Send(new MethodFrame(frame.Channel, frame.Method));
        }

        public void On<TClientMethod>(Action<ConnectionId, MethodFrame<TClientMethod>> messageHandler)
            where TClientMethod : class, IClientMethod
        {
            void FrameHandler(ConnectionId connectionId, MethodFrame frame)
            {
                messageHandler(connectionId,
                    new MethodFrame<TClientMethod>(frame.Channel, (TClientMethod)frame.Method));
            }

            lock (_methodSubscriptions)
            {
                foreach (var connection in _connections)
                {
                    connection.Value.On(typeof(TClientMethod), frame => FrameHandler(connection.Key, frame));
                }

                if (_methodSubscriptions.TryAdd(typeof(TClientMethod), FrameHandler) == false)
                {
                    ThrowDuplicateSubscriptionException<TClientMethod>();
                }
            }
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
            void FrameHandler(ConnectionId connectionId, ProtocolHeaderFrame frame)
            {
                messageHandler(connectionId,
                    new ProtocolHeaderFrame<TProtocolHeader>(frame.Channel, (TProtocolHeader)frame.ProtocolHeader));
            }

            lock (_protocolHeaderSubscriptions)
            {
                foreach (var connection in _connections)
                {
                    connection.Value.On(typeof(TProtocolHeader), frame => FrameHandler(connection.Key, frame));
                }

                if (_protocolHeaderSubscriptions.TryAdd(typeof(TProtocolHeader), FrameHandler) == false)
                {
                    ThrowDuplicateSubscriptionException<TProtocolHeader>();
                }
            }
        }
        
        public void On<THeartbeat>(Action<ConnectionId, HeartbeatFrame<THeartbeat>> messageHandler)
            where THeartbeat : class, IHeartbeat
        {
            void FrameHandler(ConnectionId connectionId, HeartbeatFrame frame)
            {
                messageHandler(connectionId,
                    new HeartbeatFrame<THeartbeat>(frame.Channel, (THeartbeat)frame.Heartbeat));
            }

            lock (_heartbeatSubscriptions)
            {
                foreach (var connection in _connections)
                {
                    connection.Value.On(typeof(THeartbeat), frame => FrameHandler(connection.Key, frame));
                }

                if (_heartbeatSubscriptions.TryAdd(typeof(THeartbeat), FrameHandler) == false)
                {
                    ThrowDuplicateSubscriptionException<THeartbeat>();
                }
            }
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        private static void ThrowDuplicateSubscriptionException<TSubscription>()
        {
            throw new InvalidOperationException($"There is already a subscription on {typeof(TSubscription).GetPrettyFullName()}. There can only be one subscription per method type.");
        }
    }
}