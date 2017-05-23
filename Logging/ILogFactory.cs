namespace Log.It
{
    public interface ILogFactory
    {
        ILogger Create(string logger);
        ILogger Create<T>();
    }
}