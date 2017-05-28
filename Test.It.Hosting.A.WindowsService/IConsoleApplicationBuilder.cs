using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.Hosting.A.WindowsService
{
    public interface IWindowsServiceBuilder
    {
        IApplicationStarter<IWindowsServiceClient> CreateWith(ITestConfigurer configurer);
    }
}