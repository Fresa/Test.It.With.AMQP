using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Subscriptions;
using Test.It.With.Amqp.System;

namespace Test.It.With.Amqp
{
    public abstract class AmqpTestFramework : IAsyncDisposable
    {
        public static InMemoryAmqpTestFramework InMemory(
            IProtocolResolver protocolResolver)
        {
            return new InMemoryAmqpTestFramework(protocolResolver);
        }

        public static InMemoryAmqpTestFramework InMemory(
            IProtocolResolver protocolResolver,
            IConfiguration configuration)
        {
            return new InMemoryAmqpTestFramework(protocolResolver, configuration);
        }

        public static SocketAmqpTestFramework WithSocket(
            IProtocolResolver protocolResolver)
        {
            return new SocketAmqpTestFramework(protocolResolver);
        }
        
        public static SocketAmqpTestFramework WithSocket(
            IProtocolResolver protocolResolver,
            INetworkConfiguration configuration)
        {
            return new SocketAmqpTestFramework(protocolResolver, configuration);
        }

        protected readonly List<IAsyncDisposable> AsyncDisposables = new List<IAsyncDisposable>();

        private readonly ConcurrentDictionary<Type, IMethodSubscription> _methodSubscriptionCollections = new ConcurrentDictionary<Type, IMethodSubscription>();
        private readonly ConcurrentDictionary<Type, IProtocolHeaderSubscription> _protocolHeaderSubscriptions = new ConcurrentDictionary<Type, IProtocolHeaderSubscription>();
        private readonly ConcurrentDictionary<Type, IHeartbeatSubscription> _heartbeatSubscriptions = new ConcurrentDictionary<Type, IHeartbeatSubscription>();

        private readonly ConcurrentDictionary<ConnectionId, AmqpConnectionSession> _sessions = new ConcurrentDictionary<ConnectionId, AmqpConnectionSession>();

        internal IDisposable AddSession(AmqpConnectionSession session)
        {
            var connectionId = session.ConnectionId;
            if (_sessions.TryAdd(connectionId, session) == false)
            {
                throw new NotSupportedException($"Client with id {connectionId} has already been registered.");
            }

            lock (_methodSubscriptionCollections)
            {
                foreach (var methodSubscription in _methodSubscriptionCollections)
                {
                    session.On(methodSubscription.Key, frame => methodSubscription.Value.Handle(connectionId, frame));
                }
            }

            lock (_protocolHeaderSubscriptions)
            {
                foreach (var protocolHeaderSubscription in _protocolHeaderSubscriptions)
                {
                    session.On(protocolHeaderSubscription.Key, frame => protocolHeaderSubscription.Value.Handle(connectionId, frame));
                }
            }

            lock (_heartbeatSubscriptions)
            {
                foreach (var heartbeatSubscription in _heartbeatSubscriptions)
                {
                    session.On(heartbeatSubscription.Key, frame => heartbeatSubscription.Value.Handle(connectionId, frame));
                }
            }

            return new DisposableActions(() => _sessions.TryRemove(connectionId, out _));
        }

        public AmqpTestFramework Send<TMessage>(ConnectionId connectionId, MethodFrame<TMessage> frame) where TMessage : class, INonContentMethod, IServerMethod
        {
            lock (_sessions[connectionId])
            {
                _sessions[connectionId].Send(new MethodFrame(frame.Channel, frame.Message));
            }
            return this;
        }

        public AmqpTestFramework Send<TMessage, THeader>(ConnectionId connectionId, MethodFrame<TMessage> frame)
            where TMessage : class, IContentMethod<THeader>, IServerMethod
            where THeader : IContentHeader
        {
            lock (_sessions[connectionId])
            {
                _sessions[connectionId].Send(new MethodFrame(frame.Channel, frame.Message));
                _sessions[connectionId].Send(new ContentHeaderFrame(frame.Channel, frame.Message.ContentHeader));
                foreach (var contentBodyFragment in frame.Message.ContentBodyFragments)
                {
                    _sessions[connectionId].Send(new ContentBodyFrame(frame.Channel, contentBodyFragment));
                }
            }
            return this;
        }

        public AmqpTestFramework Send<TMessage>(ConnectionId connectionId, HeartbeatFrame<TMessage> frame) where TMessage : class, IHeartbeat
        {
            lock (_sessions[connectionId])
            {
                _sessions[connectionId].Send(new HeartbeatFrame(frame.Channel, frame.Message));
            }
            return this;
        }

