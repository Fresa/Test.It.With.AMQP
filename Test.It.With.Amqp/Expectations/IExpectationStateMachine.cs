using System;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Expectations
{
    internal interface IExpectationStateMachine
    {
        bool ShouldPass(int channel, IProtocolHeader protocolHeader);

        bool ShouldPass(int channel, IMethod method);

        bool ShouldPass(int channel, IContentHeader contentHeader, Type type, out IContentMethod method);

        bool ShouldPass(int channel, IContentBody contentBody, Type type, out IContentMethod method);

        bool ShouldPass(int channel, IHeartbeat method);
    }
}