using System;
using System.Linq;

namespace Test.It.With.Amqp.Protocol.Expectations
{
    internal class MethodExpectation : Expectation
    {
        public MethodExpectation(params Type[] methods)
        {
            MethodResponses = methods.Distinct().ToArray();
        }

        public Type[] MethodResponses { get; }
    }

    internal class MethodExpectation<T> : MethodExpectation
    {
        public MethodExpectation() : base(typeof(T))
        {
            
        }
    }
}