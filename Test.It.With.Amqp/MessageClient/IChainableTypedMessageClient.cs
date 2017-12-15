using System;

namespace Test.It.With.Amqp.MessageClient
{
    internal interface IChainableTypedMessageClient<out TReceive, in TSend>
    {
        event Action<TReceive> Next;
        event Action Disconnected;
        void Send(TSend frame);
    }
}