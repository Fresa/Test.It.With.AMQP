using System;

namespace Test.It.With.RabbitMQ.Protocol.Definitions
{
    public class Response
    {
        private readonly Lazy<Method> _methodResolver;
        public Method Method => _methodResolver.Value;

        public Response(Lazy<Method> methodResolver)
        {
            _methodResolver = methodResolver;
        }
    }
}