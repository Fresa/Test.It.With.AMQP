using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Log.It;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Expectations;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.MessageClient;
using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.MessageRouters;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.Amqp.Subscriptions;
using Frame = Test.It.With.Amqp.Protocol.Frame;

namespace Test.It.With.Amqp
{
    internal class NetworkClientFactory : INetworkClientFactory
    {
        private readonly ProtocolVersion _protocolVersion;
        private Action<AmqpConnection> _subscription;

        public NetworkClientFactory(ProtocolVersion protocolVersion)
        {
            _protocolVersion = protocolVersion;
        }

        public void OnNetworkClientCreated(Action<AmqpConnection> subscription)
        {
            _subscription = subscription;
        }

        public INetworkClient Create()
        {
            var test  = new AmqpConnection(_protocolVersion);
            _subscription(test);
            return test.Client;
        }
    }

    public class AmqpTestFramework : IDisposable
    {
        private readonly ILogger _logger = LogFactory.Create<AmqpTestFramework>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly List<Type> _subscribedMethods = new List<Type>();

        private readonly Dictionary<Type, Action<ClientId, MethodFrame>> _methodSubscriptions = new Dictionary<Type, Action<ClientId, MethodFrame>>();
        private readonly Dictionary<Type, Action<ClientId, ProtocolHeaderFrame>> _protocolHeaderSubscriptions = new Dictionary<Type, Action<ClientId, ProtocolHeaderFrame>>();
        private readonly Dictionary<Type, Action<ClientId, HeartbeatFrame>> _heartbeatSubscriptions = new Dictionary<Type, Action<ClientId, HeartbeatFrame>>();
        private readonly ConcurrentDictionary<Guid, AmqpConnection> _connections = new ConcurrentDictionary<Guid, AmqpConnection>();

