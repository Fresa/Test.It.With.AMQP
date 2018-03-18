using System;

namespace Test.It.With.Amqp.MessageClient
{
    internal interface IReceiver<out TMessage>
    {
        event Action<TMessage> Received;
    }
}