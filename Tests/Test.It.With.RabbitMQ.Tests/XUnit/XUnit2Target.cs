using NLog;
using NLog.Targets;
using Xunit.Abstractions;

namespace Test.It.With.RabbitMQ.Tests.XUnit
{
    [Target("XUnit2")]
    public class XUnit2Target : TargetWithLayout
    {
        private readonly ITestOutputHelper _writer;

        public XUnit2Target(ITestOutputHelper writer)
        {
            _writer = writer;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = Layout.Render(logEvent);

            _writer.WriteLine(logMessage);
        }
    }
}