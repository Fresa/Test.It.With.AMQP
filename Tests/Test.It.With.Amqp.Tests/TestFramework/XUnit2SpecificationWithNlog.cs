using Test.It.With.XUnit;
using Xunit.Abstractions;
using NLogCapturingTarget = Test.It.With.Amqp.Tests.MessageHandlers.NLogCapturingTarget;

namespace Test.It.With.Amqp.Tests.TestFramework
{
    public class XUnit2SpecificationWithNLog : XUnit2Specification
    {
        public XUnit2SpecificationWithNLog(ITestOutputHelper testOutputHelper) : base(testOutputHelper, false)
        {
            NLogCapturingTarget.Subscribe += TestOutputHelper.WriteLine;
            Setup();
        }

        protected override void Dispose(bool disposing)
        {
            NLogCapturingTarget.Subscribe -= TestOutputHelper.WriteLine;
            base.Dispose(disposing);
        }
    }
}