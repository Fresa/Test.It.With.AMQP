using System;
using System.Diagnostics;
using Test.It.Specifications;
using Xunit.Abstractions;

namespace Test.It.With.XUnit
{
    public abstract class XUnit2Specification : Specification, IDisposable
    {
        protected readonly ITestOutputHelper Output;

        protected XUnit2Specification() 
            : this(new DummyTestOutputHelper())
        {
        }

        protected XUnit2Specification(ITestOutputHelper output)
        {
            Output = output;

            var outputWriter = new TestOutputHelperTextWriter(output);
            Console.SetOut(outputWriter);
            Trace.Listeners.Add(new ConsoleTraceListener());

            Setup();
        }

        protected event EventHandler OnDisposing;

        public void Dispose()
        {
            OnDisposing?.Invoke(this, null);
        }
    }
}