using System.Collections.Generic;
using System.IO;
using Test.It.With.Amqp.Protocol.Extensions;

// todo: need to remove this reference. A Frame should be abstract

namespace Test.It.With.Amqp.Protocol._091
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

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new AmqpWriter(memoryStream))
                {
                    writer.WriteShortInteger((short)method.ProtocolClassId);
                    writer.WriteShortInteger((short)method.ProtocolMethodId);
                    method.WriteTo(writer);
                }

                Payload = memoryStream.GetBuffer();
            }

            Size = Payload.Length;
        }

        private Frame(AmqpReader reader)
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