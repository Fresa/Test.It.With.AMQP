using Test.It.Hosting.A.ConsoleApplication.Consoles;
using Test.It.Specifications;

namespace Test.It.Hosting.A.ConsoleApplication
{
    public interface IConsoleApplicationFixture
    {
        IConsoleClient Start(ITestConfigurer testConfigurer);
    }
}