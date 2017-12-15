using System;

namespace Test.It.With.Amqp.MessageClient
{    
    public interface ITypedMessageClient<out TReceive, in TSend>
    {
        event Action<TReceive> Received;
        event Action Disconnected;
        void Send(TSend frame);
    }
}