using System;
using Xunit.Abstractions;

namespace Test.It.With.XUnit
{
    internal class DummyTestOutputHelper : ITestOutputHelper
    {
        public void WriteLine(string message)
        {
            WriteLine(message, new object[0]);
        }

        public void WriteLine(string format, params object[] args)
        {
            throw new InvalidOperationException($"Please use a constructor with an argument of type '{typeof(ITestOutputHelper).FullName}' in order to use the output functionality in XUnit.");
        }
    }
}