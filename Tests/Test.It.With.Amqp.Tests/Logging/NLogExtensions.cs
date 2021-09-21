using System;
using Test.It.With.Amqp.Tests.MessageHandlers;
using Test.It.With.Amqp.Tests.System;

namespace Test.It.With.Amqp.Tests.Logging
{
    internal static class NLogExtensions
    {
        private static readonly ExclusiveLock NLogCapturingTargetLock =
            new();
        internal static void RegisterLoggingOnce()
        {
            if (!NLogCapturingTargetLock.TryAcquire())
            {
                return;
            }

            NLogCapturingTarget.Subscribe += Output.Writer.Write;
            Amqp.Logging.Logger.OnLog += NLogLogger.Log;
            
            AppDomain.CurrentDomain.DomainUnload += (_, _) =>
            {
                NLogCapturingTarget.Subscribe -= Output.Writer.Write;
                Amqp.Logging.Logger.OnLog -= NLogLogger.Log;
            };
        }
    }
}