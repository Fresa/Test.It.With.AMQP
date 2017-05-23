using Test.It.Specifications;

namespace Test.It.Fixtures
{
    public interface IConsoleApplicationFixture
    {
        IClient Start(ITestConfigurer testConfigurer);
    }
}