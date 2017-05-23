using System;
using Owin;
using Test.It.Fixtures;

namespace Test.It.AppBuilders
{
    public static class AppBuilderExtensions
    {
        public static void Use(this IAppBuilder self, Func<object[], IClient> clientFactory)
        {
            self.Use<IClient>(clientFactory(new object[0]));
        }
    }
}