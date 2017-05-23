using System;

namespace Test.It.With.RabbitMQ.MessageClient
{
    internal interface ITypedMessageClient
    {
        event EventHandler<object> BufferReceived;
        event EventHandler Disconnected;
        void Send<TMessage>(TMessage message);
    }

    internal interface ITypedMessageClient<TMessageReceive>
    {
        event EventHandler<TMessageReceive> BufferReceived;
        event EventHandler Disconnected;
        void Send<TMessage>(TMessage message);
    }
}