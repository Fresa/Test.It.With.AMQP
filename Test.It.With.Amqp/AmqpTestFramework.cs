using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp
{
    public class AmqpTestFramework : IDisposable
    {
        private readonly ConcurrentBag<IDisposable> _disposables = new ConcurrentBag<IDisposable>();

        private readonly ConcurrentDictionary<Type, IMethodSubscription> _methodSubscriptionCollections = new ConcurrentDictionary<Type, IMethodSubscription>();

        //private readonly ConcurrentBag<MethodSubscription> _methodSubscriptions = new ConcurrentBag<MethodSubscription>();
        private readonly ConcurrentBag<ProtocolHeaderSubscription> _protocolHeaderSubscriptions = new ConcurrentBag<ProtocolHeaderSubscription>();
        private readonly ConcurrentBag<HeartbeatSubscription> _heartbeatSubscriptions = new ConcurrentBag<HeartbeatSubscription>();

        private readonly ConcurrentDictionary<ConnectionId, AmqpConnectionSession> _sessions = new ConcurrentDictionary<ConnectionId, AmqpConnectionSession>();

        public AmqpTestFramework(ProtocolVersion protocolVersion)
        {
            var networkClientFactory = new NetworkClientFactory(protocolVersion);
            networkClientFactory.OnNetworkClientCreated(session =>
            {
                _disposables.Add(session);

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
                        session.On(protocolHeaderSubscription.Id, frame => protocolHeaderSubscription.Subscription(connectionId, frame));
                    }
                }

                lock (_heartbeatSubscriptions)
                {
                    foreach (var heartbeatSubscription in _heartbeatSubscriptions)
                    {
                        session.On(heartbeatSubscription.Id, frame => heartbeatSubscription.Subscription(connectionId, frame));
                    }
                }
            });
            ConnectionFactory = networkClientFactory;
        }

        public INetworkClientFactory ConnectionFactory { get; }

        public AmqpTestFramework Send<TMessage>(ConnectionId connectionId, MethodFrame<TMessage> frame) where TMessage : class, INonContentMethod, IServerMethod
        {
            _sessions[connectionId].Send(new MethodFrame(frame.Channel, frame.Message));

            return this;
        }

        public AmqpTestFramework Send<TMessage, THeader>(ConnectionId connectionId, MethodFrame<TMessage> frame)
            where TMessage : class, IContentMethod<THeader>, IServerMethod
            where THeader : IContentHeader
        {
            _sessions[connectionId].Send(new MethodFrame(frame.Channel, frame.Message));
            _sessions[connectionId].Send(new ContentHeaderFrame(frame.Channel, frame.Message.ContentHeader));
            foreach (var contentBodyFragment in frame.Message.ContentBodyFragments)
            {
                _sessions[connectionId].Send(new ContentBodyFrame(frame.Channel, contentBodyFragment));
            }

            return this;
        }

        public AmqpTestFramework Send<TMessage>(ConnectionId connectionId, HeartbeatFrame<TMessage> frame) where TMessage : class, IHeartbeat
        {
            _sessions[connectionId].Send(new HeartbeatFrame(frame.Channel, frame.Message));

            return this;
        }

        public AmqpTestFramework On<TReceivingMethod>(Action<ConnectionId, MethodFrame<TReceivingMethod>> messageHandler)
            where TReceivingMethod : class, IClientMethod
        {
            void FrameHandler(ConnectionId connectionId, MethodFrame frame)
            {
                messageHandler(connectionId,
                    new MethodFrame<TReceivingMethod>(frame.Channel, (TReceivingMethod)frame.Message));
            }

            lock (_methodSubscriptionCollections)
            {
                _methodSubscriptionCollections.AddOrUpdate(
                    typeof(TReceivingMethod), type =>
                    {
                        var subscription = new MethodSubscription<TReceivingMethod>().Add(FrameHandler);
                        foreach (var connection in _sessions)
                        {
                            connection.Value.On(typeof(TReceivingMethod), frame => subscription.Handle(connection.Key, frame));
                        }
                        return subscription;
                    },
                    (type, collection) => collection.Add(FrameHandler));

                //foreach (var connection in _sessions)
                //{
                //    connection.Value.On(typeof(TReceivingMethod), frame => FrameHandler(connection.Key, frame));
                //}

                //_methodSubscriptions.Add(MethodSubscription.Create<TReceivingMethod>(FrameHandler));
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

        public AmqpTestFramework On<TReceivingProtocolHeader>(Action<ConnectionId, ProtocolHeaderFrame<TReceivingProtocolHeader>> messageHandler)
            where TReceivingProtocolHeader : class, IProtocolHeader
        {
            void FrameHandler(ConnectionId connectionId, ProtocolHeaderFrame frame)
            {
                messageHandler(connectionId,
                    new ProtocolHeaderFrame<TReceivingProtocolHeader>(frame.Channel, (TReceivingProtocolHeader)frame.Message));
            }

            lock (_protocolHeaderSubscriptions)
            {
                foreach (var connection in _sessions)
                {
                    connection.Value.On(typeof(TReceivingProtocolHeader), frame => FrameHandler(connection.Key, frame));
                }

                _protocolHeaderSubscriptions.Add(
                    ProtocolHeaderSubscription.Create<TReceivingProtocolHeader>(FrameHandler));

            }

            return this;
        }

        public AmqpTestFramework On<TReceivingHeartbeat>(Action<ConnectionId, HeartbeatFrame<TReceivingHeartbeat>> messageHandler)
            where TReceivingHeartbeat : class, IHeartbeat
        {
            void FrameHandler(ConnectionId connectionId, HeartbeatFrame frame)
            {
                messageHandler(connectionId,
                    new HeartbeatFrame<TReceivingHeartbeat>(frame.Channel, (TReceivingHeartbeat)frame.Message));
            }

            lock (_heartbeatSubscriptions)
            {
                foreach (var connection in _sessions)
                {
                    connection.Value.On(typeof(TReceivingHeartbeat), frame => FrameHandler(connection.Key, frame));
                }

                _heartbeatSubscriptions.Add(HeartbeatSubscription.Create<TReceivingHeartbeat>(FrameHandler));
            }

            return this;
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