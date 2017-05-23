namespace Test.It
{
    public interface IUseFixture<in TFixture>
    {
        void SetFixture(TFixture fixture);
    }
}