namespace Test.It.With.RabbitMQ.Protocol.Exceptions
{
    public class InvalidFrameEndException : FatalProtocolException
    {
        public InvalidFrameEndException(string message) : base(message)
        {
        }
    }
}