        public AmqpTestFramework(ProtocolVersion protocolVersion)
        {
            var factory = new NetworkClientFactory(protocolVersion);
            factory.OnNetworkClientCreated(connection =>
            {
                _disposables.Add(connection);
                if (_connections.TryAdd(connection.Client.Id, connection) == false)
                {
                    throw new NotSupportedException();
                }

                foreach (var subscription in _methodSubscriptions)
                {
                    connection.On(subscription.Key, frame => subscription.Value(new ClientId(connection.Client.Id), frame));
                }

                foreach (var subscription in _protocolHeaderSubscriptions)
                {
                    connection.On(subscription.Key, frame => subscription.Value(new ClientId(connection.Client.Id), frame));
                }

                foreach (var subscription in _heartbeatSubscriptions)
                {
                    connection.On(subscription.Key, frame => subscription.Value(new ClientId(connection.Client.Id), frame));
                }
            });
            ConnectionFactory = factory;
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

        public void Send<TMessage>(ClientId clientId, MethodFrame<TMessage> frame) where TMessage : class, IServerMethod
        {
            _connections[clientId.Value].Send(new MethodFrame(frame.Channel, frame.Method));
        }

        public void On<TClientMethod>(Action<ClientId, MethodFrame<TClientMethod>> messageHandler)
            where TClientMethod : class, IClientMethod
        {
            AssertNoDuplicateSubscriptions<TClientMethod>();

            _methodSubscriptions.Add(typeof(TClientMethod),
                (clientId, method) => messageHandler(clientId,
                    new MethodFrame<TClientMethod>(method.Channel, (TClientMethod)method.Method)));
        }

        public void On<TClientMethod, TServerMethod>(Func<ClientId, MethodFrame<TClientMethod>, TServerMethod> messageHandler)
            where TClientMethod : class, IClientMethod, INonContentMethod, IRespond<TServerMethod>
            where TServerMethod : class, IServerMethod
        {
            On<TClientMethod>((clientId, frame) =>
            {
                var response = messageHandler(clientId, frame);
                Send(clientId, new MethodFrame<TServerMethod>(frame.Channel, response));
            });
        }

        public void On<TProtocolHeader>(Action<ClientId, ProtocolHeaderFrame<TProtocolHeader>> messageHandler)
            where TProtocolHeader : class, IProtocolHeader
        {
            AssertNoDuplicateSubscriptions<TProtocolHeader>();

            _protocolHeaderSubscriptions.Add(typeof(TProtocolHeader),
                (clientId, method) => messageHandler(clientId,
                    new ProtocolHeaderFrame<TProtocolHeader>(method.Channel, (TProtocolHeader)method.ProtocolHeader)));
        }

        public void On<TProtocolHeader>(Func<ClientId, ProtocolHeaderFrame<TProtocolHeader>, Connection.Start> messageHandler)
            where TProtocolHeader : class, IProtocolHeader
        {
            On<TProtocolHeader>((clientId, header) =>
            {
                var response = messageHandler(clientId, header);
                Send(clientId, new MethodFrame<Connection.Start>(0, response));
            });
        }

        public void On<THeartbeat>(Action<ClientId, HeartbeatFrame<THeartbeat>> messageHandler)
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

    internal class IdentibleNetworkClientDecorator : INetworkClient
    {
        private readonly INetworkClient _networkClient;

        public IdentibleNetworkClientDecorator(Guid id, INetworkClient networkClient)
        {
            Id = id;
            _networkClient = networkClient;
            _networkClient.BufferReceived += (sender, args) =>
            {
                BufferReceived?.Invoke(sender, args);
            };
            _networkClient.Disconnected += (sender, args) =>
            {
                Disconnected?.Invoke(sender, args);
            };
        }

        public Guid Id { get; } 

        public void Dispose()
        {
            _networkClient.Dispose();
        }

        public event EventHandler<ReceivedEventArgs> BufferReceived;
        public event EventHandler Disconnected;
        public void Send(byte[] buffer, int offset, int count)
        {
            _networkClient.Send(buffer, offset, count);
        }
    }

    internal class AmqpConnection : IDisposable
    {
        private readonly ILogger _logger = LogFactory.Create<AmqpTestFramework>();
        private readonly ITypedMessageClient<Frame, Frame> _frameClient;
        private readonly IPublishProtocolHeader _protocolHeaderPublisher;
        private readonly IPublishMethod _methodFramePublisher;
        private readonly IPublish<ContentHeaderFrame> _contentHeaderFramePublisher;
        private readonly IPublish<ContentBodyFrame> _contentBodyFramePublisher;
        private readonly IPublishHeartbeat _heartbeatFramePublisher;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private readonly IExpectationStateMachine _expectationStateMachine;
        private readonly List<Type> _subscribedMethods = new List<Type>();

        public AmqpConnection(ProtocolVersion protocolVersion)
        {
            var protocolResolver = new ProtocolResolver(protocolVersion);
            _expectationStateMachine = protocolResolver.ExpectationStateMachine;
            var protocol = protocolResolver.Protocol;

            var networkClientFactory = new InternalRoutedNetworkClientFactory(out var serverNetworkClient);
            networkClientFactory.OnException += exception =>
            {
                // todo: Update base class (it sends Fatal on Error)
                _logger.Error(exception, "Test framework error.");
            };
            Client = new IdentibleNetworkClientDecorator(Guid.NewGuid(), networkClientFactory.Create());
            _disposables.Add(networkClientFactory);

            var protocolHeaderHandler = new ProtocolHeaderHandler();
            var methodFrameHandler = new MethodFrameHandler();
            var contentHeaderFrameHandler = new ContentHeaderFrameHandler();
            var contentBodyFrameHandler = new ContentBodyFrameHandler();
            var heartbeatFrameHandler = new HeartbeatFrameHandler();

            var heartbeatFrameRouter = new HeartbeatFrameRouter(null, protocol, heartbeatFrameHandler);
            var contentBodyFrameRouter = new ContentBodyFrameRouter(heartbeatFrameRouter, protocol, contentBodyFrameHandler);
            var contentHeaderFrameRouter = new ContentHeaderFrameRouter(contentBodyFrameRouter, protocol, contentHeaderFrameHandler);
            var methodFrameRouter = new MethodFrameRouter(contentHeaderFrameRouter, protocol, methodFrameHandler);

            var protocolHeaderClient = new ProtocolHeaderClient(serverNetworkClient, protocol);
            protocolHeaderClient.Received += protocolHeaderHandler.Handle;
            var frameClient = new FrameClient(protocolHeaderClient);
            frameClient.Received += methodFrameRouter.Handle;
            _frameClient = frameClient;

            _protocolHeaderPublisher = protocolHeaderHandler;
            _methodFramePublisher = methodFrameHandler;
            _contentHeaderFramePublisher = contentHeaderFrameHandler;
            _contentBodyFramePublisher = contentBodyFrameHandler;
            _heartbeatFramePublisher = heartbeatFrameHandler;
        }

        private void AssertNoDuplicateSubscriptions(Type type)
        {
            if (_subscribedMethods.Contains(type))
            {
                throw new InvalidOperationException($"There is already a subscription on {type.GetPrettyFullName()}. There can only be one subscription per method type.");
            }

            _subscribedMethods.Add(type);
        }


        public IdentibleNetworkClientDecorator Client { get; }

        public void Send(MethodFrame frame)
        {
            _logger.Debug($"Sending method {frame.Method.GetType().GetPrettyFullName()} on channel {frame.Channel}. {frame.Method.Serialize()}");
            _frameClient.Send(new Frame(Constants.FrameMethod, frame.Channel, frame.Method));
        }

        public void On(Type methodType, Action<MethodFrame> messageHandler)
        {
            AssertNoDuplicateSubscriptions(methodType);

            var methodSubscription = _methodFramePublisher.Subscribe(methodType, frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Method))
                {
                    _logger.Debug($"Method {methodType.GetPrettyFullName()} on channel {frame.Channel} was expected.");
                    messageHandler(new MethodFrame(
                        frame.Channel,
                        frame.Method));
                }
            });

