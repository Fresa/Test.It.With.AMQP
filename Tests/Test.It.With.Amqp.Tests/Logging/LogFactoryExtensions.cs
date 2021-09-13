using Log.It;
using Log.It.With.NLog;
using Test.It.With.Amqp.Tests.System;
using Test.It.With.Amqp.Tests.TestFramework;

namespace Test.It.With.Amqp.Tests.Logging
{
    internal static class LogFactoryExtensions
    {
        private static readonly ExclusiveLock Lock = new();

        public static void InitializeOnce()
        {
            if (!Lock.TryAcquire())
            {
                return;
            }

            if (LogFactory.HasFactory)
            {
                return;
            }

            LogFactory.Initialize(new NLogFactory(new LogicalThreadContext()));
        }
    }
}