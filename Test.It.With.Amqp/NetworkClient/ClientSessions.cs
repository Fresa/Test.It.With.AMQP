using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.NetworkClient
{
    internal sealed class ClientSessions : IAsyncDisposable
    {
        private readonly ConcurrentDictionary<ConnectionId, Func<CancellationToken, ValueTask>> _sessions;
        private readonly IAsyncDisposable _stop;

        public ClientSessions(ConcurrentDictionary<ConnectionId, Func<CancellationToken, ValueTask>> sessions, IAsyncDisposable stop)
        {
            _sessions = sessions;
            _stop = stop;
        }

        internal ValueTask DisconnectAsync(ConnectionId connectionId, CancellationToken cancellationToken = default) => 
            _sessions.TryRemove(connectionId, out var disconnectAsync) ? disconnectAsync(cancellationToken) : default;

        public ValueTask DisposeAsync()
        {
            return _stop.DisposeAsync();
        }
    }
}