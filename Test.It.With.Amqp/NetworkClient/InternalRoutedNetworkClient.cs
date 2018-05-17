using System;
using System.Linq;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class InternalRoutedNetworkClient : INetworkClient
    {
        private bool _disconnected;
        public event EventHandler<ReceivedEventArgs> SendReceived;

        public event EventHandler<ReceivedEventArgs> BufferReceived;
        public event EventHandler Disconnected;

        public void Send(byte[] buffer, int offset, int count)
        {
            if (buffer.Any())
            {
                SendReceived?.Invoke(this, new ReceivedEventArgs(buffer, offset, count));
            }
        }

        public void Dispose()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            if (_disconnected)
            {
                return;
            }
            _disconnected = true;
            Disconnected?.Invoke(this, null);
        }

        public void TriggerReceive(object sender, ReceivedEventArgs e)
        {
            if (e.Buffer.Any())
            {
                BufferReceived?.Invoke(sender, e);
            }
        }
    }
}