using System;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Protocol;
using Test.It.With.RabbitMQ.MessageClient;
using Test.It.With.RabbitMQ.Messages;
using Test.It.With.RabbitMQ.NetworkClient;
using Test.It.With.RabbitMQ.Protocol;
using Constants = RabbitMQ.Client.Framing.Constants;

namespace Test.It.With.RabbitMQ
{
    public class AmqpTestFramework : IDisposable
    {
        private readonly INetworkClient _serverNetworkClient;
        private readonly FrameClient _frameClient;
        private readonly ITypedMessageClient<ProtocolHeader, Frame> _protocolHeaderClient;
        private readonly MethodFrameClient _methodFrameClient;

        public AmqpTestFramework()
        {
            var networkClientFactory = new InternalRoutedNetworkClientFactory(out _serverNetworkClient);
            ConnectionFactory = networkClientFactory;

            _protocolHeaderClient = _frameClient = new FrameClient(_serverNetworkClient);
            _methodFrameClient = new MethodFrameClient(_frameClient, new AmqProtocol());
        }

        public INetworkClientFactory ConnectionFactory { get; }

        public void Send<TMessage>(MethodFrame<TMessage> frame) where TMessage : IServerMethod
        {
            _frameClient.Send(new Frame(Constants.FrameMethod, frame.Channel, frame.Method));
        }

        public void On<TClientMethod>(Action<MethodFrame<TClientMethod>> messageHandler)
            where TClientMethod : IClientMethod
        {
            var server = new MethodFrameClient<TClientMethod>(_methodFrameClient);
            server.Received += (sender, frame) =>
            {
                messageHandler(frame);
            };
        }

        public void On<TClientMethod, TServerMethod>(Func<MethodFrame<TClientMethod>, TServerMethod> messageHandler)
            where TClientMethod : IClientMethod, IRespond<TServerMethod>
            where TServerMethod : IServerMethod
        {
            var server = new MethodFrameClient<TClientMethod>(_methodFrameClient);
            server.Received += (sender, frame) =>
            {
                var responseMethod = messageHandler(frame);
                server.Send(new Frame(Constants.FrameMethod, frame.Channel, responseMethod));
            };
        }

        public void OnProtocolHeader(Func<ProtocolHeader, Connection.Start> messageHandler)
        {
            _protocolHeaderClient.Received += (sender, header) =>
            {
                var response = messageHandler(header);
                _protocolHeaderClient.Send(new Frame(Constants.FrameMethod, 0, response));
            };
        }

        public void Dispose()
        {
            _serverNetworkClient.Dispose();
        }
    }
}