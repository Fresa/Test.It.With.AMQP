using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Test.It.With.XUnit
{
    internal class TestOutputHelperTextWriter : TextWriter
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private string _value = string.Empty;
        private readonly object _lock = new object();
        
        public TestOutputHelperTextWriter(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public override void WriteLine(string value)
        {
            lock (_lock)
            {
                _value += value;
            }
            WriteLine();
        }

        public override void WriteLine()
        {
            lock (_lock)
            {
                _testOutputHelper.WriteLine(_value);
                _value = string.Empty;
            }
        }

        public override void Write(char value)
        {
            lock (_lock)
            {
                _value += value;
            }
        }
        
        public override Encoding Encoding { get; } = Encoding.UTF8;
    }
}