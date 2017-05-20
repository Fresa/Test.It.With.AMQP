using System.IO;

namespace Testing.RabbitMQ.NetworkClient
{
    public class NetworkClientStream : Stream
    {
        private readonly INetworkClient _networkClient;
        private long _length;
        private readonly BufferedStream _bufferedReadStream = new BufferedStream(new MemoryStream());
        private readonly BufferedStream _bufferedWriteStream = new BufferedStream(new MemoryStream());

        public NetworkClientStream(INetworkClient networkClient)
        {
            _networkClient = networkClient;
            _networkClient.BufferReceived +=
                (sender, args) => _bufferedReadStream.Write(args.Buffer, args.Offset, args.Count);
        }

        public override void Flush()
        {
            byte[] buffer = { };
            lock (_bufferedWriteStream)
            {
                var offset = 0;
                while (_bufferedWriteStream.Length > 0)
                {
                    var length = int.MaxValue;
                    if (_bufferedWriteStream.Length <= int.MaxValue)
                    {
                        length = (int)_bufferedWriteStream.Length;
                    }
                    _bufferedWriteStream.Read(buffer, 0, length);
                    _networkClient.Send(buffer, offset, length);
                    offset += length;
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            _length = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _bufferedReadStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _bufferedWriteStream.Write(buffer, offset, count);
        }

        public override bool CanRead { get; } = false;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;
        public override long Length => _length;
        public override long Position { get; set; }
    }
}