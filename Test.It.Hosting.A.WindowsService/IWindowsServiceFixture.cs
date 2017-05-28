using System;
using Test.It.Specifications;

namespace Test.It.Hosting.A.WindowsService
{
    public interface IWindowsServiceFixture : IDisposable
    {
        IWindowsServiceClient Start(ITestConfigurer testConfigurer);
    }
}