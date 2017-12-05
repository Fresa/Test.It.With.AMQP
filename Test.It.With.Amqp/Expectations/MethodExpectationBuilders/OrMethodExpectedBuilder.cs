using System;
using System.Collections.Generic;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Expectations.MethodExpectationBuilders
{
    internal class OrMethodExpectedBuilder : ExpectedMethodBuilder
    {
        private readonly MethodExpectationBuilder _builder;
        private readonly List<Type> _methods;

        public OrMethodExpectedBuilder(MethodExpectationBuilder builder, List<Type> methods)
        {
            _builder = builder;
            _methods = methods;
        }

        public OrMethodExpectedBuilder Or<TClient>() where TClient : IClientMethod
        {
            _methods.Add(typeof(TClient));
            return this;
        }

        public NextExpectedMethodBuilder When<TClient>() where TClient : IClientMethod
        {
            return _builder.When<TClient>();
        }

        public ExpectedMethodManager Manager => new ExpectedMethodManager(_builder);

        public override Type[] Types => _methods.ToArray();
    }
}