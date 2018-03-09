using System;
using Log.It;
using Log.It.With.NLog;
using NLog.Config;
using Test.It.While.Hosting.Your.Windows.Service;
using Xunit;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests.XUnit
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

            var defaultInstanceCreator = ConfigurationItemFactory.Default.CreateInstance;
            ConfigurationItemFactory.Default.CreateInstance = type =>
            {
                if (type == typeof(XUnit2Target))
                {
                    return new XUnit2Target(Output);
                }
                if (type == typeof(NLogLogContextLayoutRenderer))
                {
                    return new NLogLogContextLayoutRenderer(new LogicalThreadContext());
                }

                return defaultInstanceCreator(type);
            };

            SetConfiguration(new THostStarter());
        }
    }
}