using System;

namespace Test.It.With.Amqp.Protocol._091.Expectations.MethodExpectationBuilders
{
    internal abstract class ExpectedMethodBuilder
    {
        public abstract Type[] Types { get; }
    }
}