namespace Logging
{
    public interface ILoggerFactory
    {
        ILogFactory Create();
    }
}