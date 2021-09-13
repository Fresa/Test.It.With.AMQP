using System;
using System.IO;
using System.Threading;
using Log.It;
using Log.It.With.NLog;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using NLog.Web;
using Test.It.With.XUnit;
using Xunit.Abstractions;
using Xunit.Sdk;
using NLogCapturingTarget = Test.It.With.Amqp.Tests.MessageHandlers.NLogCapturingTarget;

namespace Test.It.With.Amqp.Tests.TestFramework
{
    public class XUnit2SpecificationWithNLog : XUnit2Specification
    {
        private readonly IDisposable _outputWriter;

        static XUnit2SpecificationWithNLog()
        {
            LogFactoryExtensions.InitializeOnce();
            NLogBuilderExtensions.ConfigureNLogOnce(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build());
            NLogCapturingTargetExtensions.RegisterOutputOnce();
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

    internal sealed class ExclusiveLock
    {
        private int _acquired;
        internal IDisposable TryAcquire(out bool acquired)
        {
            if (Interlocked.CompareExchange(ref _acquired, 1, 0) == 1)
            {
                acquired = false;
                return new DisposableActions(() => { });
            }

            acquired = true;
            return new DisposableActions(
                () =>
                {
                    Interlocked.Exchange(ref _acquired, 0);
                });
        }

        internal bool TryAcquire()
        {
            TryAcquire(out var acquired);
            return acquired;
        }
    }

    internal class DisposableActions : IDisposable
    {
        private readonly Action[] _dispose;

        internal DisposableActions(
            params Action[] dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            foreach (var dispose in _dispose)
            {
                dispose();
            }
        }
    }
}