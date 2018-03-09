using System;
using System.Collections.Concurrent;
using Log.It;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class ContentHeaderFrameHandler : IHandle<ContentHeaderFrame>, IPublish<ContentHeaderFrame>
    {
        private readonly ILogger _logger = LogFactory.Create<ContentHeaderFrameHandler>();
        private readonly ConcurrentDictionary<Guid, Action<ContentHeaderFrame>> _subscriptions =
            new ConcurrentDictionary<Guid, Action<ContentHeaderFrame>>();

        public IDisposable Subscribe(Action<ContentHeaderFrame> subscription)
        {
            var subscriptionId = Guid.NewGuid();

            _subscriptions.TryAdd(subscriptionId, subscription);

            return new Unsubscriber(() => _subscriptions.TryRemove(subscriptionId, out _));
        }

        public void Handle(ContentHeaderFrame frame)
        {
            if (_subscriptions.IsEmpty)
            {
                throw new InvalidOperationException(
                    $"There are no subscribers that can handle {typeof(ContentHeaderFrame).FullName}.");
            }

            _logger.Debug($"Received content body {frame.Message.GetType().GetPrettyFullName()}. {frame.Message.Serialize()}");
            foreach (var subscription in _subscriptions.Values)
            {
                subscription(frame);
            }
        }
    }
}