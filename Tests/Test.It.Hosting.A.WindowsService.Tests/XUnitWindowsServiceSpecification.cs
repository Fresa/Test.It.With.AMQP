using System;

namespace Test.It.Hosting.A.WindowsService.Tests
{
    public class XUnitWindowsServiceSpecification<TFixture> : WindowsServiceSpecification<TFixture>, Xunit.IClassFixture<TFixture>, IDisposable
        where TFixture : class, IWindowsServiceFixture, new()
    {
        private readonly TFixture _fixture;
        
        public XUnitWindowsServiceSpecification()
        {
            _fixture = new TFixture();
            SetFixture(_fixture);
        }
        
        public void Dispose()
        {
            _fixture.Dispose();
        }
    }
}