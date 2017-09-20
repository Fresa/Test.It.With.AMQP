using System.Text;
using Log.It;
using Test.It.With.Amqp.Protocol;

namespace Test.It.With.RabbitMQ.Protocol
{
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
            
            public int Major { get; }
            public int Minor { get; }
            public int Revision { get; }
        }
    }
}