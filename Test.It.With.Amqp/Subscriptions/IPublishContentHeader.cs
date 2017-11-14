using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IPublishContentHeader
    {
        IDisposable Subscribe<TContentHeader>(Action<ContentHeaderFrame<TContentHeader>> subscription)
            where TContentHeader : IContentHeader;
    }
}