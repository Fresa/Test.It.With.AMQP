using Test.It.With.Amqp.Protocol;

namespace Test.It.With.Amqp.Extensions
{
    internal static class AmqpReaderExtensions
    {
        internal static bool HasMore(this IAmqpReader reader)
        {
            try
            {
                reader.ThrowIfMoreData();
            }
            catch 
            {
                return true;
            }

            return false;
        }
    }
}