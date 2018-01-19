using System.IO;

namespace Test.It.With.Amqp.Protocol._091
{
    internal class Amqp091MethodFrame : Amqp091Frame
    {
        public Amqp091MethodFrame(short channel, IMethod method) : base(Constants.FrameMethod, channel)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new Amqp091Writer(memoryStream))
                {
                    method.WriteTo(writer);
                }

                Payload = memoryStream.GetBuffer();
            }

            Size = Payload.Length;
        }
    }

    internal class Amqp091ContentHeaderFrame : Amqp091Frame
    {
        public Amqp091ContentHeaderFrame(short channel, IContentHeader header) : base(Constants.FrameHeader, channel)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new Amqp091Writer(memoryStream))
                {
                    header.WriteTo(writer);
                }

                Payload = memoryStream.GetBuffer();
            }

            Size = Payload.Length;
        }
    }

    internal class Amqp091ContentBodyFrame : Amqp091Frame
    {
        public Amqp091ContentBodyFrame(short channel, IContentBody body) : base(Constants.FrameBody, channel)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new Amqp091Writer(memoryStream))
                {
                    body.WriteTo(writer);
                }

                Payload = memoryStream.GetBuffer();
            }

            Size = Payload.Length;
        }
    }

    internal class Amqp091HeartbeatFrame : Amqp091Frame
    {
        public Amqp091HeartbeatFrame(short channel, IHeartbeat heartbeat) : base(Constants.FrameHeartbeat, channel)
        {
        }
    }
}