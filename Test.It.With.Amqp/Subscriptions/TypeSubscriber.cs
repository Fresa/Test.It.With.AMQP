using System;

namespace Test.It.With.Amqp.Subscriptions
{
    internal class TypeSubscriber<TKey, TValue> : Subscriber<TValue>
    {
        public TypeSubscriber(Action<TValue> subscription) : base(typeof(TKey), subscription)
        {
        }
    }

    internal class TypeSubscriber<TValue> : Subscriber<TValue>
    {
        public TypeSubscriber(Type key, Action<TValue> subscription) : base(key, subscription)
        {
        }
    }
}