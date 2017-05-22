namespace Logging.NLog
{
    public class NLogLoggerFactory : ILoggerFactory
    {
        public ILogFactory Create()
        {
            return new NLogFactory(new LogicalThreadContext());
        }
    }
}