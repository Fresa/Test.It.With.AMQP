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
    internal class ProtocolHeaderHandler : IHandle<ProtocolHeaderFrame>, IPublishProtocolHeader
    {
        private readonly Logger _logger = Logger.Create<ProtocolHeaderHandler>();
        private readonly ConcurrentDictionary<Guid, Subscriber<ProtocolHeaderFrame<IProtocolHeader>>> _subscriptions = new ConcurrentDictionary<Guid, Subscriber<ProtocolHeaderFrame<IProtocolHeader>>>();

        public IDisposable Subscribe(Type type, Action<ProtocolHeaderFrame> subscription)
        {
            var subscriptionId = Guid.NewGuid();

            _subscriptions.TryAdd(subscriptionId,
                new TypeSubscriber<ProtocolHeaderFrame<IProtocolHeader>>(type,
                    frame =>
                        subscription(new ProtocolHeaderFrame(frame.Channel, frame.Message))));

            return new Unsubscriber(() => _subscriptions.TryRemove(subscriptionId, out _));
        }

        public void Handle(ProtocolHeaderFrame protocolHeader)
        {
            var subscriptions = _subscriptions
                .Where(pair => pair.Value.Id == protocolHeader.Message.GetType())
                .Select(pair => pair.Value.Subscription)
                .ToList();

            if (subscriptions.IsEmpty())
            {
                throw new InvalidOperationException(
                    $"There are no subscriptions on {protocolHeader.Message.GetType().FullName}.");
            }

            _logger.Debug("Received method {MessageName}. {@Message}", protocolHeader.Message.GetType().GetPrettyFullName(), protocolHeader.Message);
            foreach (var subscription in subscriptions)
            {
                subscription(new ProtocolHeaderFrame<IProtocolHeader>(protocolHeader.Channel, protocolHeader.Message));
            }
        }
    }
}