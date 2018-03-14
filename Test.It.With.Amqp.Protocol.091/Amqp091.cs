namespace Test.It.With.Amqp.Protocol
{
    public static class Amqp091
    {
        public static IProtocolResolver ProtocolResolver => Amqp091ProtocolResolver.Create();
    }
}