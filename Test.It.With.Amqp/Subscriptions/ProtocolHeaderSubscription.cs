using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal class ProtocolHeaderSubscription : BaseSubscription<Action<ConnectionId, ProtocolHeaderFrame>>
    {
        public ProtocolHeaderSubscription(Type id, Action<ConnectionId, ProtocolHeaderFrame> subscription) : base(id, subscription)
        {
        }

        public static ProtocolHeaderSubscription Create<T>(Action<ConnectionId, ProtocolHeaderFrame> subscription)
        {
            return new ProtocolHeaderSubscription(typeof(T), subscription);
        }
    }
}