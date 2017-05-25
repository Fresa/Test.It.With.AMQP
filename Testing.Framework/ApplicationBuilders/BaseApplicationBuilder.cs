﻿using Test.It.MessageClient;
using Test.It.Specifications;
using Test.It.Starters;

namespace Test.It.ApplicationBuilders
{
    public abstract class BaseApplicationBuilder : IApplicationBuilder
    {
        protected abstract IServiceContainer UseServiceContainer();

        protected abstract IApplicationStarter GetApplicationStarter();

        public IApplicationStarter CreateWith(ITestConfigurer configurer)
        {
            configurer.Configure(UseServiceContainer());
            return GetApplicationStarter();
        }
    }
}