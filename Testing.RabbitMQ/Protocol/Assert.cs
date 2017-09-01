using System;

namespace Test.It.With.RabbitMQ.Protocol
{
    internal class Assert
    {
        private readonly Lazy<Field> _fieldResolver;

        public Assert(string check, Lazy<Field> fieldResolver)
        {
            _fieldResolver = fieldResolver;
            Check = check;
        }

        public string Check { get; }
        public string Value { get; set; }
        public Field Field => _fieldResolver.Value;
    }
}