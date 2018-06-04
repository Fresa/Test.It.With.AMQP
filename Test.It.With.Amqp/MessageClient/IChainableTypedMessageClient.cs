using System;

namespace Test.It.With.Amqp.MessageClient
{
    internal interface IChainableTypedMessageClient<out TReceive, in TSend> : ISender<TSend>
    {
        event Action<TReceive> Next;
    }
}