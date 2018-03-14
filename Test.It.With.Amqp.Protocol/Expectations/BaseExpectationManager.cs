using System.Collections.Concurrent;

namespace Test.It.With.Amqp.Protocol.Expectations
{
    internal abstract class BaseExpectationManager
    {
        private readonly ConcurrentDictionary<int, Expectation> _expectations = new ConcurrentDictionary<int, Expectation>();

        protected abstract Expectation Create(int channel);
        protected abstract void ThrowUnexpectedFrameException(string message);

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
                ThrowUnexpectedFrameException(
                    $"Expected {expectation.GetType().Name}, got {typeof(TExpectation).Name}.");
            }

            return (TExpectation)expectation;
        }

        public void Set(int channel, Expectation expectation)
        {
            _expectations[channel] = expectation;
        }
    }
}