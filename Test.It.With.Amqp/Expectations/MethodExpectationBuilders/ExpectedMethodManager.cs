using System;

namespace Test.It.With.Amqp.Expectations.MethodExpectationBuilders
{
    internal class ExpectedMethodManager
    {
        private readonly MethodExpectationBuilder _methodExpectationBuilder;

        public ExpectedMethodManager(MethodExpectationBuilder methodExpectationBuilder)
        {
            _methodExpectationBuilder = methodExpectationBuilder;
        }

        public Type[] GetExpectingMethodsFor<TMethod>()
        {
            if (_methodExpectationBuilder.Expectations.ContainsKey(typeof(TMethod)))
            {
                return _methodExpectationBuilder.Expectations[typeof(TMethod)].Types;
            }

            return Array.Empty<Type>();
        }
    }
}