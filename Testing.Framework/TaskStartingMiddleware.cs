using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Test.It
{
    public class TaskStartingMiddleware
    {
        private readonly Action _bootup;

        public TaskStartingMiddleware(Action bootup)
        {
            _bootup = bootup;
        }

        public void Initialize(Func<IDictionary<string, object>, Task> next)
        {
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            await Task.Run(_bootup);
        }
    }
}