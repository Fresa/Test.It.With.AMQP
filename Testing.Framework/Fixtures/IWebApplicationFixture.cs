using System;
using System.Net.Http;
using Test.It.Specifications;

namespace Test.It.Fixtures
{
    public interface IWebApplicationFixture : IDisposable
    {
        HttpClient Start(ITestConfigurer testConfigurer);
    }
}