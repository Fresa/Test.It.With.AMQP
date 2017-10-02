using System.Collections.Generic;
using System.IO;
using System.Net;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.RabbitMQ.Protocol.Exceptions;

namespace Test.It.With.RabbitMQ.Protocol
{
    public class Frame
    {
        public static Frame ReadFrom(AmqpReader reader)
        {
            return new Frame(reader);
        }
        
        public Frame(int type, short channel, IMethod method)
        {
            Type = type;
            Channel = channel;

            var memoryStream = new MemoryStream();
            method.WriteTo(new AmqpWriter(memoryStream));
            Payload = memoryStream.GetBuffer();
            Size = Payload.Length;
        }

        private Frame(AmqpReader reader)
        {
            Type = reader.PeekByte();

            if (Type == 'A')
            {
                ProtocolHeader.ReadFrom(reader);
                throw new ProtocolViolationException("Did not expect a protocol header at this time.");
            }

            AssertValidFrameType(Type);

            reader.ReadByte();

            Channel = reader.ReadShortInteger();
            Size = reader.ReadLongInteger();
            Payload = reader.ReadBytes(Size);

            var frameEnd = reader.ReadByte();

            if (frameEnd != Constants.FrameEnd)
            {
                throw new InvalidFrameEndException($"Expected '{Constants.FrameEnd}', got '{frameEnd}'.");
            }
        }

        private readonly Dictionary<int, string> _validFrameTypes = new Dictionary<int, string>
        {
            { Constants.FrameMethod, nameof(Constants.FrameMethod) },
            { Constants.FrameHeader, nameof(Constants.FrameHeader) },
            { Constants.FrameBody, nameof(Constants.FrameBody) },
            { Constants.FrameHeartbeat, nameof(Constants.FrameHeartbeat) }
        };

        private void AssertValidFrameType(int type)
        {
            if (_validFrameTypes.ContainsKey(type) == false)
            {
                throw new InvalidFrameTypeException($"Expected: {_validFrameTypes.Join(", ", " or ", frameType => $"{frameType.Value}: {frameType.Key}")}, got: {type}.");
            }
        }

        public int Type { get; }
        public short Channel { get; }
        public int Size { get; }
        public byte[] Payload { get; }

        public void WriteTo(AmqpWriter writer)
        {
            writer.WriteByte((byte)Type);
            writer.WriteShortInteger(Channel);
            writer.WriteLongInteger(Size);
            writer.WriteBytes(Payload);
            writer.WriteByte(Constants.FrameEnd);
        }
    }
}