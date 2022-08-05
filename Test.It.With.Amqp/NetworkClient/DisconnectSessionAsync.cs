using System.Threading;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.NetworkClient
{
    internal delegate ValueTask DisconnectSessionAsync(CancellationToken cancellation);
}