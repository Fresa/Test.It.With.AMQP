using System;
using System.Text;
using Test.It.NetworkClient;

namespace Test.It.With.RabbitMQ.NetworkClient
{
    internal class InternalRoutedNetworkClient : INetworkClient
    {
        public event EventHandler<ReceivedEventArgs> SendReceived;

        public event EventHandler<ReceivedEventArgs> BufferReceived;
        public event EventHandler Disconnected;

        public void Send(byte[] buffer, int offset, int count)
        {
            Console.Write("Sending: " + Encoding.UTF8.GetString(buffer));
            SendReceived?.Invoke(this, new ReceivedEventArgs(buffer, offset, count));
        }

        public void Dispose()
        {
            Disconnected?.Invoke(this, null);
        }

        public void TriggerReceive(object sender, ReceivedEventArgs e)
        {
            Console.Write("Receiving: " + Encoding.UTF8.GetString(e.Buffer));
            BufferReceived?.Invoke(sender, e);
        }
    }
}