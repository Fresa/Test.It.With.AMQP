using System.Text;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace Logging.Loggers.NLog
{
    [LayoutRenderer("ltc")]
    public class NLogLogicalThreadContextLayoutRenderer : LayoutRenderer
    {
        [RequiredParameter]
        [DefaultParameter]
        public string Item { get; set; }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var message = NLogLogicalThreadContext.Get(Item);
            builder.AppendFormat(logEvent.FormatProvider, message);
        }
    }
}