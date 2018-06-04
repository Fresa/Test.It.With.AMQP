using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IPublishProtocolHeader
    {
        IDisposable Subscribe(Type type, Action<ProtocolHeaderFrame> subscription);
    }
}