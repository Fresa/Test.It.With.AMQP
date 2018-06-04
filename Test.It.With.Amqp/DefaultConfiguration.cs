namespace Test.It.With.Amqp
{
    internal class DefaultConfiguration : IConfiguration
    {
        public DefaultConfiguration(bool automaticReply = false)
        {
            AutomaticReply = automaticReply;
        }

        public bool AutomaticReply { get; }
    }
}