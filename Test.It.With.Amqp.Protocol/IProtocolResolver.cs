using Test.It.With.Amqp.Protocol.Expectations;

namespace Test.It.With.Amqp.Protocol
{
  public interface IProtocolResolver
  {
    IProtocol Protocol { get; }
    IExpectationStateMachineFactory ExpectationStateMachineFactory { get; }
    IFrameFactory FrameFactory { get; }
    IAmqpReaderFactory AmqpReaderFactory { get; }
    IAmqpWriterFactory AmqpWriterFactory { get; }
  }
}