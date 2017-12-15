using Test.It.With.Amqp.Expectations;
using Test.It.With.Amqp.Extensions;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp
{
    internal class ProtocolResolver
    {
        public ProtocolResolver(ProtocolVersion protocolVersion)
        {
            switch (protocolVersion)
            {
                case ProtocolVersion.AMQP091:
                    Protocol = new Amq091Protocol();
                    ExpectationStateMachine = new Amqp091ExpectationStateMachine();
                    return;
            }
        }

        public IProtocol Protocol { get; }

        public IExpectationStateMachine ExpectationStateMachine { get; }
    }
}