using System;

namespace Test.It.With.RabbitMQ.MessageClient
{
    internal interface IMessageClient : IDisposable
    {
        void Send(MessageClient.MessageEnvelope envelope);
        event EventHandler<MessageClient.MessageEnvelope> BufferReceived;
        event EventHandler Disconnected;
    }
}