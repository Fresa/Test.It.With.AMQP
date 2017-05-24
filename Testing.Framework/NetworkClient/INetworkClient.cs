using System;

namespace Test.It.NetworkClient
{
    public interface INetworkClient : IDisposable
    {
        event EventHandler<ReceivedEventArgs> BufferReceived;
        event EventHandler Disconnected;
        void Send(byte[] buffer, int offset, int count);
    }
}