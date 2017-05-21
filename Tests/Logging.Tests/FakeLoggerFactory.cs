﻿using FakeItEasy;

namespace Logging.Tests
{
    public class FakeLoggerFactory : ILoggerFactory
    {
        public ILogFactory Create()
        {
            var factory = A.Fake<ILogFactory>();
            A.CallTo(() => factory.Create<When_creating_a_logger>())
                .ReturnsLazily(() => new FakeLogger());
            return factory;
        }
    }
}