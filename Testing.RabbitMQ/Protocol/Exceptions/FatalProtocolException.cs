using System;

namespace Test.It.With.RabbitMQ.Protocol.Exceptions
{
    public abstract class FatalProtocolException : Exception
    {
        protected FatalProtocolException()
        {
        }

        protected FatalProtocolException(string message) : base(message)
        {
        }
    }
}