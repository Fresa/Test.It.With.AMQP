using Logging.Loggers;

namespace Logging
{
    public interface ILogFactory
    {
        ILogger Create(string logger);
        ILogger Create<T>();
    }
}