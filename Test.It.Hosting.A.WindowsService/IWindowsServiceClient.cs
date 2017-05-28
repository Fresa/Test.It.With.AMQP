using System;

namespace Test.It.Hosting.A.WindowsService
{
    public interface IWindowsServiceClient
    {
        event EventHandler Disconnected;
    }
}