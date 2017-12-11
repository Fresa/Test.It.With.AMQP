using System;
using System.Collections.Generic;
using System.Linq;
using Log.It;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.MessageClient;
using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.MessageRouters;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Subscriptions;
using Frame = Test.It.With.Amqp.Protocol.Frame;

namespace Test.It.With.Amqp
{
    public class AmqpTestFramework : IDisposable
    {
        private readonly ILogger _logger = LogFactory.Create<AmqpTestFramework>();
        private readonly InternalRoutedNetworkClientFactory _networkClientFactory;
        private readonly FrameClient2 _frameClient;
        private readonly IPublish<ProtocolHeader> _protocolHeaderPublisher;
        private readonly IPublishMethod _methodFramePublisher;
        private readonly IPublishContentHeader _contentHeaderFramePublisher;
        private readonly IPublish<ContentBodyFrame> _contentBodyFramePublisher;
        private readonly IPublishHeartbeat _heartbeatFramePublisher;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private readonly Amqp091ExpectationStateMachine _expectationStateMachine = new Amqp091ExpectationStateMachine();
        private readonly List<Type> _subscribedMethods = new List<Type>();
        
        public AmqpTestFramework()
        {
            _networkClientFactory = new InternalRoutedNetworkClientFactory(out var serverNetworkClient);
            _networkClientFactory.OnException += exception =>
            {
                // todo: Update base class (it sends Fatal on Error)
                _logger.Error(exception, "Test framework error.");
                OnException?.Invoke(exception);
            };
            ConnectionFactory = _networkClientFactory;
            _disposables.Add(_networkClientFactory);

            var protocol = new AmqProtocol();

            var protocolHeaderHandler = new ProtocolHeaderHandler();
            var methodFrameHandler = new MethodFrameHandler();
            var contentHeaderFrameHandler = new ContentHeaderFrameHandler();
            var contentBodyFrameHandler = new ContentBodyFrameHandler();
            var heartbeatFrameHandler = new HeartbeatFrameHandler();

            var heartbeatFrameRouter = new HeartbeatFrameRouter(null, protocol, heartbeatFrameHandler);
            var contentBodyFrameRouter = new ContentBodyFrameRouter(heartbeatFrameRouter, protocol, contentBodyFrameHandler);
            var contentHeaderFrameRouter = new ContentHeaderFrameRouter(contentBodyFrameRouter, protocol, contentHeaderFrameHandler);
            var methodFrameRouter = new MethodFrameRouter(contentHeaderFrameRouter, protocol, methodFrameHandler);
            
            _frameClient = new FrameClient2(serverNetworkClient, protocolHeaderHandler, methodFrameRouter);

            _protocolHeaderPublisher = protocolHeaderHandler;
            _methodFramePublisher = methodFrameHandler;
            _contentHeaderFramePublisher = contentHeaderFrameHandler;
            _contentBodyFramePublisher = contentBodyFrameHandler;
            _heartbeatFramePublisher = heartbeatFrameHandler;
        }

        private void AssertNoDuplicateSubscriptions<TMethod>()
        {
            if (_subscribedMethods.Contains(typeof(TMethod)))
            {
                throw new InvalidOperationException($"There is already a subscription on {typeof(TMethod).Name}. There can only be one subscription per method type.");
            }

            _subscribedMethods.Add(typeof(TMethod));
        }

        public INetworkClientFactory ConnectionFactory { get; }

        public void Send<TMessage>(MethodFrame<TMessage> frame) where TMessage : class, IServerMethod
        {
            _logger.Debug($"Sending method {typeof(TMessage).Name} on channel {frame.Channel}. {frame.Method.Serialize()}");
            _frameClient.Send(new Frame(Constants.FrameMethod, frame.Channel, frame.Method));
        }

        public void On<TClientMethod, TContentHeader>(Action<MethodFrame<TClientMethod, TContentHeader>> messageHandler)
            where TClientMethod : class, IClientMethod, IContentMethod
            where TContentHeader : class, IContentHeader
        {
            AssertNoDuplicateSubscriptions<TClientMethod>();

            var methodSubscription = _methodFramePublisher.Subscribe<TClientMethod>((frame) =>
            {
                _logger.Debug($"Received method {typeof(TClientMethod).Name} on channel {frame.Channel}. {frame.Method.Serialize()}");
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Method))
                {
                    _logger.Debug($"Method {typeof(TClientMethod).Name} on channel {frame.Channel} was expected.");
                    messageHandler(new MethodFrame<TClientMethod, TContentHeader>(
                        frame.Channel,
                        frame.Method,
                        (TContentHeader) frame.Method.ContentHeader,
                        frame.Method.ContentBodyFragments.SelectMany(body => body.Payload).ToArray()));
                }
            });

            _disposables.Add(methodSubscription);
            
