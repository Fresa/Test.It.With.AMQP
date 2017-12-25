namespace Test.It.With.Amqp.Protocol._091
{
    public class FrameMethod : Frame
    {
        public FrameMethod(short channel, IMethod method) : base(Constants.FrameMethod, channel, method)
        {
        }
    }
}