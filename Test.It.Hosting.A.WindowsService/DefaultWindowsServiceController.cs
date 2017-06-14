using System;
using System.Collections.Generic;

namespace Test.It.Hosting.A.WindowsService
{
    internal class DefaultWindowsServiceController : IWindowsServiceController
    {
        public DefaultWindowsServiceController()
        {
            Client = new DefaultWindowsServiceClient(this);
        }

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
                if (_disconnected)
                {
                    return;
                }
                _disconnected = true;
            }
            DisconnectedPrivate?.Invoke(this, null);
        }

        private readonly List<Exception> _exceptionsRaised = new List<Exception>();
        private readonly object _exceptionLock = new object();
        private event EventHandler<Exception> OnExceptionPrivate;
        public event EventHandler<Exception> OnException
        {
            add
            {
                lock (_exceptionLock)
                {
                    foreach (var exception in _exceptionsRaised)
                    {
                        value.Invoke(this, exception);
                    }
                    OnExceptionPrivate += value;
                }
            }
            remove
            {
                lock (_exceptionLock)
                {
                    OnExceptionPrivate -= value;
                }
            }
        }

        public void RaiseException(Exception exception)
        {
            lock (_exceptionLock)
            {
                _exceptionsRaised.Add(exception);
            }
            OnExceptionPrivate?.Invoke(this, exception);
        }

        public IWindowsServiceClient Client { get; }
    }
}