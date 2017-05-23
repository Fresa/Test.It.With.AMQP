using System;

namespace Test.It.With.RabbitMQ.MessageClient
{
    internal class TypedMessageClient : ITypedMessageClient
    {
        private readonly IMessageClient _messageClient;
        private readonly ISerializer _serializer;

        public TypedMessageClient(IMessageClient messageClient, ISerializer serializer)
        {
            _messageClient = messageClient;
            _serializer = serializer;

            _messageClient.BufferReceived += (sender, envelope) =>
            {
                var message = _serializer.Deserialize(envelope.Type, envelope.Message);
                BufferReceived?.Invoke(this, message);
            };

            _messageClient.Disconnected += Disconnected;
        }

        public event EventHandler<object> BufferReceived;
        public event EventHandler Disconnected;

        public void Send<TMessage>(TMessage message)
        {
            var envelope = new MessageClient.MessageEnvelope(typeof(TMessage), _serializer.Serialize(message));
            _messageClient.Send(envelope);
        }
    }

    internal class TypedMessageClient<TMessageReceive> : ITypedMessageClient<TMessageReceive>
    {
        private readonly IMessageClient _messageClient;
        private readonly ISerializer _serializer;

        public TypedMessageClient(IMessageClient messageClient, ISerializer serializer)
        {
            _messageClient = messageClient;
            _serializer = serializer;

            _messageClient.BufferReceived += (sender, envelope) =>
            {
                if (envelope.Type == typeof(TMessageReceive))
                {
                    var message = (TMessageReceive)_serializer.Deserialize(envelope.Type, envelope.Message);
                    BufferReceived?.Invoke(this, message);
                }
            };

            _messageClient.Disconnected += Disconnected;
        }

        public event EventHandler<TMessageReceive> BufferReceived;
        public event EventHandler Disconnected;

        public void Send<TMessage>(TMessage message)
        {
            var envelope = new MessageClient.MessageEnvelope(typeof(TMessage), _serializer.Serialize(message));
            _messageClient.Send(envelope);
        }
    }
}