using System;

namespace Test.It.With.RabbitMQ.MessageClient
{
    public class MessageEnvelope
    {
        public MessageEnvelope(Type type, byte[] message)
        {
            Type = type;
            Message = message;
        }

        public Type Type { get; }
        public byte[] Message { get; }
    }
}