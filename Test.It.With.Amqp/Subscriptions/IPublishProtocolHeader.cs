using System;
using Test.It.With.Amqp.Messages;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Subscriptions
{
    internal interface IPublishProtocolHeader
    {
        IDisposable Subscribe<TProtocolHeader>(Action<ProtocolHeaderFrame<TProtocolHeader>> subscription)
            where TProtocolHeader : class, IProtocolHeader;
    }
}