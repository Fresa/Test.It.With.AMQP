namespace Test.It.With.Amqp.Protocol
{
    internal class ByteArray
    {
        public ByteArray(byte[] bytes)
        {
            Bytes = bytes;
        }

        public byte[] Bytes { get; }
    }
}