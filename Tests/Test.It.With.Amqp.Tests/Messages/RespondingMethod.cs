using System;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Tests.Messages
{
    public class RespondingMethod : IMethod
    {
        public bool SentOnValidChannel(int channel)
        {
            throw new NotImplementedException();
        }

        public void ReadFrom(IAmqpReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(IAmqpWriter writer)
        {
            throw new NotImplementedException();
        }

        public int ProtocolClassId { get; }
        public int ProtocolMethodId { get; }
        public Type[] Responses()
        {
            throw new NotImplementedException();
        }
    }
}