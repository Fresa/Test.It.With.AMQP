using System;

namespace Test.It.With.RabbitMQ.MessageClient
{
    internal interface IChainableTypedMessageClient<TReceive, in TSend>
    {
        event EventHandler<TReceive> Next;
        event EventHandler Disconnected;
        void Send(TSend frame);
    }
}