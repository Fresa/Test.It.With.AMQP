using System.Collections.Generic;
using System.Linq;

namespace Test.It.With.Amqp.Protocol
{
    internal class BitReader
    {
        private readonly IByteReader _reader;

        public BitReader(IByteReader reader)
        {
            _reader = reader;
        }

        private readonly Queue<bool> _values = new Queue<bool>();

        public bool Read()
        {
            if (_values.Any() == false)
            {
                Populate();
            }

            return _values.Dequeue();
        }

        private void Populate()
        {
            var value = _reader.ReadByte();
            for (var i = 0; i < 8; i++)
            {
                _values.Enqueue((value & (1 << i)) != 0);
            }
        }

        public void Reset()
        {
            _values.Clear();
        }
    }
}