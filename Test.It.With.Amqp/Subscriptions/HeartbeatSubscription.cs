using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal class HeartbeatSubscription<T> : BaseSubscription<HeartbeatFrame>, IHeartbeatSubscription
    {
        public override Type Id => typeof(T);

        public new IHeartbeatSubscription Add(Action<ConnectionId, HeartbeatFrame> subscription)
        {
            base.Add(subscription);
            return this;
        }
    }
}