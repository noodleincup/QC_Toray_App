// Pseudocode / Plan:
// 1. Create a reusable async TCP client service class `TcpClientService`.
// 2. Provide methods: ConnectAsync(host, port, cancellation), DisconnectAsync(), SendAsync(message).
// 3. Start a receive loop after connecting that reads newline-delimited messages using a StreamReader.
// 4. Expose events/callbacks: OnMessageReceived(string), OnConnectionChanged(bool).
// 5. Use CancellationTokenSource to stop the receive loop and to cancel operations on disconnect.
// 6. Use locking to serialize writes to the network stream and ensure proper disposal.
// 7. Provide IsConnected property and implement IDisposable / IAsyncDisposable for cleanup.
// 8. Keep the API minimal and easy to use from a WPF viewmodel or window.
// Note: This implementation expects the server to use '\n' (LF) or "\r\n" line endings for messages.
// Example usage:
//   var client = new TcpClientService();
//   client.OnMessageReceived += msg => /* handle */;
//   await client.ConnectAsync("127.0.0.1", 9000, CancellationToken.None);
//   await client.SendAsync("Hello server");
//   await client.DisconnectAsync();

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QC_Toray_App_v3.Network
{
    public sealed class TcpClientService : IDisposable, IAsyncDisposable
    {
        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private CancellationTokenSource? _receiveCts;
        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private bool _disposed;

        // Callbacks
        public event Action<string>? OnMessageReceived;
        public event Action<bool>? OnConnectionChanged;

        public bool IsConnected { get; private set; }

        public async Task ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await DisconnectInternalAsync().ConfigureAwait(false);

            _tcpClient = new TcpClient();
            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var connectTask = _tcpClient.ConnectAsync(host, port);
                using (linkedCts.Token.Register(() => _tcpClient?.Close()))
                {
                    await connectTask.ConfigureAwait(false);
                }

                _networkStream = _tcpClient.GetStream();
                _reader = new StreamReader(_networkStream, Encoding.UTF8, leaveOpen: true);
                _writer = new StreamWriter(_networkStream, Encoding.UTF8, leaveOpen: true)
                {
                    AutoFlush = true
                };

                IsConnected = true;
                OnConnectionChanged?.Invoke(true);

                _receiveCts = new CancellationTokenSource();
                _ = Task.Run(() => ReceiveLoopAsync(_receiveCts.Token), CancellationToken.None);
            }
            catch
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task SendAsync(string message, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (!IsConnected || _writer is null)
                throw new InvalidOperationException("Not connected to server.");

            await _sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Ensure newline termination so the server can parse line-based messages
                if (!message.EndsWith("\n"))
                    message += "\n";

                await _writer.WriteAsync(message.AsMemory(), cancellationToken).ConfigureAwait(false);
                await _writer.FlushAsync().ConfigureAwait(false);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public async Task DisconnectAsync()
        {
            ThrowIfDisposed();
            await DisconnectInternalAsync().ConfigureAwait(false);
        }

        private async Task DisconnectInternalAsync()
        {
            if (_receiveCts != null && !_receiveCts.IsCancellationRequested)
            {
                try { _receiveCts.Cancel(); } catch { }
            }

            IsConnected = false;
            OnConnectionChanged?.Invoke(false);

            // Close and dispose streams/clients
            try
            {
                _writer?.Close();
            }
            catch { }
            try
            {
                _reader?.Close();
            }
            catch { }

            try
            {
                _networkStream?.Close();
            }
            catch { }

            try
            {
                _tcpClient?.Close();
            }
            catch { }

            _writer = null;
            _reader = null;
            _networkStream = null;
            _tcpClient = null;

            // small delay to ensure underlying sockets are cleaned up in some environments
            await Task.Yield();
        }

        private async Task ReceiveLoopAsync(CancellationToken token)
        {
            var reader = _reader;
            if (reader is null)
                return;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    byte[] buffer = new byte[1024];

                    int bytesRead = await _networkStream!.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);

                    string byteAsString = BitConverter.ToString(buffer, 0, bytesRead).Replace("-", " ");

                    string line = System.Text.Encoding.UTF8.GetString(getCleanDataBytes(buffer, bytesRead));

                    if (line is null)
                    {
                        // Stream closed by remote
                        break;
                    }

                    OnMessageReceived?.Invoke(line);
                }
            }
            catch (OperationCanceledException) { /* expected on cancellation */ }
            catch (IOException)
            {
                // network error - treat as disconnect
            }
            catch
            {
                // swallow other exceptions to avoid crashing background task
            }
            finally
            {
                // Ensure state is updated and connection closed
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
        }

        private byte[] getCleanDataBytes(byte[] data, int bytesRead)
        {
            // Trim STX (0x02) and ETX (0x03)
            int startIndex = 0;
            int endIndex = bytesRead;

            // Check first byte
            if (bytesRead > 0 && data[0] == 0x02)
                startIndex++;

            // Check last byte
            if (bytesRead > 1 && data[bytesRead - 1] == 0x03)
                endIndex--;

            // Create clean data array
            byte[] cleanData = new byte[endIndex - startIndex];
            Array.Copy(data, startIndex, cleanData, 0, cleanData.Length);
            return cleanData;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(TcpClientService));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try { _receiveCts?.Cancel(); } catch { }
            _receiveCts?.Dispose();
            _sendLock.Dispose();

            _writer?.Dispose();
            _reader?.Dispose();
            _networkStream?.Dispose();
            _tcpClient?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            try { _receiveCts?.Cancel(); } catch { }
            if (_receiveCts != null) _receiveCts.Dispose();

            _writer?.Dispose();
            _reader?.Dispose();
            _networkStream?.Dispose();
            _tcpClient?.Dispose();

            _sendLock.Dispose();

            await Task.CompletedTask;
        }
    }
}