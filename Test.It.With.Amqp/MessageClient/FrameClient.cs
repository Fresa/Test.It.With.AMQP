using System;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageClient
{  
    internal class FrameClient : ITypedMessageClient<IFrame, IFrame>
    {
        private readonly IChainableTypedMessageClient<ReceivedEventArgs, IFrame> _networkClient;

        public FrameClient(IChainableTypedMessageClient<ReceivedEventArgs, IFrame> networkClient, IAmqpReaderFactory readerFactory, IFrameFactory frameFactory)
        {
            _networkClient = networkClient;
            networkClient.Next += args =>
            {
                var reader = readerFactory.Create(args.Buffer);
                var frame = frameFactory.Create(reader);
                reader.ThrowIfMoreData();

                if (Received == null)
                {
                    throw new InvalidOperationException($"Missing subscription on {frame.GetType().FullName}.");
                }

                Received.Invoke(frame);
            };
        }

        public event Action<IFrame> Received;

        public void Send(IFrame frame)
        {
            _networkClient.Send(frame);
        }
    }
}