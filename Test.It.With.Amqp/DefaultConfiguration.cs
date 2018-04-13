namespace Test.It.With.Amqp
{
    public class DefaultConfiguration : IConfiguration
    {
        public DefaultConfiguration(bool automaticReply = false)
        {
            AutomaticReply = automaticReply;
        }

        public bool AutomaticReply { get; }
    }
}