namespace Test.It
{
    public interface IConsole
    {
        void WriteLine(string message);
        string ReadLine();
        string Title { get; set; }
    }
}