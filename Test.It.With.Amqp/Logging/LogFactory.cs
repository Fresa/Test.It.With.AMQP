using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.Logging
{
    public static class LogFactory
    {
        private static readonly ConcurrentQueue<(LogMessage, ExecutionContext)> LogMessages =
            new ConcurrentQueue<(LogMessage, ExecutionContext)>();

        private static readonly CancellationTokenSource Cts = new CancellationTokenSource();
        private static CancellationToken CancellationToken => Cts.Token;

        private static int _logMessagesInFlight;
        private const int LogMessageBuffer = 1000;

        private static readonly SemaphoreSlim LogsAvailable = new SemaphoreSlim(0);
        private static Logger _logger;

        static LogFactory()
        {
            AppDomain.CurrentDomain.DomainUnload += (sender, args) =>
            {
                try
                {
                    Cts.Cancel();
                }
                finally
                {
                    LogsAvailable.Dispose();
                    Cts.Dispose();    
                }
            };
        }

        public static bool TryInitializeOnce(Logger logger)
        {
            if (Interlocked.CompareExchange(ref _logger, logger, null) != null)
            {
                return false;
            }

            _logger = logger;
            StartSendingLogs();
            return true;
        }

        public static void Flush()
        {
            if (_logger == null)
            {
                return;
            }

            while (_logMessagesInFlight > 0)
            {
                if (LogsAvailable.Wait(0))
                {
                    Send();
                }
            }
        }

        internal static InternalLogger Create<T>()
        {
            return new InternalLogger(typeof(T).GetPrettyName(), EnqueueLogMessage);
        }

        internal static InternalLogger Create(string logger)
        {
            return new InternalLogger(logger, EnqueueLogMessage);
        }

        private static void EnqueueLogMessage(LogMessage message)
        {
            if (LogMessages.Count > LogMessageBuffer &&
                LogMessages.TryDequeue(out _))
            {
                LogMessages.Enqueue((message, ExecutionContext.Capture()));
                return;
            }

            Interlocked.Increment(ref _logMessagesInFlight);
            LogMessages.Enqueue((message, ExecutionContext.Capture()));
            LogsAvailable.Release();
        }

        private static void StartSendingLogs()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (!Cts.IsCancellationRequested)
                    {
                        await LogsAvailable.WaitAsync(CancellationToken)
                            .ConfigureAwait(false);
                        Send();
                    }
                }
                catch when (CancellationToken.IsCancellationRequested)
                {
                }
                catch (Exception ex)
                {
                    _logger?.Fatal(typeof(LogFactory).GetPrettyName(), "Caught unhandled exception when sending logs",
                        new object[0], ex);
                    _logger = null;
                }
            });
        }

        private static void Send()
        {
            if (!LogMessages.TryDequeue(out (LogMessage Message, ExecutionContext ExecutionContext) pair))
            {
                return;
            }

            var message = pair.Message;
            var executionContext = pair.ExecutionContext;
            switch (message.LogLevel)
            {
                case LogLevel.Fatal:
                    Log(_logger.Fatal);
                    break;
                case LogLevel.Trace:
                    Log(_logger.Trace);
                    break;
                case LogLevel.Debug:
                    Log(_logger.Debug);
                    break;
                case LogLevel.Info:
                    Log(_logger.Info);
                    break;
                case LogLevel.Warning:
                    Log(_logger.Warning);
                    break;
                case LogLevel.Error:
                    Log(_logger.Error);
                    break;
            }

            Interlocked.Decrement(ref _logMessagesInFlight);

            void Log(Action<string, string, object[], Exception> log)
            {
                ExecutionContext.Run(executionContext,
                    _ => log(message.LoggerName, message.Template, message.Arguments,
                        message.Exception), null);
            }
        }
    }
}