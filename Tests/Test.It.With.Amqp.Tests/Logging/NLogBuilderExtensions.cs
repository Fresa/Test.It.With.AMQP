using NLog.Extensions.Logging;
using NLog.Web;
using Test.It.With.Amqp.Tests.System;
using Test.It.With.Amqp.Tests.TestFramework;

namespace Test.It.With.Amqp.Tests.Logging
{
    internal static class NLogBuilderExtensions
    {
        private static readonly ExclusiveLock NlogConfigurationLock =
            new();

        internal static void ConfigureNLogOnce(
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            if (!NlogConfigurationLock.TryAcquire())
            {
                return;
            }

            var nLogConfig = new NLogLoggingConfiguration(
                configuration.GetSection("NLog"));
            NLogBuilder.ConfigureNLog(nLogConfig);
        }
    }
}