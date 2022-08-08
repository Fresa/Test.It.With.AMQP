using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.NetworkClient
{
    public interface IServer : IAsyncDisposable
    {
        int Port { get; }

        IPAddress Address { get; }

        /// <summary>
        /// Disconnects a connection
        /// </summary>
        /// <param name="connectionId">Connection id</param>
        /// <param name="cancellation">Cancel waiting for disconnection</param>
        ValueTask DisconnectAsync(ConnectionId connectionId, CancellationToken cancellation = default);
    }
}