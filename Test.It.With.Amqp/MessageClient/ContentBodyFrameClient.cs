using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageClient
{
    internal class ContentBodyFrameClient : ITypedMessageClient<ContentBodyFrame, Frame>, IChainableTypedMessageClient<Frame, Frame>
    {
        private readonly IChainableTypedMessageClient<Frame, Frame> _frameClient;

        public ContentBodyFrameClient(IChainableTypedMessageClient<Frame, Frame> frameClient, IProtocol protocol)
        {
            _frameClient = frameClient;

            frameClient.Next += (sender, args) =>
            {
                if (args.Type == Constants.FrameBody)
                {
                    var reader = new AmqpReader(args.Payload);
                    var contentBody = protocol.GetContentBody(reader);

                    Received?.Invoke(this, new ContentBodyFrame(args.Channel, contentBody));
                }
                else
                {
                    Next?.Invoke(sender, args);
                }
            };

            frameClient.Disconnected += Disconnected;
        }

        public event EventHandler<ContentBodyFrame> Received;

        public event EventHandler Disconnected;

        public void Send(Frame frame)
        {
            _frameClient.Send(frame);
        }

        public event EventHandler<Frame> Next;
    }
}