namespace Testing.Framework.Specifications
{
    internal class IntegrationSpecificationConfigurer : ITestConfigurer
    {
        private readonly IntegrationSpecification _webSpecification;

        public IntegrationSpecificationConfigurer(IntegrationSpecification webSpecification)
        {
            _webSpecification = webSpecification;
        }
        
        public void Configure(IServiceContainer serviceContainer)
        {
            _webSpecification.Given(serviceContainer);
        }
    }
}