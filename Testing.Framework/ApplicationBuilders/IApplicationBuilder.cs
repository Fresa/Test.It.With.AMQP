using Testing.Framework.Specifications;
using Testing.Framework.Starters;

namespace Testing.Framework.ApplicationBuilders
{
    public interface IApplicationBuilder
    {
        IApplicationStarter CreateWith(ITestConfigurer configurer);
    }
}