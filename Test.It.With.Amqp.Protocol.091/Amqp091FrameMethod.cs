namespace Test.It.With.Amqp.Protocol._091
{
    internal class Amqp091FrameMethod : Amqp091Frame
    {
        public Amqp091FrameMethod(short channel, IMethod method) : base(Constants.FrameMethod, channel, method)
        {
        }
    }
}