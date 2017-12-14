using System;
using System.Collections.Concurrent;
using Log.It;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class ProtocolHeaderHandler : IHandle<ProtocolHeader>, IPublish<ProtocolHeader>
    {
        private readonly ILogger _logger = LogFactory.Create<ContentBodyFrameHandler>();
        private readonly ConcurrentDictionary<Guid, Action<ProtocolHeader>> _subscribers = new ConcurrentDictionary<Guid, Action<ProtocolHeader>>();

        public IDisposable Subscribe(Action<ProtocolHeader> subscription)
        {
            var subscriptionId = Guid.NewGuid();

            _subscribers.TryAdd(subscriptionId, subscription);
            
            return new Unsubscriber(() => _subscribers.TryRemove(subscriptionId, out _));
        }

        public void Handle(ProtocolHeader protocolHeader)
        {
            if (_subscribers.IsEmpty)
            {
                throw new InvalidOperationException(
                    $"There are no subscribers that can handle {typeof(ProtocolHeader).FullName}.");
            }

            _logger.Debug($"Received protocol header. {protocolHeader.Serialize()}");
            foreach (var subscription in _subscribers.Values)
            {
                subscription(protocolHeader);
            }
        }
    }
}