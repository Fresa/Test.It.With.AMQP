using System;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Expectations.MethodExpectationBuilders
{
    internal class ExpectedMethodManager
    {
        private readonly MethodExpectationBuilder _methodExpectationBuilder;

        public ExpectedMethodManager(MethodExpectationBuilder methodExpectationBuilder)
        {
            _methodExpectationBuilder = methodExpectationBuilder;
        }

        public Type[] GetExpectingMethodsFor(Type type)
        {
            if (_methodExpectationBuilder.Expectations.ContainsKey(type))
            {
                return _methodExpectationBuilder.Expectations[type].Types;
            }

            return Array.Empty<Type>();
        }

        public Type[] GetExpectingMethodsFor<TMethod>()
        {
            return GetExpectingMethodsFor(typeof(TMethod));
        }
    }
}