using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace Test.It.With.Amqp.NetworkClient
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

        /// <inheritdoc />
        /// <summary>
        /// Tries to read to the current stream within the specified timeout period.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <exception cref="T:System.TimeoutException"></exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            foreach (var bytes in buffer.Skip(offset).Take(count))
            {
                if (_buffer.TryAdd(bytes, WriteTimeout) == false)
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

        /// <inheritdoc />
        /// <summary>
        /// Tries to read from the current stream within the specified timeout period.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <exception cref="T:System.TimeoutException"></exception>
        /// <returns>Bytes read</returns>
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _buffer.Dispose();
        }
    }
}