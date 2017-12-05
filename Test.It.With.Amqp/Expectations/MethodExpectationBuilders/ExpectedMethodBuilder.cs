using System;

namespace Test.It.With.Amqp.Expectations.MethodExpectationBuilders
{
    internal abstract class ExpectedMethodBuilder
    {
        public abstract Type[] Types { get; }
    }
}