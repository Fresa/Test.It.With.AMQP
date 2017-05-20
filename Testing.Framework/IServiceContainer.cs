using System;

namespace Testing.Framework
{
    public interface IServiceContainer
    {
        void Register<TImplementation>(Func<TImplementation> configurer) 
            where TImplementation : class;
        void Register<TService, TImplementation>() 
            where TService : class 
            where TImplementation : class, TService;
        void RegisterSingleton<TImplementation>(Func<TImplementation> configurer) 
            where TImplementation : class;
        void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
        TService Resolve<TService>() where TService : class;
    }
}