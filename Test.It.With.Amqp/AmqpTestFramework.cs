using System;
using System.Collections.Generic;
using Log.It;
using Test.It.With.Amqp.Expectations;
using Test.It.With.Amqp.MessageClient;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;
using Frame = Test.It.With.Amqp.Protocol.Frame;

namespace Test.It.With.Amqp
{
    public class AmqpTestFramework : IDisposable
    {
        private readonly ILogger _logger = LogFactory.Create<AmqpTestFramework>();
        private readonly InternalRoutedNetworkClientFactory _networkClientFactory;
        private readonly FrameClient _frameClient;
        private readonly ITypedMessageClient<ProtocolHeader, Frame> _protocolHeaderClient;
        private readonly ITypedMessageClient<MethodFrame, Frame> _methodFrameClient;
        private readonly ITypedMessageClient<ContentHeaderFrame, Frame> _contentHeaderFrameClient;
        private readonly ITypedMessageClient<ContentBodyFrame, Frame> _contentBodyFrameClient;
        
        private readonly List<Type> _methodsSubscribedOn = new List<Type>();

        private readonly ExpectationStateMachine _expectationStateMachine = new ExpectationStateMachine();

        public AmqpTestFramework()
        {
            _networkClientFactory = new InternalRoutedNetworkClientFactory(out var serverNetworkClient);
            ConnectionFactory = _networkClientFactory;

            var protocol = new AmqProtocol();
            _protocolHeaderClient = _frameClient = new FrameClient(serverNetworkClient);
            var methodFrameClient = new MethodFrameClient(_frameClient, protocol);
            var contentHeaderFrameClientChain = new ContentHeaderFrameClient(methodFrameClient, protocol);
            _contentBodyFrameClient = new ContentBodyFrameClient(contentHeaderFrameClientChain, protocol);
            _methodFrameClient = methodFrameClient;
            _contentHeaderFrameClient = contentHeaderFrameClientChain;
        }

        public INetworkClientFactory ConnectionFactory { get; }

        private void _subscribeOn<TMethod>()
        {
            if (_methodsSubscribedOn.Contains(typeof(TMethod)))
            {
                throw new InvalidOperationException("It is only allowed to subscribe on a specific method once.");
            }

            _methodsSubscribedOn.Add(typeof(TMethod));
        }

        public void Send<TMessage>(MethodFrame<TMessage> frame) where TMessage : IServerMethod
        {
            _logger.Debug($"Sending method {typeof(TMessage).Name} on channel {frame.Channel}.");
            _frameClient.Send(new Frame(Constants.FrameMethod, frame.Channel, frame.Method));
        }

        public void On<TClientMethod>(Action<MethodFrame<TClientMethod>> messageHandler)
            where TClientMethod : IClientMethod
        {
            _subscribeOn<TClientMethod>();

            var methodFrameClient = new MethodFrameClient<TClientMethod>(_methodFrameClient);
            methodFrameClient.Received += (sender, frame) =>
            {
                _logger.Debug($"Received method {typeof(TClientMethod).Name} on channel {frame.Channel}.");
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.Method))
                {
                    _logger.Debug($"Method {typeof(TClientMethod).Name} on channel {frame.Channel} was expected.");
                    messageHandler(frame);
                }
            };

            _contentHeaderFrameClient.Received += (sender, frame) =>
            {
                _logger.Debug($"Received a content header for method {typeof(TClientMethod).Name} on channel {frame.Channel}.");
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentHeader, out TClientMethod method))
                {
                    _logger.Debug($"Content header for method {typeof(TClientMethod).Name} on channel {frame.Channel} was expected.");
                    messageHandler(new MethodFrame<TClientMethod>(frame.Channel, method));
                }
            };

            _contentBodyFrameClient.Received += (sender, frame) =>
            {
                _logger.Debug($"Received content body for method {typeof(TClientMethod).Name} on channel {frame.Channel}.");
                if (_expectationStateMachine.ShouldPass(frame.Channel, frame.ContentBody, out TClientMethod method))
                {
                    _logger.Debug($"Content body for method {typeof(TClientMethod).Name} on channel {frame.Channel} was expected.");
                    messageHandler(new MethodFrame<TClientMethod>(frame.Channel, method));
                }
            };
        }

        public void On<TClientMethod, TServerMethod>(Func<MethodFrame<TClientMethod>, TServerMethod> messageHandler)
            where TClientMethod : IClientMethod, IRespond<TServerMethod>
            where TServerMethod : IServerMethod
        {
            On<TClientMethod>(frame =>
            {
                var response = messageHandler(frame);
                Send(new MethodFrame<TServerMethod>(frame.Channel, response));
            });
        }

        public void OnProtocolHeader(Action<ProtocolHeader> messageHandler)
        {
            _subscribeOn<ProtocolHeader>();

            _protocolHeaderClient.Received += (sender, header) =>
            {
                _logger.Debug($"Received protocol header.");
                if (_expectationStateMachine.ShouldPass(header))
                {
                    _logger.Debug($"Protocol header was expected.");
                    messageHandler(header);
                }
            };
        }

        public void OnProtocolHeader(Func<ProtocolHeader, Connection.Start> messageHandler)
        {
            OnProtocolHeader(header =>
            {
                var response = messageHandler(header);
                _protocolHeaderClient.Send(new Frame(Constants.FrameMethod, 0, response));
            });
        }

        public void Dispose()
        {
            _networkClientFactory.Dispose();
        }
    }
}