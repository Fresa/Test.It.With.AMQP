using System;
using System.Runtime.ExceptionServices;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class InternalRoutedNetworkClientFactory : INetworkClientFactory, IDisposable
    {
        private readonly InternalRoutedNetworkClient _serverNetworkClient;

        public InternalRoutedNetworkClientFactory(out INetworkClient serverNetworkClient)
        {
            // todo: should generate a new server client on each network creation
            serverNetworkClient = _serverNetworkClient = new InternalRoutedNetworkClient();
        }

        public INetworkClient Create()
        {
            var clientNetworkClient = new InternalRoutedNetworkClient();

            void OnClientTriggerReceive(object sender, ReceivedEventArgs args)
            {
                try
                {
                    _serverNetworkClient.TriggerReceive(sender, args);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(ex);

                    try
                    {
                        OnServerDisconnect(_serverNetworkClient, EventArgs.Empty);
                    }
                    catch (Exception onDisconnectException)
                    {
                        ex = new AggregateException(ex, onDisconnectException);
                    }

                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            void OnServerDisconnect(object sender, EventArgs args)
            {
                clientNetworkClient.SendReceived -= OnClientTriggerReceive;
                clientNetworkClient.Dispose();
            }

            _serverNetworkClient.SendReceived += clientNetworkClient.TriggerReceive;
            _serverNetworkClient.Disconnected += OnServerDisconnect;

            void OnClientDisconnected(object sender, EventArgs args)
            {
                _serverNetworkClient.SendReceived -= clientNetworkClient.TriggerReceive;
                _serverNetworkClient.Disconnected -= OnServerDisconnect;
            }
            
            clientNetworkClient.SendReceived += OnClientTriggerReceive;
            clientNetworkClient.Disconnected += OnClientDisconnected;

            return clientNetworkClient;
        }
        
        public event Action<Exception> OnException;

        public void Dispose()
        {
            _serverNetworkClient.Dispose();
        }
    }
}