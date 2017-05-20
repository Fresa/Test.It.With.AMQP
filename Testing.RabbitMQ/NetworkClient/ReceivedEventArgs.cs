using System;

namespace Testing.RabbitMQ.NetworkClient
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