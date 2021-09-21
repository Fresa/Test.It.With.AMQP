using System;
using System.Collections.Generic;
using System.Linq;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.Logging;
using Test.It.With.Amqp.MessageClient;
using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.MessageRouters;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Expectations;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp
{
    internal class AmqpConnectionSession : IDisposable, ISender<MethodFrame>
    {
        private readonly InternalLogger _logger = LogFactory.Create<AmqpConnectionSession>();
        private readonly ITypedMessageClient<IFrame, IFrame> _frameClient;
        private readonly IPublishProtocolHeader _protocolHeaderPublisher;
        private readonly IPublishMethod _methodFramePublisher;
        private readonly IPublish<ContentHeaderFrame> _contentHeaderFramePublisher;
        private readonly IPublish<ContentBodyFrame> _contentBodyFramePublisher;
        private readonly IPublishHeartbeat _heartbeatFramePublisher;

        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        private readonly IExpectationStateMachine _expectationStateMachine;
        private readonly IFrameFactory _frameFactory;

        public AmqpConnectionSession(
            IProtocolResolver protocolResolver, 
            IConfiguration configuration, 
            INetworkClient serverNetworkClient)
        {
            _expectationStateMachine = protocolResolver.ExpectationStateMachineFactory.Create();
            _frameFactory = protocolResolver.FrameFactory;
            var protocol = protocolResolver.Protocol;
            var amqpReaderFactory = protocolResolver.AmqpReaderFactory;
            var amqpWriterFactory = protocolResolver.AmqpWriterFactory;

            var protocolHeaderHandler = new ProtocolHeaderHandler();
            var methodFrameHandler = new MethodFrameHandler(configuration.AutomaticReply, this);
            var contentHeaderFrameHandler = new ContentHeaderFrameHandler();
            var contentBodyFrameHandler = new ContentBodyFrameHandler();
            var heartbeatFrameHandler = new HeartbeatFrameHandler();

            var heartbeatFrameRouter = new HeartbeatFrameRouter(null, protocol, heartbeatFrameHandler, amqpReaderFactory);
            var contentBodyFrameRouter = new ContentBodyFrameRouter(heartbeatFrameRouter, protocol, contentBodyFrameHandler, amqpReaderFactory);
            var contentHeaderFrameRouter = new ContentHeaderFrameRouter(contentBodyFrameRouter, protocol, contentHeaderFrameHandler, amqpReaderFactory);
            var methodFrameRouter = new MethodFrameRouter(contentHeaderFrameRouter, protocol, methodFrameHandler, amqpReaderFactory);

            var protocolHeaderClient = new ProtocolHeaderClient(serverNetworkClient, protocol, amqpReaderFactory, amqpWriterFactory);
            protocolHeaderClient.Received += frame =>
            {
                SetupLogicalLogThreadContextsForFrame(frame.Channel);
                protocolHeaderHandler.Handle(frame);
            };
            var frameClient = new FrameClient(protocolHeaderClient, amqpReaderFactory, _frameFactory);
            frameClient.Received += frame =>
            {
                SetupLogicalLogThreadContextsForFrame(frame.Channel);
                methodFrameRouter.Handle(frame);
            };
            _frameClient = frameClient;

            _protocolHeaderPublisher = protocolHeaderHandler;
            _methodFramePublisher = methodFrameHandler;
            _contentHeaderFramePublisher = contentHeaderFrameHandler;
            _contentBodyFramePublisher = contentBodyFrameHandler;
            _heartbeatFramePublisher = heartbeatFrameHandler;
        }

        private void SetupLogicalLogThreadContextsForFrame(short channel)
        {
            _logger.LogicalThreadContext.Set(LogicalLogContextKeys.ConnectionId, ConnectionId);
            _logger.LogicalThreadContext.Set(LogicalLogContextKeys.ChannelId, channel);
        }

        public ConnectionId ConnectionId { get; } = new ConnectionId(Guid.NewGuid());
        
        public void Send(MethodFrame frame)
        {
            _logger.Debug("Sending {MethodFrameName} {MessageName}. {@Message}", nameof(MethodFrame), frame.Message.GetType().GetPrettyFullName(), frame.Message);
            _frameClient.Send(_frameFactory.Create(frame.Channel, frame.Message));
        }

        public void Send(HeartbeatFrame frame)
        {
            _logger.Debug("Sending {HeartbeatFrameName} {MessageName}. {@Message}", nameof(HeartbeatFrame), frame.Message.GetType().GetPrettyFullName(), frame.Message);
            _frameClient.Send(_frameFactory.Create(frame.Channel, frame.Message));
        }

        public void Send(ContentHeaderFrame frame)
        {
            _logger.Debug("Sending {ContentHeaderFrameName} {MessageName}. {@Message}", nameof(ContentHeaderFrame), frame.Message.GetType().GetPrettyFullName(), frame.Message);
            _frameClient.Send(_frameFactory.Create(frame.Channel, frame.Message));
        }

        public void Send(ContentBodyFrame frame)
        {
            _logger.Debug("Sending {ContentBodyFrameName} {MessageName}. {@Message}", nameof(ContentBodyFrame), frame.Message.GetType().GetPrettyFullName(), frame.Message);
            _frameClient.Send(_frameFactory.Create(frame.Channel, frame.Message));
        }
        
        public void On(Type methodType, Action<MethodFrame> messageHandler)
        {
            var methodSubscription = _methodFramePublisher.Subscribe(methodType, frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Message))
                {
                    _logger.Debug("Method {Method} was expected.", methodType.GetPrettyFullName());
                    messageHandler(new MethodFrame(
                        frame.Channel,
                        frame.Message));
                }
            });

            _subscriptions.Add(methodSubscription);

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
                            "Content header {MessageType} for method {MethodType} was expected.", frame.Message.GetType().GetPrettyFullName(), method.GetType().Name);
                        messageHandler(new MethodFrame(frame.Channel, method));
                    }
                });

                _subscriptions.Add(contentHeaderSubscription);

                var contentBodySubscription = _contentBodyFramePublisher.Subscribe(frame =>
                {
                    if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Message, out var method))
                    {
                        if (method.GetType() != methodType)
                        {
                            throw new InvalidOperationException($"Expected {methodType.FullName}, got {method.GetType().FullName}.");
                        }

                        _logger.Debug(
                            "Content body {MessageType} for method {MethodType} was expected.", frame.Message.GetType().GetPrettyFullName(), method.GetType().Name);
                        messageHandler(new MethodFrame(frame.Channel, method));
                    }
                });

                _subscriptions.Add(contentBodySubscription);
            }
        }

        public void On(Type type, Action<ProtocolHeaderFrame> messageHandler)
        {
            var protocolHeaderSubscription = _protocolHeaderPublisher.Subscribe(type, frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Message))
                {
                    _logger.Debug("{MethodType} was expected.", type.GetPrettyFullName());
                    messageHandler(frame);
                }
            });

            _subscriptions.Add(protocolHeaderSubscription);
        }

        public void On(Type type, Action<HeartbeatFrame> messageHandler)
        {
            var heartbeatSubscription = _heartbeatFramePublisher.Subscribe(type, frame =>
            {
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Message))
                {
                    _logger.Debug("{MethodType} was expected.", type.GetPrettyFullName());
                    messageHandler(frame);
                }
            });

            _subscriptions.Add(heartbeatSubscription);
        }

        public void Dispose()
        {
            foreach (var disposable in _subscriptions)
            {
                disposable.Dispose();
            }
        }
    }
}