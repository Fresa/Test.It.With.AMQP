using Test.It.With.Amqp.Tests.MessageHandlers;
using Test.It.With.Amqp.Tests.System;
using Test.It.With.Amqp.Tests.TestFramework;

namespace Test.It.With.Amqp.Tests.Logging
{
    internal static class NLogCapturingTargetExtensions
    {
        private static readonly ExclusiveLock NLogCapturingTargetLock =
            new();
        internal static void RegisterOutputOnce()
        {
            if (NLogCapturingTargetLock.TryAcquire())
            {
                NLogCapturingTarget.Subscribe += Output.Writer.Write;
            }
        }
    }
}