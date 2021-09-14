using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Test.It.With.Amqp.System;

namespace Test.It.With.Amqp.NetworkClient
{
    internal sealed class SocketNetworkClient : IStartableNetworkClient
    {
        private readonly Socket _socket;

        public SocketNetworkClient(Socket socket)
        {
            _socket = socket;
        }

        public void Dispose()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            finally
            {
                _socket.Dispose();
            }
        }

        public void Send(byte[] buffer, int offset, int count)
        {
            try
            {
                _socket.Send(buffer, offset, count, SocketFlags.None);
            }
            catch (Exception) when (!_socket.Connected)
            {
                Disconnected?.Invoke(this, EventArgs.Empty);

                throw;
            }
        }

        public IAsyncDisposable StartReceiving()
        {
            if (BufferReceived == null)
            {
                throw new ArgumentNullException(nameof(BufferReceived), "There is no message receiver registered");
            }

            var cancellationTokenSource =
                new CancellationTokenSource();

            // ReSharper disable once MethodSupportsCancellation
            // Handled by the returned disposable action
            var receivingTask = Task.Run(
                async () =>
                {
                    var buffer = new ArraySegment<byte>(new byte[_socket.ReceiveBufferSize]);
                    while (cancellationTokenSource.IsCancellationRequested == false)
                    {
                        try
                        {
                            var length = await _socket
                                .ReceiveAsync(buffer, SocketFlags.None)
                                .ConfigureAwait(false);

                            BufferReceived.Invoke(this, new ReceivedEventArgs(buffer.Array, 0, length));
                        }
                        catch when (cancellationTokenSource.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                });

            return new AsyncDisposableAction(() =>
            {
                cancellationTokenSource.Cancel();
                return new ValueTask(receivingTask);
            });
        }

        public event EventHandler<ReceivedEventArgs> BufferReceived;
        public event EventHandler Disconnected;
    }
}