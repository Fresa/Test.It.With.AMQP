using System;
using System.Runtime.ExceptionServices;

namespace Test.It.With.Amqp.NetworkClient
{
    internal class InternalRoutedNetworkClientFactory
    {
        public INetworkClient Create(out INetworkClient serverNetworkClient)
        {
            var internalServerNetworkClient = new InternalRoutedNetworkClient();
            var clientNetworkClient = new InternalRoutedNetworkClient();

            void OnClientTriggerReceive(object sender, ReceivedEventArgs args)
            {
                try
                {
                    internalServerNetworkClient.TriggerReceive(sender, args);
                }
                catch (Exception ex)
                {
                    OnException?.Invoke(ex);

                    try
                    {
                        OnServerDisconnect(internalServerNetworkClient, EventArgs.Empty);
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

            internalServerNetworkClient.SendReceived += clientNetworkClient.TriggerReceive;
            internalServerNetworkClient.Disconnected += OnServerDisconnect;

            void OnClientDisconnected(object sender, EventArgs args)
            {
                internalServerNetworkClient.SendReceived -= clientNetworkClient.TriggerReceive;
                internalServerNetworkClient.Disconnected -= OnServerDisconnect;
            }
            
            clientNetworkClient.SendReceived += OnClientTriggerReceive;
            clientNetworkClient.Disconnected += OnClientDisconnected;

            serverNetworkClient = internalServerNetworkClient;
            return clientNetworkClient;
        }
        
        public event Action<Exception> OnException;        
    }
}