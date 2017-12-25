using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091; // todo: cannot reference explicit protocol

namespace Test.It.With.Amqp.MessageClient
{
    internal class ContentBodyFrameClient : ITypedMessageClient<ContentBodyFrame, Frame>, IChainableTypedMessageClient<Frame, Frame>
    {
        private readonly IChainableTypedMessageClient<Frame, Frame> _frameClient;

        public ContentBodyFrameClient(IChainableTypedMessageClient<Frame, Frame> frameClient, IProtocol protocol)
        {
            _frameClient = frameClient;

            frameClient.Next += frame =>
            {
                if (frame.Type == Constants.FrameBody)
                {
                    var reader = new AmqpReader(frame.Payload);
                    var contentBody = protocol.GetContentBody(reader);

                    if (Received == null)
                    {
                        throw new InvalidOperationException($"Missing subscription on {contentBody.GetType().FullName}.");
                    }

                    Received.Invoke(new ContentBodyFrame(frame.Channel, contentBody));
                }
                else
                {
                    if (Next == null)
                    {
                        throw new InvalidOperationException($"Missing handler of frame type {frame.Type}.");
                    }

                    Next.Invoke(frame);
                }
            };

            frameClient.Disconnected += Disconnected;
        }

        public event Action<ContentBodyFrame> Received;

        public event Action Disconnected;

        public void Send(Frame frame)
        {
            _frameClient.Send(frame);
        }

        public event Action<Frame> Next;
    }
}