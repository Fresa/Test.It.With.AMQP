using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IMethodSubscription : IBaseSubscription<MethodFrame>
    {
        IMethodSubscription Add(Action<ConnectionId, MethodFrame> subscription);
    }
}