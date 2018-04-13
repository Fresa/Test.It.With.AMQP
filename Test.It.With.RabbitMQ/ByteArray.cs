namespace Test.It.With.RabbitMQ._091
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