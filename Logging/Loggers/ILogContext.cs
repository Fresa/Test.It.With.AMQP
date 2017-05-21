namespace Logging.Loggers
{
    public interface ILogContext
    {
        void Set(string key, string value);
        string Get(string key);
        void Remove(string key);
    }
}