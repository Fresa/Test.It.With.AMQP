using System;
using System.Collections.Generic;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Expectations.MethodExpectationBuilders
{
    internal class MethodExpectationBuilder
    {
        private readonly Dictionary<Type, ExpectedMethodBuilder> _expectations = new Dictionary<Type, ExpectedMethodBuilder>();

        public IReadOnlyDictionary<Type, ExpectedMethodBuilder> Expectations => _expectations;

        public NextExpectedMethodBuilder WhenProtocolHeader()
        {
            var expecting = new NextExpectedMethodBuilder(this);
            _expectations.Add(typeof(IProtocolHeader), expecting);
            return expecting;
        }

        public NextExpectedMethodBuilder When<TClient>() where TClient : IClientMethod
        {
            var expecting = new NextExpectedMethodBuilder(this);
            _expectations.Add(typeof(TClient), expecting);
            return expecting;
        }
    }
}