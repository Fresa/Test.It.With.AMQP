using System;
using System.Collections.Concurrent;
using System.Linq;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class MethodFrameHandler : IHandle<MethodFrame>, IPublishMethod
    {
        private readonly ConcurrentDictionary<Guid, Subscriber<MethodFrame<IMethod>>> _methodSubscriptions = new ConcurrentDictionary<Guid, Subscriber<MethodFrame<IMethod>>>();

        public IDisposable Subscribe<TMethod>(Action<MethodFrame<TMethod>> subscription) 
            where TMethod : class, IMethod
        {
            var subscriptionId = Guid.NewGuid();
            
            _methodSubscriptions.TryAdd(subscriptionId, 
                new TypeSubscriber<TMethod, MethodFrame<IMethod>>(
                    frame => 
                        subscription(new MethodFrame<TMethod>(frame.Channel, (TMethod) frame.Method))));
            
            return new Unsubscriber(() => _methodSubscriptions.TryRemove(subscriptionId, out _));
        }

        public void Handle(MethodFrame methodFrame)
        {
            var subsciptions = _methodSubscriptions
                .Where(pair => pair.Value.Id == methodFrame.Method.GetType())
                .Select(pair => pair.Value.Subscription)
                .ToList();

            if (subsciptions.IsEmpty())
            {
                throw new InvalidOperationException(
                    $"There is no subscriptions on {methodFrame.Method.GetType().FullName}.");
            }

            foreach (var subscription in subsciptions)
            {
                subscription(new MethodFrame<IMethod>(methodFrame.Channel, methodFrame.Method));
            }
        }
    }
}