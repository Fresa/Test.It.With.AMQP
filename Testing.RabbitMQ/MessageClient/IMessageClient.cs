using System;

namespace Test.It.With.RabbitMQ.MessageClient
{
    public interface IMessageClient : IDisposable
    {
        void Send(MessageEnvelope envelope);
        event EventHandler<MessageEnvelope> BufferReceived;
        event EventHandler Disconnected;
    }
}