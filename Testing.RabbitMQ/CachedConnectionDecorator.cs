using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Test.It.With.RabbitMQ
{
    internal class CachedConnectionDecorator : IConnection
    {
        private readonly IConnection _connection;
        private Action _disposed;
        private readonly ConcurrentDictionary<Guid, IModel> _models = new ConcurrentDictionary<Guid, IModel>();

        public CachedConnectionDecorator(IConnection connection)
        {
            _connection = connection;

            _connection.CallbackException += CallbackException;
            _connection.ConnectionBlocked += ConnectionBlocked;
            _connection.ConnectionShutdown += ConnectionShutdown;
            _connection.ConnectionUnblocked += ConnectionUnblocked;
        }

        public event EventHandler<CallbackExceptionEventArgs> CallbackException;
        public event EventHandler<EventArgs> RecoverySucceeded;
        public event EventHandler<ConnectionRecoveryErrorEventArgs> ConnectionRecoveryError;
        public event EventHandler<ConnectionBlockedEventArgs> ConnectionBlocked;
        public event EventHandler<ShutdownEventArgs> ConnectionShutdown;
        public event EventHandler<EventArgs> ConnectionUnblocked;

        public int LocalPort => _connection.LocalPort;

        public int RemotePort => _connection.RemotePort;

        public void Dispose()
        {
            _connection.Dispose();
            _disposed();
        }

        public void Abort()
        {
            _connection.Abort();
        }

        public void Abort(ushort reasonCode, string reasonText)
        {
            _connection.Abort(reasonCode, reasonText);
        }

        public void Abort(int timeout)
        {
            _connection.Abort(timeout);
        }

        public void Abort(ushort reasonCode, string reasonText, int timeout)
        {
            _connection.Abort(reasonCode, reasonText, timeout);
        }

        public void Close()
        {
            _connection.Close();
        }

        public void Close(ushort reasonCode, string reasonText)
        {
            _connection.Close(reasonCode, reasonText);
        }

        public void Close(int timeout)
        {
            _connection.Close(timeout);
        }

        public void Close(ushort reasonCode, string reasonText, int timeout)
        {
            _connection.Close(reasonCode, reasonText, timeout);
        }

        public IModel CreateModel()
        {
            return Store(new DisposeNotifyingModelDecorator(_connection.CreateModel()));
        }

        public void HandleConnectionBlocked(string reason)
        {
            _connection.HandleConnectionBlocked(reason);
        }

        public void HandleConnectionUnblocked()
        {
            _connection.HandleConnectionUnblocked();
        }

        public bool AutoClose
        {
            get => _connection.AutoClose;
            set => _connection.AutoClose = value;
        }
        public ushort ChannelMax => _connection.ChannelMax;
        public IDictionary<string, object> ClientProperties => _connection.ClientProperties;
        public ShutdownEventArgs CloseReason => _connection.CloseReason;
        public AmqpTcpEndpoint Endpoint => _connection.Endpoint;
        public uint FrameMax => _connection.FrameMax;
        public ushort Heartbeat => _connection.Heartbeat;
        public bool IsOpen => _connection.IsOpen;
        public AmqpTcpEndpoint[] KnownHosts => _connection.KnownHosts;
        public IProtocol Protocol => _connection.Protocol;
        public IDictionary<string, object> ServerProperties => _connection.ServerProperties;
        public IList<ShutdownReportEntry> ShutdownReport => _connection.ShutdownReport;
        public string ClientProvidedName => _connection.ClientProvidedName;
        public ConsumerWorkService ConsumerWorkService => _connection.ConsumerWorkService;

        public void OnDispose(Action disposed)
        {
            _disposed = disposed;
        }

        public IReadOnlyList<IModel> CurrentModels => _models.Select(pair => pair.Value).ToList();

        private IModel Store(DisposeNotifyingModelDecorator model)
        {
            var modelId = Guid.NewGuid();
            _models.AddOrUpdate(modelId, model, (storedModelId, storedModel) => storedModel);
            model.OnDispose(() => _models.TryRemove(modelId, out var _));
            return model;
        }
    }
}