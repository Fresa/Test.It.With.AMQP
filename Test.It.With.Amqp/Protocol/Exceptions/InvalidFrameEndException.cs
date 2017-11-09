namespace Test.It.With.Amqp.Protocol.Exceptions
{
    public class InvalidFrameEndException : FatalProtocolException
    {
        public InvalidFrameEndException(string message) : base(message)
        {
        }
    }
}