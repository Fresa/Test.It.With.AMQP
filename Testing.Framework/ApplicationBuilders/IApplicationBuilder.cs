using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.ApplicationBuilders
{
    public interface IApplicationBuilder
    {
        IApplicationStarter CreateWith(ITestConfigurer configurer);
    }

    public interface IApplicationBuilder<out TClient>
    {
        IApplicationStarter<TClient> CreateWith(ITestConfigurer configurer);
    }
}