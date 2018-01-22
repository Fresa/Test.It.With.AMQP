using Test.It.With.Amqp.Protocol;
using Test.It.With.Amqp.Protocol.Expectations;
using Test.It.With.Amqp.Protocol._091;

namespace Test.It.With.Amqp
{
    internal class ProtocolResolver
    {
        public ProtocolResolver(ProtocolVersion protocolVersion)
        {
            switch (protocolVersion)
            {
                case ProtocolVersion.AMQP091:
                    Protocol = new Amq091Protocol();
                    ExpectationStateMachine = new Amqp091ExpectationStateMachine();
                    FrameFactory = new Amqp091FrameFactory();
                    AmqpReaderFactory = new Amqp091ReaderFactory();
                    AmqpWriterFactory = new Amqp091WriterFactory();
                    return;
            }
        }

        public IProtocol Protocol { get; }

        public IExpectationStateMachine ExpectationStateMachine { get; }

        public IFrameFactory FrameFactory { get; }

        public IAmqpReaderFactory AmqpReaderFactory { get; }

        public IAmqpWriterFactory AmqpWriterFactory { get; }
    }
}
