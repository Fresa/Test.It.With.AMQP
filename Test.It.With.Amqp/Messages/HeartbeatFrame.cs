using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Messages
{
    public class HeartbeatFrame : BaseFrame
    {
        public HeartbeatFrame(short channel, IHeartbeat heartbeat)
        {
            Channel = channel;
            Heartbeat = heartbeat;
        }

        public override short Channel { get; }
        public IHeartbeat Heartbeat { get; }
    }

    public class HeartbeatFrame<THeartbeat> : BaseFrame
        where THeartbeat : IHeartbeat
    {
        public HeartbeatFrame(short channel, THeartbeat heartbeat)
        {
            Channel = channel;
            Heartbeat = heartbeat;
        }

        public override short Channel { get; }
        public THeartbeat Heartbeat { get; }
    }
}