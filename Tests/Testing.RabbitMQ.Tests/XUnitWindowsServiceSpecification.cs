using Test.It.Hosting.A.WindowsService;
using Xunit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class XUnitWindowsServiceSpecification<TFixture> : WindowsServiceSpecification<TFixture>, IClassFixture<TFixture> 
        where TFixture : class, IWindowsServiceFixture, new()
    {
        public XUnitWindowsServiceSpecification()
        {
            SetFixture(new TFixture());
        }
    }
}