using System;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IBaseSubscription<in T>
    {
        Type Id { get; }
        void Handle(ConnectionId connectionId, T frame);
    }
}