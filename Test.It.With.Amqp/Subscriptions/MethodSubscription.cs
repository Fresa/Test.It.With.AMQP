using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal class MethodSubscription<T> : BaseSubscription<MethodFrame>, IMethodSubscription
    {
        public override Type Id => typeof(T);

        public new IMethodSubscription Add(Action<ConnectionId, MethodFrame> subscription)
        {
            base.Add(subscription);
            return this;
        }
    }
}