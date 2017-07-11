using System;
using Test.It.With.RabbitMQ.NetworkClient;

namespace Test.It.With.RabbitMQ.MessageClient
{
    public class MessageClient : IMessageClient
    {
        private readonly INetworkClient _networkClient;
        private readonly ISerializer _serializer;

        public MessageClient(INetworkClient networkClient, ISerializer serializer)
        {
            _networkClient = networkClient;
            _serializer = serializer;

            _networkClient.BufferReceived += (sender, args) =>
            {
                var message = _serializer.Deserialize<MessageEnvelope>(args.Buffer);
                BufferReceived?.Invoke(this, message);
            };
            _networkClient.Disconnected += Disconnected;
        }

        public event EventHandler<MessageEnvelope> BufferReceived;
        public event EventHandler Disconnected;

        public void Send(MessageEnvelope envelope)
        {
            var bytes = _serializer.Serialize(envelope);
            _networkClient.Send(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            _networkClient.Dispose();
        }
    }
}