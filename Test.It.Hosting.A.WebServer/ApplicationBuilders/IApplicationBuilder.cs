using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.Hosting.A.WebServer.ApplicationBuilders
{
    public interface IApplicationBuilder
    {
        IApplicationStarter CreateWith(ITestConfigurer configurer);
    }
}