            _disposables.Add(methodSubscription);

            var contentHeaderSubscription = _contentHeaderFramePublisher.Subscribe(frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentHeader, methodType,
                    out var method))
                {
                    if (method.GetType() != methodType)
                    {
                        throw new InvalidOperationException($"Expected {methodType.FullName}, got {method.GetType().FullName}.");
                    }

                    _logger.Debug($"Content header {frame.ContentHeader.GetType().GetPrettyFullName()} for method {method.GetType().Name} on channel {frame.Channel} was expected.");
                    messageHandler(new MethodFrame(frame.Channel, method));
                }
            });

            _disposables.Add(contentHeaderSubscription);

            var contentBodySubscription = _contentBodyFramePublisher.Subscribe(frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentBody, methodType, out var method))
                {
                    if (method.GetType() != methodType)
                    {
                        throw new InvalidOperationException($"Expected {methodType.FullName}, got {method.GetType().FullName}.");
                    }

                    _logger.Debug($"Content body {frame.ContentBody.GetType().GetPrettyFullName()} for method {method.GetType().Name} on channel {frame.Channel} was expected.");
                    messageHandler(new MethodFrame(frame.Channel, method));
                }
            });

            _disposables.Add(contentBodySubscription);
        }

        public void On(Type type, Action<ProtocolHeaderFrame> messageHandler)
        {
            AssertNoDuplicateSubscriptions(type);

            var protocolHeaderSubscription = _protocolHeaderPublisher.Subscribe(type, frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ProtocolHeader))
                {
                    _logger.Debug($"{type.GetPrettyFullName()} on channel {frame.Channel} was expected.");
                    messageHandler(frame);
                }
            });

            _disposables.Add(protocolHeaderSubscription);
        }

        public void On(Type type, Action<HeartbeatFrame> messageHandler)
        {
            AssertNoDuplicateSubscriptions(type);

            var heartbeatSubscription = _heartbeatFramePublisher.Subscribe(type, frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Heartbeat))
                {
                    _logger.Debug($"{type.GetPrettyFullName()} on channel {frame.Channel} was expected.");
                    messageHandler(frame);
                }
            });

            _disposables.Add(heartbeatSubscription);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }

    //internal class AmqpServer2 : IDisposable
    //{
    //    private readonly ILogger _logger = LogFactory.Create<AmqpTestFramework>();
    //    private readonly ITypedMessageClient<Frame, Frame> _frameClient;
    //    private readonly IPublishProtocolHeader _protocolHeaderPublisher;
    //    private readonly IPublishMethod _methodFramePublisher;
    //    private readonly IPublish<ContentHeaderFrame> _contentHeaderFramePublisher;
    //    private readonly IPublish<ContentBodyFrame> _contentBodyFramePublisher;
    //    private readonly IPublishHeartbeat _heartbeatFramePublisher;

    //    private readonly List<IDisposable> _disposables = new List<IDisposable>();

    //    private readonly IExpectationStateMachine _expectationStateMachine;
    //    private readonly List<Type> _subscribedMethods = new List<Type>();

    //    public AmqpServer2(ProtocolVersion protocolVersion)
    //    {
    //        var protocolResolver = new ProtocolResolver(protocolVersion);
    //        _expectationStateMachine = protocolResolver.ExpectationStateMachine;
    //        var protocol = protocolResolver.Protocol;

    //        var networkClientFactory = new InternalRoutedNetworkClientFactory(out var serverNetworkClient);
    //        networkClientFactory.OnException += exception =>
    //        {
    //            // todo: Update base class (it sends Fatal on Error)
    //            _logger.Error(exception, "Test framework error.");
    //        };
    //        Client = networkClientFactory.Create();
    //        _disposables.Add(networkClientFactory);

    //        var protocolHeaderHandler = new ProtocolHeaderHandler();
    //        var methodFrameHandler = new MethodFrameHandler();
    //        var contentHeaderFrameHandler = new ContentHeaderFrameHandler();
    //        var contentBodyFrameHandler = new ContentBodyFrameHandler();
    //        var heartbeatFrameHandler = new HeartbeatFrameHandler();

    //        var heartbeatFrameRouter = new HeartbeatFrameRouter(null, protocol, heartbeatFrameHandler);
    //        var contentBodyFrameRouter = new ContentBodyFrameRouter(heartbeatFrameRouter, protocol, contentBodyFrameHandler);
    //        var contentHeaderFrameRouter = new ContentHeaderFrameRouter(contentBodyFrameRouter, protocol, contentHeaderFrameHandler);
    //        var methodFrameRouter = new MethodFrameRouter(contentHeaderFrameRouter, protocol, methodFrameHandler);

    //        var protocolHeaderClient = new ProtocolHeaderClient(serverNetworkClient, protocol);
    //        protocolHeaderClient.Received += protocolHeaderHandler.Handle;
    //        var frameClient = new FrameClient(protocolHeaderClient);
    //        frameClient.Received += methodFrameRouter.Handle;
    //        _frameClient = frameClient;

    //        _protocolHeaderPublisher = protocolHeaderHandler;
    //        _methodFramePublisher = methodFrameHandler;
    //        _contentHeaderFramePublisher = contentHeaderFrameHandler;
    //        _contentBodyFramePublisher = contentBodyFrameHandler;
    //        _heartbeatFramePublisher = heartbeatFrameHandler;
    //    }

    //    private void AssertNoDuplicateSubscriptions<TMethod>()
    //    {
    //        if (_subscribedMethods.Contains(typeof(TMethod)))
    //        {
    //            throw new InvalidOperationException($"There is already a subscription on {typeof(TMethod).GetPrettyFullName()}. There can only be one subscription per method type.");
    //        }

    //        _subscribedMethods.Add(typeof(TMethod));
    //    }

    //    public INetworkClient Client { get; }

    //    public void Send<TMessage>(MethodFrame<TMessage> frame) where TMessage : class, IServerMethod
    //    {
    //        _logger.Debug($"Sending method {typeof(TMessage).GetPrettyFullName()} on channel {frame.Channel}. {frame.Method.Serialize()}");
    //        _frameClient.Send(new Frame(Constants.FrameMethod, frame.Channel, frame.Method));
    //    }

    //    public void On<TClientMethod>(Action<MethodFrame<TClientMethod>> messageHandler)
    //        where TClientMethod : class, IClientMethod
    //    {
    //        AssertNoDuplicateSubscriptions<TClientMethod>();

    //        var methodSubscription = _methodFramePublisher.Subscribe<TClientMethod>((frame) =>
    //        {
    //            if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Method))
    //            {
    //                _logger.Debug($"Method {typeof(TClientMethod).GetPrettyFullName()} on channel {frame.Channel} was expected.");
    //                messageHandler(new MethodFrame<TClientMethod>(
    //                    frame.Channel,
    //                    frame.Method));
    //            }
    //        });

    //        _disposables.Add(methodSubscription);

    //        var contentHeaderSubscription = _contentHeaderFramePublisher.Subscribe(frame =>
    //        {
    //            if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentHeader,
    //                out TClientMethod method))
    //            {
    //                _logger.Debug($"Content header {frame.ContentHeader.GetType().GetPrettyFullName()} for method {typeof(TClientMethod).Name} on channel {frame.Channel} was expected.");
    //                messageHandler(new MethodFrame<TClientMethod>(frame.Channel, method));
    //            }
    //        });

    //        _disposables.Add(contentHeaderSubscription);

    //        var contentBodySubscription = _contentBodyFramePublisher.Subscribe(frame =>
    //        {
    //            if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentBody, out TClientMethod method))
    //            {
    //                _logger.Debug($"Content body {frame.ContentBody.GetType().GetPrettyFullName()} for method {typeof(TClientMethod).Name} on channel {frame.Channel} was expected.");
    //                messageHandler(new MethodFrame<TClientMethod>(frame.Channel, method));
    //            }
    //        });

    //        _disposables.Add(contentBodySubscription);
    //    }

    //    public void On<TClientMethod, TServerMethod>(Func<MethodFrame<TClientMethod>, TServerMethod> messageHandler)
    //        where TClientMethod : class, IClientMethod, INonContentMethod, IRespond<TServerMethod>
    //        where TServerMethod : class, IServerMethod
    //    {
    //        On<TClientMethod>(frame =>
    //        {
    //            var response = messageHandler(frame);
    //            Send(new MethodFrame<TServerMethod>(frame.Channel, response));
    //        });
    //    }

    //    public void On<TProtocolHeader>(Action<ProtocolHeaderFrame<TProtocolHeader>> messageHandler)
    //        where TProtocolHeader : class, IProtocolHeader
    //    {
    //        AssertNoDuplicateSubscriptions<TProtocolHeader>();

    //        var protocolHeaderSubscription = _protocolHeaderPublisher.Subscribe<TProtocolHeader>(frame =>
    //        {
    //            if (_expectationStateMachine.ShouldPass(frame.Channel, (IProtocolHeader)frame.ProtocolHeader))
    //            {
    //                _logger.Debug($"{typeof(TProtocolHeader).GetPrettyFullName()} on channel {frame.Channel} was expected.");
    //                messageHandler(frame);
    //            }
    //        });

    //        _disposables.Add(protocolHeaderSubscription);
    //    }

    //    public void On<TProtocolHeader>(Func<ProtocolHeaderFrame<TProtocolHeader>, Connection.Start> messageHandler)
    //        where TProtocolHeader : class, IProtocolHeader
    //    {
    //        On<TProtocolHeader>(header =>
    //        {
    //            var response = messageHandler(header);
    //            Send(new MethodFrame<Connection.Start>(0, response));
    //        });
    //    }

    //    public void On<THeartbeat>(Action<HeartbeatFrame<THeartbeat>> messageHandler)
    //        where THeartbeat : class, IHeartbeat
    //    {
    //        AssertNoDuplicateSubscriptions<THeartbeat>();

    //        var heartbeatSubscription = _heartbeatFramePublisher.Subscribe<THeartbeat>((frame) =>
    //        {
    //            if (_expectationStateMachine.ShouldPass(frame.Channel, (IHeartbeat)frame.Heartbeat))
    //            {
    //                _logger.Debug($"{typeof(THeartbeat).GetPrettyFullName()} on channel {frame.Channel} was expected.");
    //                messageHandler(frame);
    //            }
    //        });

    //        _disposables.Add(heartbeatSubscription);
    //    }

    //    public void Dispose()
    //    {
    //        foreach (var disposable in _disposables)
    //        {
    //            disposable.Dispose();
    //        }
    //    }
    //}
}