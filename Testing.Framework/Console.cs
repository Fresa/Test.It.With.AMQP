namespace Test.It
{
    public class Console : IConsole
    {
        public string Title
        {
            get => System.Console.Title;
            set => System.Console.Title = value;
        }

        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }
    }
}