using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IPublishHeartbeat
    {
        IDisposable Subscribe(Type type, Action<HeartbeatFrame> subscription);
    }
}