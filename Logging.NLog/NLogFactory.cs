namespace Log.It.With.NLog
{
    public class NLogFactory : ILogFactory
    {
        private readonly ILogContext _logContext;

        public NLogFactory(ILogContext logContext)
        {
            _logContext = logContext;
        }

        public ILogger Create(string logger)
        {
            return new NLogLogger(logger, _logContext);
        }

        public ILogger Create<T>()
        {
            return new NLogLogger(typeof(T).FullName, _logContext);
        }
    }
}