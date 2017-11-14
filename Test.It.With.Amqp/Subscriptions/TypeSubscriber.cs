using System;

namespace Test.It.With.Amqp.Subscriptions
{
    internal class TypeSubscriber<TKey, TValue> : Subscriber<TValue>
    {
        public TypeSubscriber(Action<TValue> subscription) : base(typeof(TKey), subscription)
        {
        }
    }
}