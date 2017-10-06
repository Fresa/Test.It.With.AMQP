using System;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Protocol;
using Test.It.With.RabbitMQ.Messages;
using Test.It.With.RabbitMQ.Protocol;

namespace Test.It.With.RabbitMQ.MessageClient
{
    internal class ContentHeaderFrameClient : ITypedMessageClient<ContentHeaderFrame, Frame>, IChainableTypedMessageClient<Frame, Frame>
    {
        private readonly IChainableTypedMessageClient<Frame, Frame> _frameClient;

        public ContentHeaderFrameClient(IChainableTypedMessageClient<Frame, Frame> frameClient, IProtocol protocol)
        {
            _frameClient = frameClient;

            frameClient.Next += (sender, args) =>
            {
                if (args.Type == Constants.FrameHeader)
                {
                    var reader = new AmqpReader(args.Payload);
                    var contentHeader = protocol.GetContentHeader(reader);

                    Received?.Invoke(this, new ContentHeaderFrame(args.Channel, contentHeader));
                }
                else
                {
                    Next?.Invoke(sender, args);
                }
            };

            frameClient.Disconnected += Disconnected;
        }

        public event EventHandler<ContentHeaderFrame> Received;

        public event EventHandler Disconnected;

        public void Send(Frame frame)
        {
            _frameClient.Send(frame);
        }

        public event EventHandler<Frame> Next;
    }
}