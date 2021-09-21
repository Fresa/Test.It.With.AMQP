using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.Logging
{
    public static class LogFactory
    {
        private static readonly ConcurrentQueue<(LogMessage, ExecutionContext)> LogMessages =
            new ConcurrentQueue<(LogMessage, ExecutionContext)>();

        private static int _logMessagesInFlight;
        private const int LogMessageBuffer = 1000;

        private static readonly SemaphoreSlim LogsAvailable = new SemaphoreSlim(0);
        private static Logger _logger;

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
            if (LogMessages.Count > LogMessageBuffer)
            {
                LogMessages.TryDequeue(out _);
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
                while (true)
                {
                    await LogsAvailable.WaitAsync()
                        .ConfigureAwait(false);
                    Send();
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
                    ExecutionContext.Run(executionContext,
                        _ => _logger.Fatal(message.LoggerName, message.Template, message.Arguments,
                            message.Exception), null);
                    break;
                case LogLevel.Trace:
                    ExecutionContext.Run(executionContext,
                        _ => _logger.Fatal(message.LoggerName, message.Template, message.Arguments,
                            message.Exception), null);
                    break;
                case LogLevel.Debug:
                    ExecutionContext.Run(executionContext,
                        _ => _logger.Fatal(message.LoggerName, message.Template, message.Arguments,
                            message.Exception), null);
                    break;
                case LogLevel.Info:
                    ExecutionContext.Run(executionContext,
                        _ => _logger.Fatal(message.LoggerName, message.Template, message.Arguments,
                            message.Exception), null);
                    break;
                case LogLevel.Warning:
                    ExecutionContext.Run(executionContext,
                        _ => _logger.Fatal(message.LoggerName, message.Template, message.Arguments,
                            message.Exception), null);
                    break;
                case LogLevel.Error:
                    ExecutionContext.Run(executionContext,
                        _ => _logger.Fatal(message.LoggerName, message.Template, message.Arguments,
                            message.Exception), null);
                    break;
            }

            Interlocked.Decrement(ref _logMessagesInFlight);
        }
    }
}