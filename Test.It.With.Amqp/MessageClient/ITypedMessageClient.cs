using System;

namespace Test.It.With.Amqp.MessageClient
{    
    internal interface ITypedMessageClient<out TReceive, in TSend> : ISender<TSend>, IReceiver<TReceive>
    {
        event Action Disconnected;
    }
}