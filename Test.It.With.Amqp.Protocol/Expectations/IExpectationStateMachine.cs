namespace Test.It.With.Amqp.Protocol.Expectations
{
    public interface IExpectationStateMachine
    {
        bool ShouldPass(int channel, IProtocolHeader protocolHeader);

        bool ShouldPass(int channel, IMethod method);

        bool ShouldPass(int channel, IContentHeader contentHeader, out IContentMethod method);

        bool ShouldPass(int channel, IContentBody contentBody, out IContentMethod method);

        bool ShouldPass(int channel, IHeartbeat method);
    }
}