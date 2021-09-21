using System;
using System.Collections.Concurrent;
using System.Linq;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.Logging;
using Test.It.With.Amqp.MessageClient;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class MethodFrameHandler : IHandle<MethodFrame>, IPublishMethod
    {
        private readonly bool _automaticReplyOnMissingSubscription;
        private readonly ISender<MethodFrame> _sender;
        private static readonly InternalLogger Logger = LogFactory.Create<MethodFrameHandler>();
        private readonly ConcurrentDictionary<Guid, Subscriber<MethodFrame<IMethod>>> _methodSubscriptions = new ConcurrentDictionary<Guid, Subscriber<MethodFrame<IMethod>>>();

        public MethodFrameHandler(bool automaticReplyOnMissingSubscription, ISender<MethodFrame> sender)
        {
            _automaticReplyOnMissingSubscription = automaticReplyOnMissingSubscription;
            _sender = sender;
        }

        public IDisposable Subscribe(Type type, Action<MethodFrame> subscription)
        {
            var subscriptionId = Guid.NewGuid();

            _methodSubscriptions.TryAdd(subscriptionId,
                new TypeSubscriber<MethodFrame<IMethod>>(type,
                    frame =>
                        subscription(new MethodFrame(frame.Channel, frame.Message))));

            return new Unsubscriber(() => _methodSubscriptions.TryRemove(subscriptionId, out _));
        }

        public void Handle(MethodFrame methodFrame)
        {
            Logger.Debug("Received method {Name}. {@Message}", methodFrame.Message.GetType().GetPrettyFullName(), methodFrame.Message);

            var subscriptions = _methodSubscriptions
                .Where(pair => pair.Value.Id == methodFrame.Message.GetType())
                .Select(pair => pair.Value.Subscription)
                .ToList();

            if (subscriptions.IsEmpty())
            {
                if (_automaticReplyOnMissingSubscription && methodFrame.Message.Responses().Any())
                {
                    _sender.Send(new MethodFrame(methodFrame.Channel, (IMethod)Activator.CreateInstance(methodFrame.Message.Responses().First())));
                    return;
                }

                throw new InvalidOperationException(
                    $"There are no subscriptions on {methodFrame.Message.GetType().FullName}.");
            }

            foreach (var subscription in subscriptions)
            {
                subscription(new MethodFrame<IMethod>(methodFrame.Channel, methodFrame.Message));
            }
        }
    }
}