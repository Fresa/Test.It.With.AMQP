using System;
using System.IO;
using System.Linq;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.MessageClient
{
    internal class ProtocolHeaderClient : ITypedMessageClient<ProtocolHeaderFrame, IFrame>, IChainableTypedMessageClient<ReceivedEventArgs, IFrame>
    {
        private readonly INetworkClient _client;
        private readonly IAmqpWriterFactory _amqpWriterFactory;

        public ProtocolHeaderClient(INetworkClient networkClient, IProtocol protocol, IAmqpReaderFactory amqpReaderFactory, IAmqpWriterFactory amqpWriterFactory)
        {
            _client = networkClient;
            _amqpWriterFactory = amqpWriterFactory;
            _client.BufferReceived += (sender, args) =>
            {
                if (args.Count == 0)
                {
                    return;
                }

                var reader = amqpReaderFactory.Create(
                    args.Buffer
                        .Skip(args.Offset)
                        .Take(args.Count)
                        .ToArray());

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
        }

        public event Action<ProtocolHeaderFrame> Received;
        public event Action<ReceivedEventArgs> Next;

        public void Send(IFrame frame)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = _amqpWriterFactory.Create(stream))
                {
                    frame.WriteTo(writer);
                }
                var bytes = stream.ToArray();
                _client.Send(bytes, 0, bytes.Length);
            }
        }
    }
}