using Test.It.With.Amqp.MessageHandlers;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageClient
{  
    internal class FrameClient
    {
        private readonly INetworkClient _networkClient;

        public FrameClient(INetworkClient networkClient, IHandle<ProtocolHeader> protocolHeaderHandler,
            IHandle<Frame> frameHandler)
        {
            _networkClient = networkClient;
            networkClient.BufferReceived += (sender, args) =>
            {
                var reader = new AmqpReader(args.Buffer);
                if (reader.PeekByte() == 'A')
                {
                    var header = ProtocolHeader.ReadFrom(reader);
                    protocolHeaderHandler.Handle(header);
                    return;
                }

                var frame = Frame.ReadFrom(reader);
                reader.ThrowIfMoreData();

                frameHandler.Handle(frame);
            };
        }

        public void Send(Frame frame)
        {
            _networkClient.Send(frame);
        }
    }
}