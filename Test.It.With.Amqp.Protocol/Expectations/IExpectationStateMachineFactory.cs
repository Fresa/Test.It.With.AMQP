namespace Test.It.With.Amqp.Protocol.Expectations
{
    public interface IExpectationStateMachineFactory
    {
        IExpectationStateMachine Create();
    }
}