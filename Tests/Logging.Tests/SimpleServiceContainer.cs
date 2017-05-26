using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.It.Tests
{
    public class SimpleServiceContainer : IServiceContainer
    {
        private readonly Dictionary<Type, Func<object>> _dependencyResolver = new Dictionary<Type, Func<object>>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public void Register<TImplementation>(Func<TImplementation> configurer) where TImplementation : class
        {
            if (_dependencyResolver.TryGetValue(typeof(TImplementation), out _))
            {
                _dependencyResolver[typeof(TImplementation)] = configurer;
                return;
            }
            _dependencyResolver.Add(typeof(TImplementation), configurer);
        }

        public void Register<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Register<TService>(Activator.CreateInstance<TImplementation>);
        }

        public void RegisterSingleton<TImplementation>(Func<TImplementation> configurer) where TImplementation : class
        {
            Register(() => new Lazy<TImplementation>(configurer).Value);
        }

        public void RegisterSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Register<TService>(() => new Lazy<TImplementation>(Activator.CreateInstance<TImplementation>).Value);
        }

        public TService Resolve<TService>() where TService : class
        {
            var service = (TService) _dependencyResolver[typeof(TService)]();
            if (typeof(TService).GetInterfaces().Any(interfaceType => interfaceType == typeof(IDisposable)))
            {
                _disposables.Add((IDisposable) service);
            }
            return service;
        }

        public void Verify()
        {
            foreach (var create in _dependencyResolver.Values)
            {
                create();
            }
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}