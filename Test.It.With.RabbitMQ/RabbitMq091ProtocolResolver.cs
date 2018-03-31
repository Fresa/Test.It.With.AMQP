using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Expectations;

namespace Test.It.With.RabbitMQ
{
    internal class RabbitMq091ProtocolResolver : IProtocolResolver
    {
        private readonly IProtocolResolver _amqpResolver;

        private RabbitMq091ProtocolResolver()
        {
            _amqpResolver = Amqp091ProtocolResolver.Create();
            AmqpReaderFactory = new RabbitMq091ReaderFactory();
            AmqpWriterFactory = new RabbitMq091WriterFactory();
            Protocol = new RabbitMq091ProtocolGeneratorDecorator(_amqpResolver.Protocol);
        }

        public static IProtocolResolver Create()
        {
            return new RabbitMq091ProtocolResolver();
        }

        public IProtocol Protocol { get; }

        public IExpectationStateMachineFactory ExpectationStateMachineFactory =>
            _amqpResolver.ExpectationStateMachineFactory;

        public IFrameFactory FrameFactory => _amqpResolver.FrameFactory;

        public IAmqpReaderFactory AmqpReaderFactory { get; }

        public IAmqpWriterFactory AmqpWriterFactory { get; }
    }
}