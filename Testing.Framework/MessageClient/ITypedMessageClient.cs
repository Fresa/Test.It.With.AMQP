using System;

namespace Test.It.MessageClient
{    
    public interface ITypedMessageClient<TMessageReceive>
    {
        event EventHandler<TMessageReceive> BufferReceived;
        event EventHandler Disconnected;
        void Send<TMessage>(TMessage message);
    }

    public interface ITypedMessageClient<TMessageReceive, in TMessageSend>
    {
        event EventHandler<TMessageReceive> BufferReceived;
        event EventHandler Disconnected;
        void Send(TMessageSend message);
    }
}