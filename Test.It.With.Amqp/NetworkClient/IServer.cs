using System;
using System.Net;

namespace Test.It.With.Amqp.NetworkClient
{
    public interface IServer : IAsyncDisposable
    {
        int Port { get; }

        IPAddress Address { get; }
    }
}