using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Messages
{
    public class ProtocolHeaderFrame : BaseFrame
    {
        public ProtocolHeaderFrame(short channel, IProtocolHeader protocolHeader)
        {
            Channel = channel;
            ProtocolHeader = protocolHeader;
        }

        public override short Channel { get; }
        public IProtocolHeader ProtocolHeader { get; }
    }

    public class ProtocolHeaderFrame<TProtocolHeader> : BaseFrame
        where TProtocolHeader : class, IProtocolHeader
    {
        public ProtocolHeaderFrame(short channel, TProtocolHeader protocolHeader)
        {
            Channel = channel;
            ProtocolHeader = protocolHeader;
        }

        public override short Channel { get; }
        public TProtocolHeader ProtocolHeader { get; }
    }
}