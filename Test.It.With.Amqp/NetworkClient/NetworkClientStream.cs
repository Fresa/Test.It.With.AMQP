using System.IO;

namespace Test.It.With.Amqp.NetworkClient
{
    public class NetworkClientStream : Stream
    {
        private readonly INetworkClient _networkClient;
        private readonly BlockingStream _bufferedReadStream = new BlockingStream();
        private MemoryStream _bufferedWriteStream = new MemoryStream();

        public NetworkClientStream(INetworkClient networkClient)
        {
            _bufferedReadStream.ReadTimeout = 5000;

            _networkClient = networkClient;
            _networkClient.BufferReceived +=
                (sender, args) => _bufferedReadStream.Write(args.Buffer, args.Offset, args.Count);
        }

        public override void Flush()
        {
            var buffer = new byte[_bufferedWriteStream.Length];
            _bufferedWriteStream.Position = 0;
            var bytesRead = _bufferedWriteStream.Read(buffer, 0, buffer.Length);
            _networkClient.Send(buffer, 0, bytesRead);
            _bufferedWriteStream = new MemoryStream();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _bufferedReadStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _bufferedWriteStream.Write(buffer, offset, count);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;

        public override long Position
        {
            get { return 0; }
            set { }
        }
    }
}