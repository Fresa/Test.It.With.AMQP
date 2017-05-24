using Test.It.MessageClient;
using Test.It.Specifications;

namespace Test.It.Fixtures
{
    public interface IConsoleApplicationFixture
    {
        ITypedMessageClient<string, string> Start(ITestConfigurer testConfigurer);
    }
}