using System.Collections.Generic;
using Should.Fluent.Model;

namespace Test.It.With.RabbitMQ.Integration.Tests.Assertion
{
    public static class ShouldExtensions
    {       
        public static ExtendedContain<T> Contain<T>(this IShould<IEnumerable<T>> should)
        {
            return new ExtendedContain<T>(should);
        }
    }
}