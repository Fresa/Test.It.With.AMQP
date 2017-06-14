using System;

namespace Test.It.Hosting.A.WindowsService
{
    public interface IWindowsServiceController
    {
        event EventHandler Disconnected;
        event EventHandler<Exception> OnException;
        void RaiseException(Exception exception);
        IWindowsServiceClient Client { get; }
    }

    public interface IWindowsServiceClient
    {
        void Disconnect();
    }
}