using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IPublishMethod
    {
        IDisposable Subscribe<TMethod>(Action<MethodFrame<TMethod>> subscription)
            where TMethod : class, IMethod;

        IDisposable Subscribe(Type type, Action<MethodFrame> subscription);
    }
}