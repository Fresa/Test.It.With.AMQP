using System;
using System.Linq;

namespace Test.It.With.Amqp.Expectations
{
    internal class MethodExpectation : Expectation
    {
        public MethodExpectation(params Type[] methods)
        {
            MethodResponses = methods.Distinct().ToArray();
        }

        public Type[] MethodResponses { get; }
    }
}