using System;

namespace Test.It.With.RabbitMQ.MessageClient
{    
    public interface ITypedMessageClient<TReceive, in TSend>
    {
        event EventHandler<TReceive> Received;
        event EventHandler Disconnected;
        void Send(TSend frame);
    }
}