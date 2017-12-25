using System;

namespace Test.It.With.Amqp.Protocol
{
    public interface IMethod
    {
        int ProtocolClassId { get; }
        int ProtocolMethodId { get; }
        bool SentOnValidChannel(int channel);
        Type[] Responses();

        void ReadFrom(IAmqpReader reader);
        void WriteTo(IAmqpWriter writer);
    }
}