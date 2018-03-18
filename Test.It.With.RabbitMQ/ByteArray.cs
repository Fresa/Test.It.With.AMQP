namespace Test.It.With.RabbitMQ
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