using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Impl;
using RabbitMQ.Util;
using Test.It.With.Amqp.NetworkClient;

namespace Test.It.With.RabbitMQ
{
    public class TestFrameHandler : IFrameHandler
    {
        private readonly NetworkBinaryReader _reader;
        private readonly NetworkBinaryWriter _writer;
        private readonly NetworkClientStream _stream;

        public TestFrameHandler(INetworkClient networkClient)
        {
            networkClient.Disconnected += (sender, args) =>
            {
                Close();
            };

            _stream = new NetworkClientStream(networkClient);
            _reader = new NetworkBinaryReader(_stream);
            _writer = new NetworkBinaryWriter(_stream);
        }

        public void Close()
        {
            _reader.Close();
            lock (_writer)
            {
                _writer.Close();
            }
        }

        public InboundFrame ReadFrame()
        {
            return InboundFrame.ReadFrom(_reader);
        }

        public void SendHeader()
        {
            lock (_writer)
            {
                _writer.Write(Encoding.ASCII.GetBytes("AMQP"));
                if (Endpoint.Protocol.Revision != 0)
                {
                    _writer.Write((byte)0);
                    _writer.Write((byte)Endpoint.Protocol.MajorVersion);
                    _writer.Write((byte)Endpoint.Protocol.MinorVersion);
                    _writer.Write((byte)Endpoint.Protocol.Revision);
                }
                else
                {
                    _writer.Write((byte)1);
                    _writer.Write((byte)1);
                    _writer.Write((byte)Endpoint.Protocol.MajorVersion);
                    _writer.Write((byte)Endpoint.Protocol.MinorVersion);
                }
                _writer.Flush();
            }
        }

        public void WriteFrame(OutboundFrame frame)
        {
            lock (_writer)
            {
                frame.WriteTo(_writer);
                _writer.Flush();
            }
        }

        public void WriteFrameSet(IList<OutboundFrame> frames)
        {
            lock (_writer)
            {
                foreach (var frame in frames)
                {
                    frame.WriteTo(_writer);
                    _writer.Flush();
                }
            }
        }

        public void Flush()
        {
            lock (_writer)
            {
                _writer.Flush();
            }
        }

        public AmqpTcpEndpoint Endpoint { get; } = new AmqpTcpEndpoint();

        public EndPoint LocalEndPoint { get; } = new IPEndPoint(IPAddress.Any, 0);

        public int LocalPort { get; } = 0;

        public EndPoint RemoteEndPoint { get; } = new IPEndPoint(IPAddress.Any, 0);

        public int RemotePort { get; } = 0;

        public int ReadTimeout
        {
            set => _stream.ReadTimeout = value == 0 ? Timeout.Infinite : value;
        }

        public int WriteTimeout { set => _stream.WriteTimeout = value; }
    }
}
