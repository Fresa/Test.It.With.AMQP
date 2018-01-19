using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Messages
{
    public class ProtocolHeaderFrame : BaseFrame<IProtocolHeader>
    {
        public ProtocolHeaderFrame(short channel, IProtocolHeader protocolHeader)
        {
            Channel = channel;
            Message = protocolHeader;
        }

        public override short Channel { get; }
        public override IProtocolHeader Message { get; }
    }

    public class ProtocolHeaderFrame<TProtocolHeader> : BaseFrame<TProtocolHeader>
        where TProtocolHeader : class, IProtocolHeader
    {
        public ProtocolHeaderFrame(short channel, TProtocolHeader protocolHeader)
        {
            Channel = channel;
            Message = protocolHeader;
        }

        public override short Channel { get; }
        public override TProtocolHeader Message { get; }
    }
}