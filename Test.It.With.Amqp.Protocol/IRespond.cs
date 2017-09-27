namespace Test.It.With.Amqp.Protocol
{
    public interface IRespond<TMethod>
        where TMethod : IMethod
    {
        TMethod Respond(TMethod method);
    }
}