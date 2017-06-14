namespace Test.It.With.RabbitMQ.Tests.TestApplication
{
    public class TestMessage
    {
        public TestMessage(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}