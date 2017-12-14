using System;
using System.Collections.Concurrent;
using Log.It;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class ContentBodyFrameHandler : IHandle<ContentBodyFrame>, IPublish<ContentBodyFrame>
    {
        private readonly ILogger _logger = LogFactory.Create<ContentBodyFrameHandler>();
        private readonly ConcurrentDictionary<Guid, Action<ContentBodyFrame>> _subscriptions =
            new ConcurrentDictionary<Guid, Action<ContentBodyFrame>>();

        public IDisposable Subscribe(Action<ContentBodyFrame> subscription)
        {
            var subscriptionId = Guid.NewGuid();

            _subscriptions.TryAdd(subscriptionId, subscription);

            return new Unsubscriber(() => _subscriptions.TryRemove(subscriptionId, out _));
        }

        public void Handle(ContentBodyFrame frame)
        {
            if (_subscriptions.IsEmpty)
            {
                throw new InvalidOperationException(
                    $"There are no subscribers that can handle {typeof(ContentBodyFrame).FullName}.");
            }

            _logger.Debug($"Received content header {frame.ContentBody.GetType().GetPrettyFullName()} on channel {frame.Channel}. {frame.ContentBody.Serialize()}");
            foreach (var subscription in _subscriptions.Values)
            {
                subscription(frame);
            }
        }
    }
}