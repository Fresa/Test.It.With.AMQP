using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Log.It;
using RabbitMQ.Client.Framing;
using RabbitMQ.Client.Framing.Impl;
using RabbitMQ.Client.Impl;
using RabbitMQ.Util;
using Test.It.With.RabbitMQ.NetworkClient;

namespace Test.It.With.RabbitMQ.MessageClient
{
    public class MessageClient : IMessageClient
    {
        private readonly INetworkClient _networkClient;
        private readonly ISerializer _serializer;
        private ILogger _logger = LogFactory.Create<MessageClient>();

        public MessageClient(INetworkClient networkClient, ISerializer serializer)
        {
            _networkClient = networkClient;
            _serializer = serializer;

            _networkClient.BufferReceived += (sender, args) =>
            {
                var logMessage = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.Count);
                
                _logger.Info(logMessage);

                if (args.Buffer[0] == 'A')
                {
                    //var writer = new NetworkBinaryWriter(stream);
                    //var argumentWriter = new MethodArgumentWriter(writer);

                    var start = new ConnectionStart(0, 9, new Dictionary<string, object>(), Encoding.UTF8.GetBytes("PLAIN"), new byte[0]);
                    
                    
                    var frame = new Frame(Constants.FrameMethod, 0);
                    //frame.m_accumulator = stream;
                    NetworkBinaryWriter writer = frame.GetWriter();
                    writer.Write((ushort)start.ProtocolClassId);
                    writer.Write((ushort)start.ProtocolMethodId);
                    var argWriter = new MethodArgumentWriter(writer);
                    start.WriteArgumentsTo(argWriter);
                    argWriter.Flush();

                    var stream = new MemoryStream();
                    var outputWriter = new NetworkBinaryWriter(stream);
                    frame.WriteTo(outputWriter);
                    writer.Flush();

                    var buffer = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(buffer, 0, buffer.Length);
                    _networkClient.Send(buffer, 0, buffer.Length);
                }

                //var responseMessage = new byte[] { 65, 77, 81, 80, 1, 1,1,1};
                //_networkClient.Send(responseMessage, 0, responseMessage.Length);
                return;
                var message = _serializer.Deserialize<MessageEnvelope>(args.Buffer);
                BufferReceived?.Invoke(this, message);
            };
            _networkClient.Disconnected += Disconnected;
        }

        public event EventHandler<MessageEnvelope> BufferReceived;
        public event EventHandler Disconnected;

        public void Send(MessageEnvelope envelope)
        {
            var bytes = _serializer.Serialize(envelope);
            _networkClient.Send(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            _networkClient.Dispose();
        }
    }
}