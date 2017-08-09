using System;
using Test.It.While.Hosting.Your.Windows.Service;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests
{
    public abstract class XUnitWindowsServiceSpecification<THostStarter> : WindowsServiceSpecification<THostStarter>, IClassFixture<THostStarter> 
        where THostStarter : class, IWindowsServiceHostStarter, new()
    {
        protected ITestOutputHelper Output { get; }
        
        protected XUnitWindowsServiceSpecification(ITestOutputHelper output)
        {
            Output = output;
            var outputWriter = new TestOutputHelperTextWriter(output);
            Console.SetOut(outputWriter);

            SetConfiguration(new THostStarter());
        }
    }
}