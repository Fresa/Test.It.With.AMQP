using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Test.It.With.Amqp.NetworkClient
{
    internal sealed class SocketNetworkClient : INetworkClient
    {
        private readonly Socket _socket;
        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private Task _receivingTask;

        public SocketNetworkClient(Socket socket)
        {
            _socket = socket;
        }

        public void Dispose()
        {
            try
            {
                _cancellationTokenSource.Cancel();
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _receivingTask?.GetAwaiter().GetResult();
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
            catch (Exception)
            {
                if (!_socket.Connected)
                {
                    Disconnected?.Invoke(this, EventArgs.Empty);
                }

                throw;
            }
        }

        public void StartReceiving()
        {
            if (BufferReceived == null)
            {
                throw new ArgumentNullException(nameof(BufferReceived), "There is no message receiver registered");
            }

            _receivingTask = Task.Run(
                async () =>
                {
                    var buffer = new ArraySegment<byte>(new byte[_socket.ReceiveBufferSize]);
                    while (_cancellationTokenSource.IsCancellationRequested == false)
                    {
                        try
                        {
                            var length = await _socket
                                .ReceiveAsync(buffer, SocketFlags.None)
                                .ConfigureAwait(false);

                            BufferReceived.Invoke(this, new ReceivedEventArgs(buffer.Array, 0, length));
                        }
                        catch when (_cancellationTokenSource.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                });
        }


        public event EventHandler<ReceivedEventArgs> BufferReceived;
        public event EventHandler Disconnected;
    }
}