            var contentHeaderSubscription = _contentHeaderFramePublisher.Subscribe<TContentHeader>(frame =>
            {
                _logger.Debug($"Received content header {typeof(TContentHeader).Name} on channel {frame.Channel}. {frame.ContentHeader.Serialize()}");
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentHeader,
                    out TClientMethod method))
                {
                    _logger.Debug($"Content header {typeof(TContentHeader).Name} for method {typeof(TClientMethod).Name} on channel {frame.Channel} was expected.");
                    messageHandler(new MethodFrame<TClientMethod, TContentHeader>(frame.Channel, method, (TContentHeader)method.ContentHeader, method.ContentBodyFragments.SelectMany(body => body.Payload).ToArray()));
                }
            });

            _disposables.Add(contentHeaderSubscription);

            var contentBodySubscription = _contentBodyFramePublisher.Subscribe(frame =>
            {
                _logger.Debug($"Received content body {frame.ContentBody.GetType().Name} on channel {frame.Channel}.");
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentBody, out TClientMethod method))
                {
                    _logger.Debug($"Content body {frame.ContentBody.GetType().Name} for method {typeof(TClientMethod).Name} on channel {frame.Channel} was expected.");
                    messageHandler(new MethodFrame<TClientMethod, TContentHeader>(frame.Channel, method, (TContentHeader)method.ContentHeader, method.ContentBodyFragments.SelectMany(body => body.Payload).ToArray()));
                }
            });

            _disposables.Add(contentBodySubscription);
        }

        public void On<TClientMethod, TContentHeader, TServerMethod>(Func<MethodFrame<TClientMethod, TContentHeader>, TServerMethod> messageHandler)
            where TClientMethod : class, IClientMethod, IContentMethod
            where TContentHeader : class, IContentHeader
            where TServerMethod : class, IServerMethod
        {
            On<TClientMethod, TContentHeader>(frame =>
            {
                var response = messageHandler(frame);
                Send(new MethodFrame<TServerMethod>(frame.Channel, response));
            });
        }

        public void On<TClientMethod>(Action<MethodFrame<TClientMethod>> messageHandler)
            where TClientMethod : class, IClientMethod, INonContentMethod
        {
            AssertNoDuplicateSubscriptions<TClientMethod>();

            var methodSubscription = _methodFramePublisher.Subscribe<TClientMethod>(frame =>
            {
                _logger.Debug($"Received method {typeof(TClientMethod).Name} on channel {frame.Channel}. {frame.Method.Serialize()}");
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Method))
                {
                    _logger.Debug($"Method {typeof(TClientMethod).Name} on channel {frame.Channel} was expected.");
                    messageHandler(frame);
                }
            });

            _disposables.Add(methodSubscription);
        }

        public void On<TClientMethod, TServerMethod>(Func<MethodFrame<TClientMethod>, TServerMethod> messageHandler)
            where TClientMethod : class, IClientMethod, INonContentMethod, IRespond<TServerMethod>
            where TServerMethod : class, IServerMethod
        {
            On<TClientMethod>(frame =>
            {
                var response = messageHandler(frame);
                Send(new MethodFrame<TServerMethod>(frame.Channel, response));
            });
        }

        public void OnProtocolHeader(Action<ProtocolHeader> messageHandler)
        {
            AssertNoDuplicateSubscriptions<ProtocolHeader>();

            var protocolHeaderSubscription = _protocolHeaderPublisher.Subscribe(header =>
            {
                _logger.Debug($"Received protocol header.");
                if (_expectationStateMachine.ShouldPass(header))
                {
                    _logger.Debug($"Protocol header was expected.");
                    messageHandler(header);
                }
            });

            _disposables.Add(protocolHeaderSubscription);
        }

        public void OnProtocolHeader(Func<ProtocolHeader, Connection.Start> messageHandler)
        {
            OnProtocolHeader(header =>
            {
                var response = messageHandler(header);
                Send(new MethodFrame<Connection.Start>(0, response));
            });
        }

        public void On<THeartbeat>(Action<HeartbeatFrame<THeartbeat>> messageHandler)
            where THeartbeat : class, IHeartbeat
        {
            AssertNoDuplicateSubscriptions<THeartbeat>();

            var heartbeatSubscription = _heartbeatFramePublisher.Subscribe<THeartbeat>((frame) =>
            {
                _logger.Debug($"Received heartbeat {typeof(THeartbeat).Name} on channel {frame.Channel}. {frame.Heartbeat.Serialize()}");
                if (_expectationStateMachine.ShouldPass(frame.Channel, (IHeartbeat)frame.Heartbeat))
                {
                    _logger.Debug($"{typeof(THeartbeat).Name} on channel {frame.Channel} was expected.");
                    messageHandler(frame);
                }
            });

            _disposables.Add(heartbeatSubscription);
        }

        public event Action<Exception> OnException;

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}