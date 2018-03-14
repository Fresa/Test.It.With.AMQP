using Test.It.With.Amqp.Protocol.Expectations;
using Test.It.With.Amqp.Protocol._091;

namespace Test.It.With.Amqp.Protocol
{
    internal class Amqp091ProtocolResolver : IProtocolResolver
    {
        private Amqp091ProtocolResolver()
        {
            Protocol = new Amq091Protocol();
            ExpectationStateMachineFactory = new Amqp091ExpectationStateMachineFactory();
            FrameFactory = new Amqp091FrameFactory();
            AmqpReaderFactory = new Amqp091ReaderFactory();
            AmqpWriterFactory = new Amqp091WriterFactory();
        }

        public static IProtocolResolver Create()
        {
            return new Amqp091ProtocolResolver();
        }

        public IProtocol Protocol { get; }

        public IExpectationStateMachineFactory ExpectationStateMachineFactory { get; }

        public IFrameFactory FrameFactory { get; }

        public IAmqpReaderFactory AmqpReaderFactory { get; }

        public IAmqpWriterFactory AmqpWriterFactory { get; }
    }
}
