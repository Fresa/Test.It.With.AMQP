using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Expectations
{
    internal interface IExpectationStateMachine
    {
        bool ShouldPass(ProtocolHeader protocolHeader);

        bool ShouldPass<TMethod>(int channel, TMethod method)
            where TMethod : IClientMethod;

        bool ShouldPass<TMethod>(int channel, IContentHeader contentHeader, out TMethod method)
            where TMethod : IClientMethod, IContentMethod;

        bool ShouldPass<TMethod>(int channel, IContentBody contentBody, out TMethod method)
            where TMethod : IClientMethod, IContentMethod;

        bool ShouldPass(int channel, IHeartbeat method);
    }
}