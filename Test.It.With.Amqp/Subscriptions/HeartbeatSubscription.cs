using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal class HeartbeatSubscription : BaseSubscription<Action<ConnectionId, HeartbeatFrame>>
    {
        public HeartbeatSubscription(Type id, Action<ConnectionId, HeartbeatFrame> subscription) : base(id, subscription)
        {
        }

        public static HeartbeatSubscription Create<T>(Action<ConnectionId, HeartbeatFrame> subscription)
        {
            return new HeartbeatSubscription(typeof(T), subscription);
        }
    }
}