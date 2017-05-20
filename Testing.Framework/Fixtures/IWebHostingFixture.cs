using System;
using System.Net.Http;
using Testing.Framework.Specifications;

namespace Testing.Framework.Fixtures
{
    public interface IWebHostingFixture : IDisposable
    {
        HttpClient Start(ITestConfigurer testConfigurer);
    }
}