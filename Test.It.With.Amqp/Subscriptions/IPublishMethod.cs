using System;
using Test.It.With.Amqp.Messages;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IPublishMethod
    {
        IDisposable Subscribe(Type type, Action<MethodFrame> subscription);
    }
}