using System;
using System.Collections.Generic;

namespace Test.It.Tests
{
    public class SimpleServiceContainer : IServiceContainer
    {
        private readonly Dictionary<Type, Func<object>> _dependencyResolver = new Dictionary<Type, Func<object>>();

        public void Dispose()
        {
        }
        
        public void Register<TImplementation>(Func<TImplementation> configurer) where TImplementation : class
        {
            if (_dependencyResolver.TryGetValue(typeof(TImplementation), out _))
            {
                _dependencyResolver[typeof(TImplementation)] = configurer;
            }
            _dependencyResolver.Add(typeof(TImplementation), configurer);
        }

        public void Register<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Register<TService>(Activator.CreateInstance<TImplementation>);
        }

        public void RegisterSingleton<TImplementation>(Func<TImplementation> configurer) where TImplementation : class
        {
            Register(configurer);
        }

        public void RegisterSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Register<TService, TImplementation>();
        }

        public TService Resolve<TService>() where TService : class
        {
            return (TService) _dependencyResolver[typeof(TService)]();
        }

        public void Verify()
        {
            
        }
    }
}