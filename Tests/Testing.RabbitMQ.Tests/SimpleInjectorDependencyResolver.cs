using System;
using SimpleInjector;
using Testing.Framework;

namespace Testing.RabbitMQ.Tests
{
    public class SimpleInjectorDependencyResolver : IServiceContainer
    {
        private readonly Container _container;

        public SimpleInjectorDependencyResolver(Container container)
        {
            _container = container;
        }

        public void Register<TImplementation>(Func<TImplementation> configurer) 
            where TImplementation : class
        {
            _container.Register(configurer);
        }

        public void Register<TService, TImplementation>() 
            where TService : class 
            where TImplementation : class, TService
        {
            _container.Register<TService, TImplementation>();
        }

        public void RegisterSingleton<TImplementation>(Func<TImplementation> configurer) 
            where TImplementation : class
        {
            _container.RegisterSingleton(configurer);
        }

        public void RegisterSingleton<TService, TImplementation>() 
            where TService : class 
            where TImplementation : class, TService
        {
            _container.RegisterSingleton<TService, TImplementation>();
        }

        public TService Resolve<TService>() 
            where TService : class
        {
            return _container.GetInstance<TService>();
        }
    }
}