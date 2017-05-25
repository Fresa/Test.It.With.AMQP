using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.It.Middleware
{
    internal class TaskStartingMiddleware
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