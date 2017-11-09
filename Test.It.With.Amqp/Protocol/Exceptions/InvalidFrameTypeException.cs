namespace Test.It.With.Amqp.Protocol.Exceptions
{
    public class InvalidFrameTypeException : FatalProtocolException
    {
        public InvalidFrameTypeException(string message) : base(message)
        {
        }
    }
}