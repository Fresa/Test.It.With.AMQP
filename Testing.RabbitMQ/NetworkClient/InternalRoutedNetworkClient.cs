using System;

namespace Testing.RabbitMQ.NetworkClient
{
    internal class InternalRoutedNetworkClient : INetworkClient
    {
        public event EventHandler<ReceivedEventArgs> SendReceived;

        public event EventHandler<ReceivedEventArgs> BufferReceived;
        public event EventHandler Disconnected;

        public void Send(byte[] buffer, int offset, int count)
        {
            SendReceived?.Invoke(this, new ReceivedEventArgs(buffer, offset, count));
        }

        public void Dispose()
        {
            Disconnected?.Invoke(this, null);
        }

        public void TriggerReceive(object sender, ReceivedEventArgs e)
        {
            BufferReceived?.Invoke(sender, e);
        }
    }
}