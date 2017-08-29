using System.Xml;

namespace Test.It.With.RabbitMQ.Protocol
{
    public partial class ProtocolGenerator
    {
        public ProtocolGenerator(XmlDocument protocolDefinition)
        {
            _protocolDefinition = protocolDefinition;
        }
    }
}