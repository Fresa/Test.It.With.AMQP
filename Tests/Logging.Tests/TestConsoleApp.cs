﻿using System;

namespace Test.It.Tests
{
    public class TestConsoleApp
    {
        private static TestConsoleApp _app;
        private SimpleServiceContainer _serviceContainer;

        public TestConsoleApp(Action<IServiceContainer> reconfigurer)
        {
            _serviceContainer = new SimpleServiceContainer();

            reconfigurer(_serviceContainer);


        }

        public int Start(params string[] args)
        {
            var console = _serviceContainer.Resolve<IConsole>();
            console.WriteLine(console.ReadLine());
            return 0;
        }


        public static void Main(params string[] args)
        {
            _app = new TestConsoleApp(container => { });
            _app.Start(args);
        }


    }
}