using System;

namespace Test.It.With.RabbitMQ.NetworkClient
{
    public class ReceivedEventArgs : EventArgs
    {
        public ReceivedEventArgs(byte[] buffer, int offset, int count)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Offset = offset;
            Count = count;
        }

        public byte[] Buffer { get; }
        public int Offset { get; }
        public int Count { get; }
    }
}