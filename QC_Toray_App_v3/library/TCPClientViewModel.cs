using QC_Toray_App_v3.Network;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace QC_Toray_App_v3
{
    /// <summary>
    /// App-wide single TCP client view-model/service.
    /// Exposes events for UI to subscribe and logs caller/stack when Connect/Send are invoked.
    /// </summary>
    public sealed class TCPClientViewModel : IDisposable, IAsyncDisposable
    {
        private static readonly Lazy<TCPClientViewModel> _instance = new(() => new TCPClientViewModel());
        public static TCPClientViewModel Instance => _instance.Value;

        private readonly TcpClientService _clientService;
        private string _serverHost = "192.168.0.123";
        private int _serverPort = 7930;
        private CancellationTokenSource _connectCts = new CancellationTokenSource();
        private readonly SemaphoreSlim _connectLock = new(1, 1);
        private bool _disposed;

        // Events for UI / consumers
        public event Action<string>? MessageReceived;
        public event Action<bool>? ConnectionStatusChanged;

        public string ServerHost { get => _serverHost; set => _serverHost = value; }
        public int ServerPort { get => _serverPort; set => _serverPort = value; }
        public bool IsConnected => _clientService?.IsConnected ?? false;

        private TCPClientViewModel()
        {
            _clientService = new TcpClientService();
            _clientService.OnMessageReceived += Internal_OnMessageReceived;
            _clientService.OnConnectionChanged += Internal_OnConnectionChanged;
        }

        private void Internal_OnMessageReceived(string msg)
        {
            Log($"OnMessageReceived -> \"{msg.Replace("\n", "\\n")}\"");
            MessageReceived?.Invoke(msg);
        }

        private void Internal_OnConnectionChanged(bool connected)
        {
            Log($"OnConnectionChanged -> {connected}");
            ConnectionStatusChanged?.Invoke(connected);
        }

        private void Log(string msg)
        {
            Console.WriteLine($"[TCPVM {DateTime.Now:HH:mm:ss.fff}] {msg}");
        }

        private void LogWithStack(string msg, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            var shortFile = Path.GetFileName(file);
            var st = new StackTrace(skipFrames: 1, fNeedFileInfo: true).ToString();
            Console.WriteLine($"[TCPVM {DateTime.Now:HH:mm:ss.fff}] {msg} (called from {caller} in {shortFile}:{line})\nStack:\n{st}");
        }

        public async Task ConnectToServerAsync([CallerMemberName] string caller = "", [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            //LogWithStack("ConnectToServerAsync requested", caller, callerFile, callerLine);

            await _connectLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_clientService.IsConnected)
                {
                    Log("Connect skipped — already connected.");
                    return;
                }

                _connectCts?.Cancel();
                _connectCts = new CancellationTokenSource();

                Log($"Attempting to connect to {_serverHost}:{_serverPort}...");
                await _clientService.ConnectAsync(_serverHost, _serverPort, _connectCts.Token).ConfigureAwait(false);
                Log("ConnectAsync completed.");
            }
            catch (Exception ex)
            {
                Log($"Connect failed: {ex.GetType().Name}: {ex.Message}");
                throw;
            }
            finally
            {
                _connectLock.Release();
            }
        }

        public async Task DisconnectFromServerAsync([CallerMemberName] string caller = "", [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            //LogWithStack("DisconnectFromServerAsync requested", caller, callerFile, callerLine);

            try
            {
                _connectCts?.Cancel();
                await _clientService.DisconnectAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log($"Disconnect failed: {ex.Message}");
            }
        }

        public async Task SendDataAsync(string data, [CallerMemberName] string caller = "", [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            //LogWithStack($"SendDataAsync called -> \"{data.Replace("\n", "\\n")}\"", caller, callerFile, callerLine);

            if (!_clientService.IsConnected)
            {
                Log("Not connected before send — will attempt to connect first.");
                try
                {
                    await ConnectToServerAsync(caller, callerFile, callerLine).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log($"Connect-before-send failed: {ex.Message}");
                }
            }

            if (_clientService.IsConnected)
            {
                try
                {
                    await _clientService.SendAsync(data).ConfigureAwait(false);
                    Log($"SendAsync completed -> \"{data.Replace("\n", "\\n")}\"");
                }
                catch (Exception ex)
                {
                    Log($"Send failed: {ex.Message}");
                    throw;
                }
            }
            else
            {
                Log($"Send skipped (still not connected): \"{data.Replace("\n", "\\n")}\"");
                throw new InvalidOperationException("Not connected");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                _clientService.OnMessageReceived -= Internal_OnMessageReceived;
                _clientService.OnConnectionChanged -= Internal_OnConnectionChanged;
                _clientService.Dispose();
            }
            catch { /* swallow */ }
        }

        public async ValueTask DisposeAsync()
        {
            Dispose();
            await Task.CompletedTask;
        }
    }
}