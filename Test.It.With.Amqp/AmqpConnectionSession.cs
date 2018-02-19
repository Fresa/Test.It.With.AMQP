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
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp
{
    internal class AmqpConnectionSession : IDisposable
    {
        private readonly ILogger _logger = LogFactory.Create<AmqpConnectionSession>();
        private readonly ITypedMessageClient<IFrame, IFrame> _frameClient;
        private readonly IPublishProtocolHeader _protocolHeaderPublisher;
        private readonly IPublishMethod _methodFramePublisher;
        private readonly IPublish<ContentHeaderFrame> _contentHeaderFramePublisher;
        private readonly IPublish<ContentBodyFrame> _contentBodyFramePublisher;
        private readonly IPublishHeartbeat _heartbeatFramePublisher;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private readonly IExpectationStateMachine _expectationStateMachine;
        private readonly IFrameFactory _frameFactory;

        public AmqpConnectionSession(ProtocolVersion protocolVersion)
        {
            var protocolResolver = new ProtocolResolver(protocolVersion);
            _expectationStateMachine = protocolResolver.ExpectationStateMachine;
            _frameFactory = protocolResolver.FrameFactory;
            var protocol = protocolResolver.Protocol;
            var amqpReaderFactory = protocolResolver.AmqpReaderFactory;
            var amqpWriterFactory = protocolResolver.AmqpWriterFactory;

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

            var heartbeatFrameRouter = new HeartbeatFrameRouter(null, protocol, heartbeatFrameHandler, amqpReaderFactory);
            var contentBodyFrameRouter = new ContentBodyFrameRouter(heartbeatFrameRouter, protocol, contentBodyFrameHandler, amqpReaderFactory);
            var contentHeaderFrameRouter = new ContentHeaderFrameRouter(contentBodyFrameRouter, protocol, contentHeaderFrameHandler, amqpReaderFactory);
            var methodFrameRouter = new MethodFrameRouter(contentHeaderFrameRouter, protocol, methodFrameHandler, amqpReaderFactory);

            var protocolHeaderClient = new ProtocolHeaderClient(serverNetworkClient, protocol, amqpReaderFactory, amqpWriterFactory);
            protocolHeaderClient.Received += protocolHeaderHandler.Handle;
            var frameClient = new FrameClient(protocolHeaderClient, amqpReaderFactory, _frameFactory);
            frameClient.Received += methodFrameRouter.Handle;
            _frameClient = frameClient;

            _protocolHeaderPublisher = protocolHeaderHandler;
            _methodFramePublisher = methodFrameHandler;
            _contentHeaderFramePublisher = contentHeaderFrameHandler;
            _contentBodyFramePublisher = contentBodyFrameHandler;
            _heartbeatFramePublisher = heartbeatFrameHandler;
        }

        public ConnectionId ConnectionId { get; } = new ConnectionId(Guid.NewGuid());
        
        public INetworkClient Client { get; }
        
        public void Send(MethodFrame frame)
        {
            _logger.Debug($"Sending {nameof(MethodFrame)} {frame.Message.GetType().GetPrettyFullName()} on channel {frame.Channel}. {frame.Message.Serialize()}");
            _frameClient.Send(_frameFactory.Create(frame.Channel, frame.Message));
        }

        public void Send(HeartbeatFrame frame)
        {
            _logger.Debug($"Sending {nameof(HeartbeatFrame)} {frame.Message.GetType().GetPrettyFullName()} on channel {frame.Channel}. {frame.Message.Serialize()}");
            _frameClient.Send(_frameFactory.Create(frame.Channel, frame.Message));
        }

        public void Send(ContentHeaderFrame frame)
        {
            _logger.Debug($"Sending {nameof(ContentHeaderFrame)} {frame.Message.GetType().GetPrettyFullName()} on channel {frame.Channel}. {frame.Message.Serialize()}");
            _frameClient.Send(_frameFactory.Create(frame.Channel, frame.Message));
        }

        public void Send(ContentBodyFrame frame)
        {
            _logger.Debug($"Sending {nameof(ContentBodyFrame)} {frame.Message.GetType().GetPrettyFullName()} on channel {frame.Channel}.");
            _frameClient.Send(_frameFactory.Create(frame.Channel, frame.Message));
        }
        
        public void On(Type methodType, Action<MethodFrame> messageHandler)
        {
            var methodSubscription = _methodFramePublisher.Subscribe(methodType, frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Message))
                {
                    _logger.Debug($"Method {methodType.GetPrettyFullName()} on channel {frame.Channel} was expected.");
                    messageHandler(new MethodFrame(
                        frame.Channel,
                        frame.Message));
                }
            });

            _disposables.Add(methodSubscription);

            if (methodType.GetInterfaces().Contains(typeof(IContentMethod)))
            {
                var contentHeaderSubscription = _contentHeaderFramePublisher.Subscribe(frame =>
                {
                    if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Message, out var method))
                    {
                        if (method.GetType() != methodType)
                        {
                            throw new InvalidOperationException($"Expected {methodType.FullName}, got {method.GetType().FullName}.");
                        }

                        _logger.Debug(
                            $"Content header {frame.Message.GetType().GetPrettyFullName()} for method {method.GetType().Name} on channel {frame.Channel} was expected.");
                        messageHandler(new MethodFrame(frame.Channel, method));
                    }
                });

                _disposables.Add(contentHeaderSubscription);

                var contentBodySubscription = _contentBodyFramePublisher.Subscribe(frame =>
                {
                    if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Message, out var method))
                    {
                        if (method.GetType() != methodType)
                        {
                            throw new InvalidOperationException($"Expected {methodType.FullName}, got {method.GetType().FullName}.");
                        }

                        _logger.Debug(
                            $"Content body {frame.Message.GetType().GetPrettyFullName()} for method {method.GetType().Name} on channel {frame.Channel} was expected.");
                        messageHandler(new MethodFrame(frame.Channel, method));
                    }
                });

                _disposables.Add(contentBodySubscription);
            }
        }

        public void On(Type type, Action<ProtocolHeaderFrame> messageHandler)
        {
            var protocolHeaderSubscription = _protocolHeaderPublisher.Subscribe(type, frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Message))
                {
                    _logger.Debug($"{type.GetPrettyFullName()} on channel {frame.Channel} was expected.");
                    messageHandler(frame);
                }
            });

            _disposables.Add(protocolHeaderSubscription);
        }

        public void On(Type type, Action<HeartbeatFrame> messageHandler)
        {
            var heartbeatSubscription = _heartbeatFramePublisher.Subscribe(type, frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Message))
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