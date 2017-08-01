using Test.It.While.Hosting.Your.Windows.Service;
using Xunit;

namespace Test.It.With.RabbitMQ.Tests
{
    public class XUnitWindowsServiceSpecification<THostStarter> : WindowsServiceSpecification<THostStarter>, IClassFixture<THostStarter> 
        where THostStarter : class, IWindowsServiceHostStarter, new()
    {
        public XUnitWindowsServiceSpecification()
        {
            SetConfiguration(new THostStarter());
        }
    }
}