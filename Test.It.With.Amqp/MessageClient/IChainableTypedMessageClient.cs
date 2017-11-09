using System;

namespace Test.It.With.Amqp.MessageClient
{
    internal interface IChainableTypedMessageClient<TReceive, in TSend>
    {
        event EventHandler<TReceive> Next;
        event EventHandler Disconnected;
        void Send(TSend frame);
    }
}