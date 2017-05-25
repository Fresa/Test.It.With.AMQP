using Test.It.Specifications;

namespace Test.It.Fixtures
{
    public interface IConsoleApplicationFixture
    {
        IConsoleClient Start(ITestConfigurer testConfigurer);
    }
}