        public AmqpTestFramework On<TReceivingMethod>(Action<ConnectionId, MethodFrame<TReceivingMethod>> handleFrame)
            where TReceivingMethod : class, IClientMethod
        {
            void HandleFrame(ConnectionId connectionId, MethodFrame frame)
            {
                handleFrame(connectionId,
                    new MethodFrame<TReceivingMethod>(frame.Channel, (TReceivingMethod)frame.Message));
            }

            IMethodSubscription AddSubscription(Type type)
            {
                var subscription = new MethodSubscription<TReceivingMethod>()
                    .Add(HandleFrame);

                foreach (var session in _sessions)
                {
                    session.Value.On(typeof(TReceivingMethod),
                        frame => subscription.Handle(session.Key, frame));
                }

                return subscription;
            }

            IMethodSubscription UpdateSubscription(Type type, IMethodSubscription subscription)
            {
                return subscription.Add(HandleFrame);
            }

            lock (_methodSubscriptionCollections)
            {
                _methodSubscriptionCollections.AddOrUpdate(
                    typeof(TReceivingMethod), AddSubscription, UpdateSubscription);
            }

            return this;
        }

        public AmqpTestFramework On<TReceivingMethod, TSendingMethod>(Func<ConnectionId, MethodFrame<TReceivingMethod>, TSendingMethod> messageHandler)
            where TReceivingMethod : class, IClientMethod, IRespond<TSendingMethod>
            where TSendingMethod : class, IServerMethod, INonContentMethod
        {
            On<TReceivingMethod>((clientId, frame) =>
            {
                var response = messageHandler(clientId, frame);
                Send(clientId, new MethodFrame<TSendingMethod>(frame.Channel, response));
            });

            return this;
        }

        public AmqpTestFramework On<TReceivingMethod, TSendingMethod, THeader>(Func<ConnectionId, MethodFrame<TReceivingMethod>, TSendingMethod> messageHandler)
            where TReceivingMethod : class, IClientMethod, IRespond<TSendingMethod>
            where TSendingMethod : class, IServerMethod, IContentMethod<THeader>
            where THeader : class, IContentHeader
        {
            On<TReceivingMethod>((clientId, frame) =>
            {
                var response = messageHandler(clientId, frame);
                Send<TSendingMethod, THeader>(clientId, new MethodFrame<TSendingMethod>(frame.Channel, response));
            });

            return this;
        }

        public AmqpTestFramework On<TReceivingProtocolHeader>(Action<ConnectionId, ProtocolHeaderFrame<TReceivingProtocolHeader>> handleFrame)
            where TReceivingProtocolHeader : class, IProtocolHeader
        {
            void HandleFrame(ConnectionId connectionId, ProtocolHeaderFrame frame)
            {
                handleFrame(connectionId,
                    new ProtocolHeaderFrame<TReceivingProtocolHeader>(frame.Channel, (TReceivingProtocolHeader)frame.Message));
            }

            IProtocolHeaderSubscription AddSubscription(Type type)
            {
                var subscription = new ProtocolHeaderSubscription<TReceivingProtocolHeader>()
                    .Add(HandleFrame);

                foreach (var session in _sessions)
                {
                    session.Value.On(typeof(TReceivingProtocolHeader), 
                        frame => subscription.Handle(session.Key, frame));
                }

                return subscription;
            }

            IProtocolHeaderSubscription UpdateSubscription(Type type, IProtocolHeaderSubscription subscription)
            {
                return subscription.Add(HandleFrame);
            }

            lock (_protocolHeaderSubscriptions)
            {
                _protocolHeaderSubscriptions.AddOrUpdate(
                    typeof(TReceivingProtocolHeader), AddSubscription, UpdateSubscription);
            }

            return this;
        }

        public AmqpTestFramework On<TReceivingHeartbeat>(Action<ConnectionId, HeartbeatFrame<TReceivingHeartbeat>> handleFrame)
            where TReceivingHeartbeat : class, IHeartbeat
        {
            void HandleFrame(ConnectionId connectionId, HeartbeatFrame frame)
            {
                handleFrame(connectionId,
                    new HeartbeatFrame<TReceivingHeartbeat>(frame.Channel, (TReceivingHeartbeat)frame.Message));
            }

            IHeartbeatSubscription AddSubscription(Type type)
            {
                var subscription = new HeartbeatSubscription<TReceivingHeartbeat>()
                    .Add(HandleFrame);

                foreach (var connection in _sessions)
                {
                    connection.Value.On(typeof(TReceivingHeartbeat), 
                        frame => subscription.Handle(connection.Key, frame));
                }

                return subscription;
            }

            IHeartbeatSubscription UpdateSubscription(Type type, IHeartbeatSubscription subscription)
            {
                return subscription.Add(HandleFrame);
            }

            lock (_heartbeatSubscriptions)
            {
                _heartbeatSubscriptions.AddOrUpdate(
                    typeof(TReceivingHeartbeat), AddSubscription, UpdateSubscription);
            }

            return this;
        }
        
        public ValueTask DisposeAsync()
        {
            return new ValueTask(AsyncDisposables.DisposeAllAsync());
        }
    }
}