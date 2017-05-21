namespace Logging.Loggers.NLog
{
    public class NLogLoggerFactory : ILoggerFactory
    {
        public ILogFactory Create()
        {
            return new NLogFactory();
        }
    }
}