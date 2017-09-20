namespace Test.It.With.RabbitMQ.Protocol.Exceptions
{
    public class InvalidFrameTypeException : FatalProtocolException
    {
        public InvalidFrameTypeException(string message) : base(message)
        {
        }
    }
}