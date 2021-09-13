using System;

namespace Test.It.With.Amqp.Tests.System
{
    internal class DisposableActions : IDisposable
    {
        private readonly Action[] _dispose;

        internal DisposableActions(
            params Action[] dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            foreach (var dispose in _dispose)
            {
                dispose();
            }
        }
    }
}