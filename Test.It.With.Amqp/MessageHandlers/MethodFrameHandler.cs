using System;
using System.Collections.Concurrent;
using System.Linq;
using Log.It;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.Amqp.Subscriptions;

namespace Test.It.With.Amqp.MessageHandlers
{
    internal class MethodFrameHandler : IHandle<MethodFrame>, IPublishMethod
    {
        private readonly ILogger _logger = LogFactory.Create<MethodFrameHandler>();
        private readonly ConcurrentDictionary<Guid, Subscriber<MethodFrame<IMethod>>> _methodSubscriptions = new ConcurrentDictionary<Guid, Subscriber<MethodFrame<IMethod>>>();

        public IDisposable Subscribe<TMethod>(Action<MethodFrame<TMethod>> subscription) 
            where TMethod : class, IMethod
        {
            var subscriptionId = Guid.NewGuid();
            
            _methodSubscriptions.TryAdd(subscriptionId, 
                new TypeSubscriber<TMethod, MethodFrame<IMethod>>(
                    frame => 
                        subscription(new MethodFrame<TMethod>(frame.Channel, (TMethod) frame.Message))));
            
            return new Unsubscriber(() => _methodSubscriptions.TryRemove(subscriptionId, out _));
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
            var subscriptions = _methodSubscriptions
                .Where(pair => pair.Value.Id == methodFrame.Message.GetType())
                .Select(pair => pair.Value.Subscription)
                .ToList();

            if (subscriptions.IsEmpty())
            {
                throw new InvalidOperationException(
                    $"There are no subscriptions on {methodFrame.Message.GetType().FullName}.");
            }

            _logger.Debug($"Received method {methodFrame.Message.GetType().GetPrettyFullName()}. {methodFrame.Message.Serialize()}");
            foreach (var subscription in subscriptions)
            {
                subscription(new MethodFrame<IMethod>(methodFrame.Channel, methodFrame.Message));
            }
        }
    }
}