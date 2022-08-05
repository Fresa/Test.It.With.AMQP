using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.NetworkClient
{
    internal sealed class ClientSessions : IAsyncDisposable
    {
        private readonly ConcurrentDictionary<ConnectionId, DisconnectSessionAsync> _sessions;
        private readonly IAsyncDisposable _stop;

        public ClientSessions(ConcurrentDictionary<ConnectionId, DisconnectSessionAsync> sessions, IAsyncDisposable stop)
        {
            _sessions = sessions;
            _stop = stop;
        }

        internal ValueTask DisconnectAsync(ConnectionId connectionId, CancellationToken cancellationToken = default) => 
            _sessions.TryRemove(connectionId, out var disconnectSessionAsync) ? disconnectSessionAsync(cancellationToken) : default;

        public ValueTask DisposeAsync()
        {
            return _stop.DisposeAsync();
        }
    }
}