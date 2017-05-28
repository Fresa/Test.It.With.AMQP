using System;

namespace Test.It.Hosting.A.WindowsService
{
    internal class WindowsServiceController : IWindowsServiceClient
    {
        private event EventHandler DisconnectedPrivate;
        public event EventHandler Disconnected
        {
            add
            {
                lock (_disconnectLock)
                {
                    if (_disconnected)
                    {
                        value.Invoke(this, null);
                    }
                    DisconnectedPrivate += value;
                }
            }
            remove
            {
                lock (_disconnectLock)
                {
                    DisconnectedPrivate -= value;
                }
            }
        }

        private readonly object _disconnectLock = new object();
        private bool _disconnected;

        public void Disconnect()
        {
            lock (_disconnectLock)
            {
                _disconnected = true;
            }
            DisconnectedPrivate?.Invoke(this, null);
        }
    }
}