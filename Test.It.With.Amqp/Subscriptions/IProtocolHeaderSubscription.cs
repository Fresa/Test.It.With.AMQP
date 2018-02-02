using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IProtocolHeaderSubscription : IBaseSubscription<ProtocolHeaderFrame>
    {
        IProtocolHeaderSubscription Add(Action<ConnectionId, ProtocolHeaderFrame> subscription);
    }
}