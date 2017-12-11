using System;
using System.Collections.Concurrent;
using System.Linq;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class HeartbeatFrameHandler : IHandle<HeartbeatFrame>, IPublishHeartbeat
    {
        private readonly ConcurrentDictionary<Guid, Subscriber<HeartbeatFrame<IHeartbeat>>> _subscriptions =
            new ConcurrentDictionary<Guid, Subscriber<HeartbeatFrame<IHeartbeat>>>();

        public IDisposable Subscribe<THeartbeat>(Action<HeartbeatFrame<THeartbeat>> subscription) 
            where THeartbeat : class, IHeartbeat
        {
            var subscriptionId = Guid.NewGuid();

            _subscriptions.TryAdd(subscriptionId,
                new TypeSubscriber<THeartbeat, HeartbeatFrame<IHeartbeat>>(
                    frame => subscription(
                        new HeartbeatFrame<THeartbeat>(frame.Channel, (THeartbeat)frame.Heartbeat))));

            return new Unsubscriber(() => _subscriptions.TryRemove(subscriptionId, out _));
        }

        public void Handle(HeartbeatFrame contentHeaderFrame)
        {
            var subscriptions = _subscriptions
                .Where(pair => pair.Value.Id == contentHeaderFrame.Heartbeat.GetType())
                .Select(pair => pair.Value.Subscription)
                .ToList();

            if (subscriptions.IsEmpty())
            {
                throw new InvalidOperationException(
                    $"There are no subscriptions on {contentHeaderFrame.Heartbeat.GetType().FullName}.");
            }

            foreach (var subscription in subscriptions)
            {
                subscription(new HeartbeatFrame<IHeartbeat>(contentHeaderFrame.Channel,
                    contentHeaderFrame.Heartbeat));
            }
        }
    }
}