using System;

namespace Test.It.With.Amqp.Protocol
{
    public interface IMethod : IMessage
    {
        int ProtocolClassId { get; }
        int ProtocolMethodId { get; }
        Type[] Responses();
    }
}