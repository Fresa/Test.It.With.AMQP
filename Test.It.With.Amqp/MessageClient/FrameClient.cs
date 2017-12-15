using System;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageClient
{  
    internal class FrameClient : ITypedMessageClient<Frame, Frame>
    {
        private readonly IChainableTypedMessageClient<ReceivedEventArgs, Frame> _networkClient;

        public FrameClient(IChainableTypedMessageClient<ReceivedEventArgs, Frame> networkClient)
        {
            _networkClient = networkClient;
            networkClient.Next += args =>
            {
                var reader = new AmqpReader(args.Buffer);
                var frame = Frame.ReadFrom(reader);
                reader.ThrowIfMoreData();

                if (Received == null)
                {
                    throw new InvalidOperationException($"Missing subscription on {frame.GetType().FullName}.");
                }

                Received.Invoke(frame);
            };

            networkClient.Disconnected += Disconnected;
        }

        public event Action<Frame> Received;
        public event Action Disconnected;

        public void Send(Frame frame)
        {
            _networkClient.Send(frame);
        }
    }
}