using System;
using Test.It.With.Amqp.Protocol;
using Test.It.With.RabbitMQ.NetworkClient;
using Test.It.With.RabbitMQ.Protocol;

namespace Test.It.With.RabbitMQ.MessageClient
{
    internal class FrameClient : ITypedMessageClient<Frame, Frame>, ITypedMessageClient<ProtocolHeader, Frame>
    {
        public FrameClient(INetworkClient networkClient)
        {
            networkClient.BufferReceived += (sender, args) =>
            {
                var reader = new AmqpReader(args.Buffer);
                if (reader.PeekByte() == 'A')
                {
                    var header = ProtocolHeader.ReadFrom(reader);
                    ReceivedProtocolHeader?.Invoke(this, header);
                    return;
                }

                var frame = Frame.ReadFrom(reader);
                Received?.Invoke(this, frame);
            };
        }

        public event EventHandler<Frame> Received;

        private event EventHandler<ProtocolHeader> ReceivedProtocolHeader;
        event EventHandler<ProtocolHeader> ITypedMessageClient<ProtocolHeader, Frame>.Received
        {
            add => ReceivedProtocolHeader += value;
            remove => ReceivedProtocolHeader -= value;
        }

        public event EventHandler Disconnected;
        public void Send(Frame frame)
        {
            Frame.WriteTo(frame);
        }
    }
}