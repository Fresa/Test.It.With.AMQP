using System;
using System.Collections.Concurrent;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.Logging;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class ContentHeaderFrameHandler : IHandle<ContentHeaderFrame>, IPublish<ContentHeaderFrame>
    {
        private readonly Logger _logger = Logger.Create<ContentHeaderFrameHandler>();
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

            _logger.Debug("Received content body {MessageName}. {@Message}", frame.Message.GetType().GetPrettyFullName(), frame.Message);
            foreach (var subscription in _subscriptions.Values)
            {
                subscription(frame);
            }
        }
    }
}