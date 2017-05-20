using Testing.Framework.Specifications;

namespace Testing.Framework.Fixtures
{
    public interface IConsoleApplicationFixture
    {
        IClient Start(ITestConfigurer testConfigurer);
    }
}