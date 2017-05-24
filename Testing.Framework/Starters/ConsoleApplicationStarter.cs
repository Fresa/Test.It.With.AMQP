using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owin;
using Test.It.MessageClient;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Test.It.Starters
{
    public abstract class ConsoleApplicationStarter : IApplicationStarter
    {
        protected abstract ITypedMessageClient<string, string> GetClient();
        protected abstract Action Starter { get; }

        public void Start(IAppBuilder applicationBuilder)
        {
            applicationBuilder.Use(new TaskStartingMiddleware(Starter), GetClient());
            //applicationBuilder.Use<ITypedMessageClient<string, string>>(GetClient());

        }
    }

    public class TaskStartingMiddleware
    {
        private readonly Action _bootup;

        public TaskStartingMiddleware(Action bootup)
        {
            _bootup = bootup;
        }

        public void Initialize(AppFunc next)
        {
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            await Task.Run(_bootup);
        }
    }
    
}