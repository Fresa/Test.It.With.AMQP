using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
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

        public TestFrameHandler(INetworkClient networkClient)
        {
            var stream = new NetworkClientStream(networkClient);
            _reader = new NetworkBinaryReader(stream);
            _writer = new NetworkBinaryWriter(stream);
        }

        public void Close()
        {
            lock (_reader)
            {
                _reader.Close();
            }
            lock (_writer)
            {
                _writer.Close();
            }
        }

        public InboundFrame ReadFrame()
        {
            lock (_reader)
            {
                try
                {
                    return InboundFrame.ReadFrom(_reader);
                }
                catch
                {
                    // Send heartbeat
                    var stream = new MemoryStream();
                    var writer = new NetworkBinaryWriter(stream);
                    
                    var outbound = new OutboundFrame(FrameType.FrameHeartbeat, 0);
                    outbound.WriteTo(writer);
                    stream.Position = 0;
                    var reader = new NetworkBinaryReader(stream);
                    return InboundFrame.ReadFrom(reader);
                }
            }
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
                }
                _writer.Flush();
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
            set { }
        }

        public int WriteTimeout
        {
            set { }
        }
    }
}
