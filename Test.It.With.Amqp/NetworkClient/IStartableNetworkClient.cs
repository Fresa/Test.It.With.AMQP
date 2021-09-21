using System;

namespace Test.It.With.Amqp.NetworkClient
{
    internal interface IStartableNetworkClient : INetworkClient
    {
        IAsyncDisposable StartReceiving();
    }
}