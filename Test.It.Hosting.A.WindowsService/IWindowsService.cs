namespace Test.It.Hosting.A.WindowsService
{
    public interface IWindowsService
    {
        int Start(params string[] args);
        void Stop();
    }
}