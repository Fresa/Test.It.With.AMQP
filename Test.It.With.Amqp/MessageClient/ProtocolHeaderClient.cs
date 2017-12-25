using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol._091;

namespace Test.It.With.Amqp.MessageClient
{
    internal class ProtocolHeaderClient : ITypedMessageClient<ProtocolHeaderFrame, Frame>, IChainableTypedMessageClient<ReceivedEventArgs, Frame>
    {
        private readonly INetworkClient _client;

        public ProtocolHeaderClient(INetworkClient networkClient, IProtocol protocol)
        {
            _client = networkClient;
            _client.BufferReceived += (sender, args) =>
            {
                var reader = new AmqpReader(args.Buffer);
                if (reader.PeekByte() == 'A')
                {
                    var header = protocol.GetProtocolHeader(reader);

                    if (Received == null)
                    {
                        throw new InvalidOperationException($"Missing subscription on {header.GetType().FullName}.");
                    }

                    Received.Invoke(new ProtocolHeaderFrame(0, header));
                }
                else
                {
                    if (Next == null)
                    {
                        throw new InvalidOperationException($"Missing handler for {typeof(ReceivedEventArgs)}.");
                    }

                    Next.Invoke(args);
                }
            };

            networkClient.Disconnected += (sender, args) =>
            {
                Disconnected?.Invoke();
            };
        }

        public event Action<ProtocolHeaderFrame> Received;
        public event Action<ReceivedEventArgs> Next;
        public event Action Disconnected;

        public void Send(Frame frame)
        {
            _client.Send(frame);
        }
    }
}