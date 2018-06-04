using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Messages
{
    internal class HeartbeatFrame : BaseFrame<IHeartbeat>
    {
        public HeartbeatFrame(short channel, IHeartbeat heartbeat)
        {
            Channel = channel;
            Message = heartbeat;
        }

        public override short Channel { get; }
        public override IHeartbeat Message { get; }
    }

    public class HeartbeatFrame<THeartbeat> : BaseFrame<THeartbeat>
        where THeartbeat : class, IHeartbeat
    {
        public HeartbeatFrame(short channel, THeartbeat heartbeat)
        {
            Channel = channel;
            Message = heartbeat;
        }

        public override short Channel { get; }
        public override THeartbeat Message { get; }
    }
}