using System.Collections.Concurrent;
using Test.It.With.Amqp091;

namespace Test.It.With.Amqp.Expectations
{
    internal class ExpectationManager
    {
        private readonly ConcurrentDictionary<int, Expectation> _expectations = new ConcurrentDictionary<int, Expectation>();

        private static Expectation Create(int channel)
        {
            switch (channel)
            {
                case 0:
                    return new ProtocolHeaderExpectation();
                default:
                    return new MethodExpectation(typeof(Channel.Open));
            }
        }

        public bool IsExpecting<TExpectation>(int channel) where TExpectation : Expectation
        {
            if (_expectations.TryGetValue(channel, out var expectation) == false)
            {
                expectation = Create(channel);
            }

            return expectation is TExpectation;
        }

        public TExpectation Get<TExpectation>(int channel) where TExpectation : Expectation
        {
            if (_expectations.TryGetValue(channel, out var expectation) == false)
            {
                expectation = Create(channel);
                Set(channel, expectation);
            }

            if (expectation is TExpectation == false)
            {
                // todo: need to abstract protocol specific exceptions
                throw new UnexpectedFrameException(
                    $"Expected {expectation.Name}, got {typeof(TExpectation).FullName}.");
            }

            return (TExpectation)expectation;
        }

        public void Set(int channel, Expectation expectation)
        {
            _expectations[channel] = expectation;
        }
    }
}