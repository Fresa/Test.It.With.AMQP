using System;

namespace Test.It.With.Amqp.Subscriptions
{
    internal abstract class Subscriber<TValue>
    {
        protected Subscriber(Type id, Action<TValue> subscription)
        {
            Id = id;
            Subscription = subscription;
        }

        public Type Id { get; }
        public Action<TValue> Subscription { get; }
    }
}