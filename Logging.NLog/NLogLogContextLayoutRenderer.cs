using System.Text;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace Log.It.With.NLog
{
    [LayoutRenderer("lc")]
    public class NLogLogContextLayoutRenderer : LayoutRenderer
    {
        private readonly ILogContext _logContext;

        public NLogLogContextLayoutRenderer(ILogContext logContext)
        {
            _logContext = logContext;
        }

        [RequiredParameter]
        [DefaultParameter]
        public string Key { get; set; }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var message = _logContext.Get<string>(Key);
            if (message != null)
            {
                builder.AppendFormat(logEvent.FormatProvider, message);
            }
        }
    }
}