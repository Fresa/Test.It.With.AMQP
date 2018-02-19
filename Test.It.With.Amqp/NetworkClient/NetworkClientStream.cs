using System;
using System.IO;
using System.Net.Sockets;

namespace Test.It.With.Amqp.NetworkClient
{
    public class NetworkClientStream : Stream
    {
        private readonly INetworkClient _networkClient;
        private readonly BlockingStream _bufferedReadStream = new BlockingStream();
        private MemoryStream _bufferedWriteStream = new MemoryStream();

        public NetworkClientStream(INetworkClient networkClient)
        {
            _networkClient = networkClient;
            _networkClient.BufferReceived += OnBufferReceived;
        }

        private void OnBufferReceived(object sender, ReceivedEventArgs args)
        {
            _bufferedReadStream.Write(args.Buffer, args.Offset, args.Count);
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
          try
          {
            return _bufferedReadStream.Read(buffer, offset, count);
          }
          catch (TimeoutException ex)
          {
            throw new IOException(ex.Message, new SocketException((int)SocketError.TimedOut));
          }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _bufferedWriteStream.Write(buffer, offset, count);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;

        public override int ReadTimeout
        {
            get => _bufferedReadStream.ReadTimeout;
            set => _bufferedReadStream.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            set { }
        }

        public override long Position
        {
            get { return 0; }
            set { }
        }

        public override void Close()
        {
            _bufferedWriteStream.Close();
            _bufferedReadStream.Close();
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _networkClient.BufferReceived -= OnBufferReceived;
                _networkClient.Dispose();
            }
        }
    }
}