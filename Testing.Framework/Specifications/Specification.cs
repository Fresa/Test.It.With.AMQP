namespace Testing.Framework.Specifications
{
    public abstract class Specification
    {
        protected void Setup()
        {
            Given();
            When();
        }

        protected virtual void Given() { }
        protected virtual void When() { }
    }
}
