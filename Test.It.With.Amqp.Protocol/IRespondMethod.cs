namespace Test.It.With.Amqp.Protocol
{
    public interface IRespondMethod<TMethod> : IMethod 
        where TMethod : IMethod
    {
        TMethod Respond(TMethod method);
    }
}