using System;

namespace Log.It
{
    public interface ILogContext
    {
        void Set<TValue>(string key, TValue value);
        TValue Get<TValue>(string key);
        string Get(string key, IFormatProvider formatProvider);
        void Remove(string key);
    }
}