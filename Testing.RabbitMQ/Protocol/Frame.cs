using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Test.It.With.Amqp;
using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Extensions;
using Test.It.With.RabbitMQ.MessageClient;
using Test.It.With.RabbitMQ.NetworkClient;
using Validation;

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

    public class ProtocolHeader
    {
        public string Protocol { get; }
        public IVersion Version { get; }

        public static ProtocolHeader ReadFrom(AmqpReader reader)
        {
            return new ProtocolHeader(reader);
        }

        private ProtocolHeader(AmqpReader reader)
        {
            Protocol = Encoding.UTF8.GetString(reader.ReadBytes(4));

            var constant = reader.ReadByte();

            Version = new ProtocolHeaderVersion(reader);

            if (Protocol != "AMQP" || constant != 0xd0)
            {
                throw new ProtocolViolationException($"Incorrect header. Expected 'AMQP0xd0<major version><minor version><revision>'. Got '{Protocol}{constant}{Version.Major}{Version.Minor}{Version.Revision}'.");
            }
        }

        private class ProtocolHeaderVersion : IVersion
        {
            public ProtocolHeaderVersion(AmqpReader reader)
            {
                Major = reader.ReadByte();
                Minor = reader.ReadByte();
                Revision = reader.ReadByte();
            }

            public int Major { get; }
            public int Minor { get; }
            public int Revision { get; }
        }
    }

    public class ProtocolProcessor
    {
        private readonly INetworkClient _networkClient;
        private bool _startupPhase;


        public ProtocolProcessor(INetworkClient networkClient)
        {
            _networkClient = networkClient;

        }

        public void Process(AmqpReader reader)
        {
            if (_startupPhase)
            {
                var header = ProtocolHeader.ReadFrom(reader);
                //Frame.ReadFrom()
            }
        }


    }

    public class InvalidFrameTypeException : FatalProtocolException
    {
        public InvalidFrameTypeException(string message) : base(message)
        {
        }
    }

    public class InvalidFrameEndException : FatalProtocolException
    {
        public InvalidFrameEndException(string message) : base(message)
        {
        }
    }

    public abstract class FatalProtocolException : Exception
    {
        protected FatalProtocolException()
        {
        }

        protected FatalProtocolException(string message) : base(message)
        {
        }
    }
}