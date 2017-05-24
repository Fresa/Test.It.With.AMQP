using System;

namespace Test.It.MessageClient
{
    internal abstract class BaseTypedMessageClient<TMessageReceive>
    {
        private readonly IMessageClient _messageClient;
        private readonly ISerializer _serializer;

        protected BaseTypedMessageClient(IMessageClient messageClient, ISerializer serializer)
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

        protected event EventHandler<TMessageReceive> BufferReceived;
        protected event EventHandler Disconnected;

        protected void Send<TMessage>(TMessage message)
        {
            var envelope = new MessageEnvelope(typeof(TMessage), _serializer.Serialize(message));
            _messageClient.Send(envelope);
        }
    }
}