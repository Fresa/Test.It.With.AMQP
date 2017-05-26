using Test.It.Hosting.A.ConsoleApplication.Consoles;
using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.Hosting.A.ConsoleApplication
{
    public interface IConsoleApplicationBuilder
    {
        IApplicationStarter<IConsoleClient> CreateWith(ITestConfigurer configurer);
    }
}