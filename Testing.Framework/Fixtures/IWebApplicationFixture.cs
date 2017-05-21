using System;
using System.Net.Http;
using Testing.Framework.Specifications;

namespace Testing.Framework.Fixtures
{
    public interface IWebApplicationFixture : IDisposable
    {
        HttpClient Start(ITestConfigurer testConfigurer);
    }
}