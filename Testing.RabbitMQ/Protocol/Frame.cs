using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Log.It;
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
        private readonly ILogger _logger = LogFactory.Create<ProtocolHeader>();

        public string Protocol { get; }
        public IVersion Version { get; }
        private const byte ProtocolId = 0xd0;

        public static ProtocolHeader ReadFrom(AmqpReader reader)
        {
            return new ProtocolHeader(reader);
        }

        public ProtocolHeader(IVersion version)
        {
            Protocol = "AMQP";
            Version = version;
        }

        public void WriteTo(AmqpWriter writer)
        {
            writer.WriteBytes(Encoding.UTF8.GetBytes(Protocol));
            writer.WriteByte(ProtocolId);
            writer.WriteByte((byte)Version.Major);
            writer.WriteByte((byte)Version.Minor);
            writer.WriteByte((byte)Version.Revision);
        }

        private ProtocolHeader(AmqpReader reader)
        {
            Protocol = Encoding.UTF8.GetString(reader.ReadBytes(4));

            var constant = reader.ReadByte();

            Version = new ProtocolHeaderVersion(reader);

            if (Protocol != "AMQP" || constant != ProtocolId)
            {
                IsValid = false;
                _logger.Error($"Incorrect header. Expected 'AMQP{ProtocolId}<major version><minor version><revision>'. Got '{Protocol}{constant}{Version.Major}{Version.Minor}{Version.Revision}'.");
            }
        }

        public bool IsValid { get; set; } = true;

        private class ProtocolHeaderVersion : IVersion
        {
            public ProtocolHeaderVersion(AmqpReader reader)
            {
                Major = reader.ReadByte();
                Minor = reader.ReadByte();
                Revision = reader.ReadByte();
            }

            public ProtocolHeaderVersion(int major, int minor, int revision)
            {
                Major = major;
                Minor = minor;
                Revision = revision;
            }

            public int Major { get; }
            public int Minor { get; }
            public int Revision { get; }
        }
    }

    public class ProtocolProcessor
    {
        private readonly INetworkClient _networkClient;
        private readonly IProtocol _protocol;
        private bool _startupPhase = true;


        public ProtocolProcessor(INetworkClient networkClient, IProtocol protocol)
        {
            _networkClient = networkClient;
            _protocol = protocol;
        }

        public void Process(AmqpReader reader)
        {
            if (_startupPhase)
            {
                var header = ProtocolHeader.ReadFrom(reader);

                if (header.Version.Major == _protocol.Version.Major &&
                    header.Version.Minor == _protocol.Version.Minor &&
                    header.Version.Revision == _protocol.Version.Revision &&
                    header.IsValid)
                {
                    // todo: Default, need to check if callback is registered in the test framework
                    var start = new Connection.Start
                    {
                        VersionMajor = new Octet((byte)header.Version.Major),
                        VersionMinor = new Octet((byte)header.Version.Minor),
                        ServerProperties = new PeerProperties(new Dictionary<string, object>
                        {
                            { "host", "localhost"},
                            { "product", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product },
                            { "version", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyVersionAttribute>()?.Version },
                            { "platform", Enum.GetName(typeof(PlatformID), Environment.OSVersion.Platform) },
                            { "copyright", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright },
                            { "information", Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description }
                        }),
                        Locales = new Longstr(Encoding.UTF8.GetBytes("en_US")),
                        Mechanisms = new Longstr(Encoding.UTF8.GetBytes("PLAIN"))
                    };
                    // todo: create frame
                }
                else
                {
                    using (var stream = new MemoryStream())
                    {
                        var writer = new AmqpWriter(stream);
                        var response = new ProtocolHeader(_protocol.Version);
                        response.WriteTo(writer);
                        var bytes = stream.ToArray();
                        _networkClient.Send(bytes, 0, bytes.Length);
                    }

                    Close();
                    return;
                }

            }
        }

        private void Close()
        {
            _networkClient.Dispose();
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