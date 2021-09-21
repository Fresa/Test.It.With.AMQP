using System;
using System.Collections.Concurrent;
using System.Linq;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.Logging;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class HeartbeatFrameHandler : IHandle<HeartbeatFrame>, IPublishHeartbeat
    {
        private readonly InternalLogger _logger = LogFactory.Create<HeartbeatFrameHandler>();
        private readonly ConcurrentDictionary<Guid, Subscriber<HeartbeatFrame<IHeartbeat>>> _subscriptions =
            new ConcurrentDictionary<Guid, Subscriber<HeartbeatFrame<IHeartbeat>>>();

        public IDisposable Subscribe(Type type, Action<HeartbeatFrame> subscription)
        {
            var subscriptionId = Guid.NewGuid();

            _subscriptions.TryAdd(subscriptionId,
                new TypeSubscriber<HeartbeatFrame<IHeartbeat>>(type,
                    frame => subscription(
                        new HeartbeatFrame(frame.Channel, frame.Message))));

            return new Unsubscriber(() => _subscriptions.TryRemove(subscriptionId, out _));
        }

        public void Handle(HeartbeatFrame heartbeatFrame)
        {
            var subscriptions = _subscriptions
                .Where(pair => pair.Value.Id == heartbeatFrame.Message.GetType())
                .Select(pair => pair.Value.Subscription)
                .ToList();

            if (subscriptions.IsEmpty())
            {
                throw new InvalidOperationException(
                    $"There are no subscriptions on {heartbeatFrame.Message.GetType().FullName}.");
            }

            _logger.Debug("Received heartbeat {MessageName}. {@Message}", heartbeatFrame.Message.GetType().GetPrettyFullName(), heartbeatFrame.Message);
            foreach (var subscription in subscriptions)
            {
                subscription(new HeartbeatFrame<IHeartbeat>(heartbeatFrame.Channel,
                    heartbeatFrame.Message));
            }
        }
    }
}