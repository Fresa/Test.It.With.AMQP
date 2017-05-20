using System;
using Owin;
using Testing.Framework.Fixtures;

namespace Testing.Framework.AppBuilders
{
    public static class AppBuilderExtensions
    {
        public static void Use(this IAppBuilder self, Func<object[], IClient> clientFactory)
        {
            self.Use<IClient>(clientFactory(new object[0]));
        }
    }
}