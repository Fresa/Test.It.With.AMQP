using System;

namespace Test.It.MessageClient
{    
    public interface ITypedMessageClient<TMessageReceive>
    {
        event EventHandler<TMessageReceive> Received;
        event EventHandler Disconnected;
        void Send<TMessage>(TMessage message);
    }

    public interface ITypedMessageClient<TReceive, in TSend>
    {
        event EventHandler<TReceive> Received;
        event EventHandler Disconnected;
        void Send(TSend message);
    }
}