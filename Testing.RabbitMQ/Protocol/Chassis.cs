namespace Test.It.With.RabbitMQ.Protocol
{
    public class Chassis
    {
        public ChassisName Name { get; }
        public bool MustImplement { get; }

        public Chassis(ChassisName name, bool mustImplement)
        {
            Name = name;
            MustImplement = mustImplement;
        }
    }
}