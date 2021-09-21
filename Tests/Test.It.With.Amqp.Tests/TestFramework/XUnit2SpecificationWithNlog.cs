using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Test.It.With.Amqp.Tests.Logging;
using Test.It.With.XUnit;
using Xunit.Abstractions;
using Logger = Test.It.With.Amqp.Logging.Logger;

namespace Test.It.With.Amqp.Tests.TestFramework
{
    public class XUnit2SpecificationWithNLog : XUnit2Specification
    {
        private readonly IDisposable _outputWriter;

        static XUnit2SpecificationWithNLog()
        {
            NLogBuilderExtensions.ConfigureNLogOnce(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build());
            NLogExtensions.RegisterLoggingOnce();
        }

        public XUnit2SpecificationWithNLog(ITestOutputHelper testOutputHelper) : base(testOutputHelper, false)
        {
            _outputWriter = XUnit.Output.WriteTo(testOutputHelper);
            Setup();
        }

        protected override void Dispose(bool disposing)
        {
            _outputWriter.Dispose();
            base.Dispose(disposing);
        }
    }
}