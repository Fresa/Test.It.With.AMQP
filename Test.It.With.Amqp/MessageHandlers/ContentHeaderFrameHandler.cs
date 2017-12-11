using System;
using System.Collections.Concurrent;
using System.Linq;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class ContentHeaderFrameHandler : IHandle<ContentHeaderFrame>, IPublishContentHeader
    {
        private readonly ConcurrentDictionary<Guid, Subscriber<ContentHeaderFrame<IContentHeader>>> _subscriptions =
            new ConcurrentDictionary<Guid, Subscriber<ContentHeaderFrame<IContentHeader>>>();

        public IDisposable Subscribe<TContentHeader>(Action<ContentHeaderFrame<TContentHeader>> subscription) 
            where TContentHeader : class, IContentHeader
        {
            var subscriptionId = Guid.NewGuid();

            _subscriptions.TryAdd(subscriptionId,
                new TypeSubscriber<TContentHeader, ContentHeaderFrame<IContentHeader>>(
                    frame => subscription(
                        new ContentHeaderFrame<TContentHeader>(frame.Channel, (TContentHeader)frame.ContentHeader))));
                
            return new Unsubscriber(() => _subscriptions.TryRemove(subscriptionId, out _));
        }

        public void Handle(ContentHeaderFrame contentHeaderFrame)
        {
            var subscriptions = _subscriptions
                .Where(pair => pair.Value.Id == contentHeaderFrame.ContentHeader.GetType())
                .Select(pair => pair.Value.Subscription)
                .ToList();

            if (subscriptions.IsEmpty())
            {
                throw new InvalidOperationException(
                    $"There are no subscriptions on {contentHeaderFrame.ContentHeader.GetType().FullName}.");
            }

            foreach (var subscription in subscriptions)
            {
                subscription(new ContentHeaderFrame<IContentHeader>(contentHeaderFrame.Channel,
                    contentHeaderFrame.ContentHeader));
            }
        }
    }
}