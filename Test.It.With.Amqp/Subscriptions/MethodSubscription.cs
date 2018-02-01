using System;
using System.Collections.Concurrent;
using System.Linq;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IMethodSubscription
    {
        Type Id { get; }

        void Handle(ConnectionId connectionId, MethodFrame frame);

        IMethodSubscription Add(Action<ConnectionId, MethodFrame> subscription);
    }

    internal class MethodSubscription<T> : IMethodSubscription
    {
        private readonly ConcurrentDictionary<int, Action<ConnectionId, MethodFrame>> _subscriptions = new ConcurrentDictionary<int, Action<ConnectionId, MethodFrame>>();

        public Type Id => typeof(T);

        public void Handle(ConnectionId connectionId, MethodFrame frame)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Value(connectionId, frame);
            }
        }

        public IMethodSubscription Add(Action<ConnectionId, MethodFrame> subscription)
        {
            _subscriptions.TryAdd(subscription.GetHashCode(), subscription);
            return this;
        }
    }
}