namespace Testing.Framework
{
    public interface IUseFixture<in TFixture>
    {
        void SetFixture(TFixture fixture);
    }
}