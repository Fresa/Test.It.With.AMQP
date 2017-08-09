using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Test.It.With.RabbitMQ.NetworkClient
{
    public class NetworkClientStream : Stream
    {
        private readonly INetworkClient _networkClient;
        private readonly BlockingStream _bufferedReadStream = new BlockingStream();
        private readonly MemoryStream _bufferedWriteStream = new MemoryStream();

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
            lock (_bufferedWriteStream)
            {
                _bufferedWriteStream.Position = 0;
                var bytesRead = _bufferedWriteStream.Read(buffer, 0, buffer.Length);
                _networkClient.Send(buffer, 0, bytesRead);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (CanSeek)
            {
                return _bufferedReadStream.Seek(offset, origin);
            }
            return -1;
        }

        public override void SetLength(long value)
        {
            _bufferedReadStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _bufferedReadStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _bufferedWriteStream.Write(buffer, offset, count);
        }

        public override bool CanRead => _bufferedReadStream.CanRead;
        public override bool CanSeek => _bufferedReadStream.CanSeek;
        public override bool CanWrite => _bufferedWriteStream.CanWrite;
        public override long Length => _bufferedReadStream.Length;

        public override long Position
        {
            get => _bufferedReadStream.Position;
            set => _bufferedReadStream.Position = value;
        }
    }
}