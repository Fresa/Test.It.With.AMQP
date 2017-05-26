namespace Test.It.Hosting.A.ConsoleApplication.Consoles
{
    public interface IConsole
    {
        void WriteLine(string message);
        string ReadLine();
        string Title { get; set; }
    }
}