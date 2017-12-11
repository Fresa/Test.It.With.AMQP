using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IPublishHeartbeat
    {
        IDisposable Subscribe<THeartbeat>(Action<HeartbeatFrame<THeartbeat>> subscription)
            where THeartbeat : class, IHeartbeat;
    }
}