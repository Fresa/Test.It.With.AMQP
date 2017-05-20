using System;

namespace Testing.Framework.Specifications
{
    internal class IntegrationSpecification
    {
        private readonly Action<IServiceContainer> _given;
        private readonly Action _when;

        public IntegrationSpecification(Action<IServiceContainer> given, Action @when)
        {
            _given = given;
            _when = when;
        }


        public void Given(IServiceContainer configurer)
        {
            _given(configurer);
        }

        public void When()
        {
            _when();
        }
    }
}