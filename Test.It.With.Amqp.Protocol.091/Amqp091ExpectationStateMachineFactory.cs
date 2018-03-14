using Test.It.With.Amqp.Protocol.Expectations;

namespace Test.It.With.Amqp.Protocol._091
{
    internal class Amqp091ExpectationStateMachineFactory : IExpectationStateMachineFactory
    {
        public IExpectationStateMachine Create()
        {
            return new Amqp091ExpectationStateMachine();
        }
    }
}