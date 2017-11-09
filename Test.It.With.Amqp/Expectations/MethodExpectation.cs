using System;

namespace Test.It.With.Amqp.Expectations
{
    internal class MethodExpectation : Expectation
    {
        public MethodExpectation()
        {
            MethodResponses = Array.Empty<Type>();
        }

        public MethodExpectation(Type[] methods)
        {
            MethodResponses = methods;
        }

        public Type[] MethodResponses { get; }
    }
}