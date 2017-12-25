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
using Test.It.With.Amqp.Protocol.Expectations;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.Amqp.Protocol._091;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp
{
    internal class AmqpTestServer : IDisposable
    {
        private readonly ILogger _logger = LogFactory.Create<AmqpTestServer>();
        private readonly ITypedMessageClient<Frame, Frame> _frameClient;
        private readonly IPublishProtocolHeader _protocolHeaderPublisher;
        private readonly IPublishMethod _methodFramePublisher;
        private readonly IPublish<ContentHeaderFrame> _contentHeaderFramePublisher;
        private readonly IPublish<ContentBodyFrame> _contentBodyFramePublisher;
        private readonly IPublishHeartbeat _heartbeatFramePublisher;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private readonly IExpectationStateMachine _expectationStateMachine;
        private readonly List<Type> _subscribedMethods = new List<Type>();

        public AmqpTestServer(ProtocolVersion protocolVersion)
        {
            var protocolResolver = new ProtocolResolver(protocolVersion);
            _expectationStateMachine = protocolResolver.ExpectationStateMachine;
            var protocol = protocolResolver.Protocol;

            var networkClientFactory = new InternalRoutedNetworkClientFactory();
            networkClientFactory.OnException += exception =>
            {
                // todo: Update base class (it sends Fatal on Error)
                _logger.Error(exception, "Test framework error.");
            };
            Client = networkClientFactory.Create(out var serverNetworkClient);
            _disposables.Add(serverNetworkClient);

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

        public AmqpTestServer(ProtocolVersion protocolVersion, IExpectationStateMachine expectationStateMachine) : this(protocolVersion)
        {
            _expectationStateMachine = expectationStateMachine;
        }

        private void AssertNoDuplicateSubscriptions(Type type)
        {
            if (_subscribedMethods.Contains(type))
            {
                throw new InvalidOperationException($"There is already a subscription on {type.GetPrettyFullName()}. There can only be one subscription per method type.");
            }

            _subscribedMethods.Add(type);
        }

        public INetworkClient Client { get; }

        public void Send(MethodFrame frame)
        {
            _logger.Debug($"Sending method {frame.Method.GetType().GetPrettyFullName()} on channel {frame.Channel}. {frame.Method.Serialize()}");
            _frameClient.Send(new FrameMethod(frame.Channel, frame.Method));
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

            if (methodType.GetInterfaces().Contains(typeof(IContentMethod)))
            {
                var contentHeaderSubscription = _contentHeaderFramePublisher.Subscribe(frame =>
                {
                    if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentHeader, out var method))
                    {
                        if (method.GetType() != methodType)
                        {
                            throw new InvalidOperationException($"Expected {methodType.FullName}, got {method.GetType().FullName}.");
                        }

                        _logger.Debug(
                            $"Content header {frame.ContentHeader.GetType().GetPrettyFullName()} for method {method.GetType().Name} on channel {frame.Channel} was expected.");
                        messageHandler(new MethodFrame(frame.Channel, method));
                    }
                });

                _disposables.Add(contentHeaderSubscription);

                var contentBodySubscription = _contentBodyFramePublisher.Subscribe(frame =>
                {
                    if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentBody, out var method))
                    {
                        if (method.GetType() != methodType)
                        {
                            throw new InvalidOperationException($"Expected {methodType.FullName}, got {method.GetType().FullName}.");
                        }

                        _logger.Debug(
                            $"Content body {frame.ContentBody.GetType().GetPrettyFullName()} for method {method.GetType().Name} on channel {frame.Channel} was expected.");
                        messageHandler(new MethodFrame(frame.Channel, method));
                    }
                });

                _disposables.Add(contentBodySubscription);
            }
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
}