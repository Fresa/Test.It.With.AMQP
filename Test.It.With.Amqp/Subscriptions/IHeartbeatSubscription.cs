using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IHeartbeatSubscription : IBaseSubscription<HeartbeatFrame>
    {
        IHeartbeatSubscription Add(Action<ConnectionId, HeartbeatFrame> subscription);
    }
}