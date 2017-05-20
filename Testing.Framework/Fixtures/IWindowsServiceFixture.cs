using System;
using Testing.Framework.Specifications;

namespace Testing.Framework.Fixtures
{
    public interface IWindowsServiceFixture : IDisposable
    {
        void Start(ITestConfigurer testConfigurer);
    }
}