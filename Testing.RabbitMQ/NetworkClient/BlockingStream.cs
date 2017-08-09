using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace Test.It.With.RabbitMQ.NetworkClient
{
    internal class BlockingStream : Stream
    {
        private BlockingCollection<byte> _buffer = new BlockingCollection<byte>(int.MaxValue);

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = false;
        public override bool CanTimeout { get; } = true;
        public override bool CanWrite { get; } = true;
        public override long Length => _buffer.Count;
        public override long Position { get { return 0; } set { } }
        public override int ReadTimeout { get; set; } = Timeout.Infinite;
        public override int WriteTimeout { get; set; } = Timeout.Infinite;

        public override void Write(byte[] buffer, int offset, int count)
        {
            foreach (var b in buffer.Skip(offset).Take(count))
            {
                if (_buffer.TryAdd(b, WriteTimeout) == false)
                {
                    throw new TimeoutException($"Waited to write for {WriteTimeout}ms.");
                }
            }
        }

        public override void Flush()
        {
            _buffer = new BlockingCollection<byte>(int.MaxValue);
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
            for (var i = 0; i < count; i++)
            {
                if (_buffer.TryTake(out var data, ReadTimeout) == false)
                {
                    throw new TimeoutException($"Waited to read for {ReadTimeout}ms.");
                }
                buffer[i + offset] = data;
            }
            return count;
        }
    }
}