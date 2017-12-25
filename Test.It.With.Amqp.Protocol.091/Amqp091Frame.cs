using System.Collections.Generic;
using System.IO;
using Test.It.With.Amqp.Protocol.Extensions;

namespace Test.It.With.Amqp.Protocol._091
{
    internal class Amqp091Frame : IFrame
    {
        public static Amqp091Frame ReadFrom(Amqp091Reader reader)
        {
            return new Amqp091Frame(reader);
        }
        
        public Amqp091Frame(int type, short channel, IMethod method)
        {
            Type = type;
            AssertValidFrameType(Type);

            Channel = channel;

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new Amqp091Writer(memoryStream))
                {
                    writer.WriteShortInteger((short)method.ProtocolClassId);
                    writer.WriteShortInteger((short)method.ProtocolMethodId);
                    method.WriteTo(writer);
                }

                Payload = memoryStream.GetBuffer();
            }

            Size = Payload.Length;
        }

        private Amqp091Frame(IAmqpReader reader)
        {
            Type = reader.ReadByte();
            AssertValidFrameType(Type);
            
            Channel = reader.ReadShortInteger();
            Size = reader.ReadLongInteger();
            Payload = reader.ReadBytes(Size);

            var frameEnd = reader.ReadByte();

            if (frameEnd != Constants.FrameEnd)
            {
                throw new FrameErrorException($"Expected '{Constants.FrameEnd}', got '{frameEnd}'.");
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
                // todo: Resolve protocol specific exceptions in an abstract way
                throw new FrameErrorException($"Expected: {_validFrameTypes.Join(", ", " or ", frameType => $"{frameType.Value}: {frameType.Key}")}, got: {type}.");
            }
        }

        public int Type { get; }
        public short Channel { get; }
        public int Size { get; }
        public byte[] Payload { get; }

        public void WriteTo(IAmqpWriter writer)
        {
            writer.WriteByte((byte)Type);
            writer.WriteShortInteger(Channel);
            writer.WriteLongInteger(Size);
            writer.WriteBytes(Payload);
            writer.WriteByte(Constants.FrameEnd);
        }

        public bool IsMethod()
        {
            return Type == Constants.FrameMethod;
        }

        public bool IsBody()
        {
            return Type == Constants.FrameBody;
        }

        public bool IsHeader()
        {
            return Type == Constants.FrameHeader;
        }

        public bool IsHeartbeat()
        {
            return Type == Constants.FrameHeartbeat;
        }
    }
}