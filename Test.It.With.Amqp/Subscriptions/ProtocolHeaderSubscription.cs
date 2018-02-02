using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal class ProtocolHeaderSubscription<T> : BaseSubscription<ProtocolHeaderFrame>, IProtocolHeaderSubscription
    {
        public override Type Id => typeof(T);

        public new IProtocolHeaderSubscription Add(Action<ConnectionId, ProtocolHeaderFrame> subscription)
        {
            base.Add(subscription);
            return this;
        }
    }
}