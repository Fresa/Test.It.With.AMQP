using System;

namespace Test.It.With.RabbitMQ.NetworkClient
{
    public interface INetworkClient : IDisposable
    {
        event EventHandler<ReceivedEventArgs> BufferReceived;
        event EventHandler Disconnected;
        void Send(byte[] buffer, int offset, int count);
    }
}