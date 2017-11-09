using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Should.Fluent;
using Test.It.With.Amqp.NetworkClient;
using Test.It.With.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.Amqp.Tests
{
    public class When_reading_and_writing_to_the_blocking_stream_concurrently : XUnit2Specification
    {
        private BlockingStream _stream;
        private List<string> _readData;
        private List<string> _dataSent;

        public When_reading_and_writing_to_the_blocking_stream_concurrently(ITestOutputHelper output) : base(output)
        {
            
        }

        protected override void Given()
        {
            _stream = new BlockingStream();
        }

        protected override void When()
        {
            var bytesSent = new List<byte[]>();
            _dataSent = new List<string>();

            for (var i = 0; i < 1000; i++)
            {
                var data = "test" + i;
                _dataSent.Add(data);
                var bytes = Encoding.UTF8.GetBytes(data);
                bytesSent.Add(bytes);
            }

            _readData = new List<string>();
            var resetEvent = new ManualResetEvent(false);

            var read = new ManualResetEvent(false);
            var write = new ManualResetEvent(false);

            Task.Run(() =>
            {
                read.WaitOne();
                foreach (var bytes in bytesSent)
                {
                    var data = new byte[bytes.Length];
                    _stream.Read(data, 0, data.Length);
                    _readData.Add(Encoding.UTF8.GetString(data));
                    write.Set();
                }
                resetEvent.Set();
            });

            Task.Run(() =>
            {
                foreach (var bytes in bytesSent)
                {
                    _stream.Write(bytes, 0, bytes.Length);
                    read.Set();
                    write.WaitOne();
                }
            });

            resetEvent.WaitOne();
        }

        [Fact]
        public void It_should_send_data()
        {
            _dataSent.Count.Should().Equal(1000);
        }

        [Fact]
        public void It_should_receive_data()
        {
            _readData.Count.Should().Equal(1000);
        }

        [Fact]
        public void It_should_have_read_all_data()
        {
            for (var i = 0; i<_dataSent.Count; i++)
            {
                var dataSent = _dataSent[i];
                var dataRead = _readData[i];

                dataSent.Length.Should().Be.GreaterThan(0);
                dataRead.Should().Equal(dataSent);
            }
        }
    }
}