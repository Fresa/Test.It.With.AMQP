using Test.It.With.Amqp.Protocol;

namespace Test.It.With.RabbitMQ._091
{
    internal class RabbitMq091ProtocolGeneratorDecorator : IProtocol
    {
        private readonly IProtocol _protocol;
        private readonly RabbitMq091ProtocolExtensions _protocolExtensions = new RabbitMq091ProtocolExtensions();

        public RabbitMq091ProtocolGeneratorDecorator(IProtocol protocol)
        {
            _protocol = protocol;
        }

        public IVersion Version => _protocol.Version;
        public IProtocolHeader GetProtocolHeader(IAmqpReader reader)
        {
            return _protocol.GetProtocolHeader(reader);
        }

        public IMethod GetMethod(IAmqpReader reader)
        {
            if (_protocolExtensions.TryGetMethod(reader.Clone(), out var method))
            {
                return method;
            }

            return _protocol.GetMethod(reader);
        }

        public IContentHeader GetContentHeader(IAmqpReader reader)
        {
            return _protocol.GetContentHeader(reader);
        }

        public IContentBody GetContentBody(IAmqpReader reader)
        {
            return _protocol.GetContentBody(reader);
        }

        public IHeartbeat GetHeartbeat(IAmqpReader reader)
        {
            return _protocol.GetHeartbeat(reader);
        }
    }
}