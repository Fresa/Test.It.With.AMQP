using System;
using Test.It.Specifications;

namespace Test.It.Fixtures
{
    public interface IWindowsServiceFixture : IDisposable
    {
        void Start(ITestConfigurer testConfigurer);
    }
}