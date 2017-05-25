using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.ApplicationBuilders
{
    public interface IConsoleApplicationBuilder<out TClient>
        where TClient : IConsoleClient
    {
        IApplicationStarter<TClient> CreateWith(ITestConfigurer configurer);
    }
}