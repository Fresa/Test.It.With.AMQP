using NLog;
using NLog.Targets;

namespace Log.It.With.NLog.Tests
{
    [Target("Test")]
    public sealed class TestTarget : TargetWithLayout
    {
        private readonly IWrite _writer;
       
        public TestTarget(IWrite writer)
        {
            _writer = writer;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = Layout.Render(logEvent);

            _writer.Write(logMessage);
        }
    }
}