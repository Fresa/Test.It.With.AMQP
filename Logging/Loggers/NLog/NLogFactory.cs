namespace Logging.Loggers.NLog
{
    public class NLogFactory : ILogFactory
    {
        public ILogger Create(string logger)
        {
            return new NLogLogger(logger);
        }

        public ILogger Create<T>()
        {
            return new NLogLogger(typeof(T).FullName);
        }
    }
}