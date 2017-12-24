using System;
using System.Text;
using Log.It;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class InternalRoutedNetworkClient : INetworkClient
    {
        private bool _disconnected;
        public event EventHandler<ReceivedEventArgs> SendReceived;

        public event EventHandler<ReceivedEventArgs> BufferReceived;
        public event EventHandler Disconnected;
        private readonly ILogger _logger = LogFactory.Create<InternalRoutedNetworkClient>();

        public void Send(byte[] buffer, int offset, int count)
        {
            _logger.Debug($"Sending: {Encoding.UTF8.GetString(buffer)}");
            SendReceived?.Invoke(this, new ReceivedEventArgs(buffer, offset, count));
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
            _logger.Debug("Disconnecting.");
            Disconnected?.Invoke(this, null);
        }

        public void TriggerReceive(object sender, ReceivedEventArgs e)
        {
            _logger.Debug($"Receiving: {Encoding.UTF8.GetString(e.Buffer)}");
            BufferReceived?.Invoke(sender, e);
        }
    }
}