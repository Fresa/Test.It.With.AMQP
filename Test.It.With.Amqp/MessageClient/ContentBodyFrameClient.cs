using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091; // todo: cannot reference explicit protocol

namespace Test.It.With.Amqp.MessageClient
{
    internal class ContentBodyFrameClient : ITypedMessageClient<ContentBodyFrame, IFrame>, IChainableTypedMessageClient<IFrame, IFrame>
    {
        private readonly IChainableTypedMessageClient<IFrame, IFrame> _frameClient;

        public ContentBodyFrameClient(IChainableTypedMessageClient<IFrame, IFrame> frameClient, IProtocol protocol)
        {
            _frameClient = frameClient;

            frameClient.Next += frame =>
            {
                if (frame.IsBody())
                {
                    var reader = new Amqp091Reader(frame.Payload);
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
                        throw new InvalidOperationException($"Missing handler for frame {frame}.");
                    }

                    Next.Invoke(frame);
                }
            };

            frameClient.Disconnected += Disconnected;
        }

        public event Action<ContentBodyFrame> Received;

        public event Action Disconnected;

        public void Send(IFrame frame)
        {
            _frameClient.Send(frame);
        }

        public event Action<IFrame> Next;
    }
}