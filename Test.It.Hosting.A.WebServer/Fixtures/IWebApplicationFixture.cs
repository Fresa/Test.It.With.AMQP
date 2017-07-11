using System;
using System.Net.Http;
using Test.It.Specifications;

namespace Test.It.Hosting.A.WebServer.Fixtures
{
    public interface IWebApplicationFixture : IDisposable
    {
        HttpClient Start(ITestConfigurer testConfigurer);
    }
}