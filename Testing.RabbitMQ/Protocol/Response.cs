namespace Test.It.With.RabbitMQ.Protocol
{
    internal class Response
    {
        public string MethodName { get; }

        public Response(string methodName)
        {
            MethodName = methodName;
        }
    }
}