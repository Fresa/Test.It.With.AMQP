using System;
using System.Collections.Concurrent;

namespace Test.It.With.Amqp.Subscriptions
{
    internal abstract class BaseSubscription<T> : IBaseSubscription<T>
    {
        private readonly ConcurrentBag<Action<ConnectionId, T>> _subscriptions = new ConcurrentBag<Action<ConnectionId, T>>();

        public abstract Type Id { get; }

        public virtual void Handle(ConnectionId connectionId, T frame)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription(connectionId, frame);
            }
        }

        protected void Add(Action<ConnectionId, T> subscription)
        {
            _subscriptions.Add(subscription);
        }
    }
}