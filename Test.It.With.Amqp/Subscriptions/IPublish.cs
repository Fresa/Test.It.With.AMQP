using System;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IPublish<out T>
    {
        IDisposable Subscribe(Action<T> subscription);
    }
}