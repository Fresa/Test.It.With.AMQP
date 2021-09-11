using System.Threading;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.NetworkClient
{
    internal interface INetworkServer
    {
        Task<INetworkClient> WaitForConnectedClientAsync
            (CancellationToken cancellationToken = default);
    }
}