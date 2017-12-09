using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.It.With.Amqp.Protocol
{
    internal class BitWriter : IDisposable
    {
        private readonly IByteWriter _writer;

        public BitWriter(IByteWriter writer)
        {
            _writer = writer;
        }

        private readonly List<bool> _values = new List<bool>();

        public void Write(bool value)
        {
            if (_values.Count == 8)
            {
                Flush();
            }

            _values.Add(value);
        }

        public void Flush()
        {
            if (_values.Any() == false)
            {
                return;
            }

            var result = 0;
            for (var i = 0; i < _values.Count; i++)
            {
                result |= (_values[i] ? 1 : 0) << i;
            }

            _writer.WriteByte((byte)result);
            _values.Clear();
        }

        public void Dispose()
        {
            Flush();
        }
    }
}