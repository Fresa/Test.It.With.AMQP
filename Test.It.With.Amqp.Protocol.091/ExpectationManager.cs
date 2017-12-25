using Test.It.With.Amqp.Protocol.Expectations;

namespace Test.It.With.Amqp.Protocol._091
{
    internal class ExpectationManager : BaseExpectationManager
    {
        protected override Expectation Create(int channel)
        {
            switch (channel)
            {
                case 0:
                    return new ProtocolHeaderExpectation();
                default:
                    return new MethodExpectation<Channel.Open>();
            }
        }

        protected override void ThrowUnexpectedFrameException(string message)
        {
            throw new UnexpectedFrameException(message);
        }
    }
}