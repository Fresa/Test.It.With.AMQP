using System;
using Test.It.Specifications;

namespace Test.It
{
    public class SimpleTestConfigurer : ITestConfigurer
    {
        private readonly Action<IServiceContainer> _configurer;

        public SimpleTestConfigurer(Action<IServiceContainer> configurer)
        {
            _configurer = configurer;
        }
        
        public void Configure(IServiceContainer serviceContainer)
        {
            _configurer(serviceContainer);
        }
    }
}