using System;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.System
{
    internal sealed class AsyncDisposableAction : IAsyncDisposable
    {
        private readonly Func<ValueTask> _onDispose = () => new ValueTask();

        public AsyncDisposableAction()
        {
        }

        public AsyncDisposableAction(Func<ValueTask> onDispose)
        {
            _onDispose = onDispose;
        }
        
        public ValueTask DisposeAsync()
        {
            return _onDispose();
        }
    }
}