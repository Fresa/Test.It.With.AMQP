using System;

namespace Test.It.With.Amqp.Subscriptions
{
    internal abstract class BaseSubscription<TSubscription>
    {
        protected BaseSubscription(Type id, TSubscription subscription)
        {
            Id = id;
            Subscription = subscription;
        }

        public Type Id { get; }

        public TSubscription Subscription { get; }
    }
}