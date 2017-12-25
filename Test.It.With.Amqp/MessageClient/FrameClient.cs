using System;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091;

namespace Test.It.With.Amqp.MessageClient
{  
    internal class FrameClient : ITypedMessageClient<IFrame, IFrame>
    {
        private readonly IChainableTypedMessageClient<ReceivedEventArgs, IFrame> _networkClient;

        public FrameClient(IChainableTypedMessageClient<ReceivedEventArgs, IFrame> networkClient)
        {
            _networkClient = networkClient;
            networkClient.Next += args =>
            {
                var reader = new Amqp091Reader(args.Buffer);
                var frame = Amqp091Frame.ReadFrom(reader);
                reader.ThrowIfMoreData();

                if (Received == null)
                {
                    throw new InvalidOperationException($"Missing subscription on {frame.GetType().FullName}.");
                }

                Received.Invoke(frame);
            };

            networkClient.Disconnected += Disconnected;
        }

        public event Action<IFrame> Received;
        public event Action Disconnected;

        public void Send(IFrame frame)
        {
            _networkClient.Send(frame);
        }
    }
}