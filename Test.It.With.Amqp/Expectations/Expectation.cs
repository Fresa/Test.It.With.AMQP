using Test.It.With.Amqp.Protocol.Extensions;

namespace Test.It.With.Amqp.Expectations
{
    internal abstract class Expectation
    {
        public string Name => GetType().Name.SplitOnUpperCase().Join(" ").ToLower();
    }
}