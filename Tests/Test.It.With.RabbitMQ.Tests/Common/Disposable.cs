using System;

namespace Test.It.With.RabbitMQ.Tests.Common
{
    public class Disposable : IDisposable
    {
        private readonly Action _action;

        public Disposable(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}