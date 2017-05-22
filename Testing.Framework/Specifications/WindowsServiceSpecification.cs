﻿using Testing.Framework.Fixtures;

namespace Testing.Framework.Specifications
{
    public abstract class WindowsServiceSpecification<TWindowsServiceFixture> : IUseFixture<TWindowsServiceFixture>
        where TWindowsServiceFixture : IWindowsServiceFixture, new()
    {
        public void SetFixture(TWindowsServiceFixture fixture)
        {
            fixture.Start(new IntegrationSpecificationConfigurer(new IntegrationSpecification(Given, When)));

            When();
        }

        protected virtual void Given(IServiceContainer configurer) { }
        protected virtual void When() { }
    }
}