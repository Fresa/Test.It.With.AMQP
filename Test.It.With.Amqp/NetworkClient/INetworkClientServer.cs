using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.NetworkClient
{
    internal interface INetworkClientServer : IAsyncDisposable
    {
        Task<IStartableNetworkClient> WaitForConnectedClientAsync
            (CancellationToken cancellationToken = default);
    }
}