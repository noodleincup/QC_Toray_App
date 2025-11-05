using QC_Toray_App_v3.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QC_Toray_App_v3
{
    class TCPClientViewModel
    {
        private readonly TcpClientService _clientService;
        private string _serverHost = "192.168.0.123";
        private int _serverPort = 7930;
        private CancellationTokenSource _connectCts = new CancellationTokenSource();

        // Callback to update UI after receiving message
        public event Action<string>? UpdateDefectAferReceiveMessage;



        public string ServerHost { get => _serverHost; set => _serverHost = value; }
        public int ServerPort { get => _serverPort; set => _serverPort = value; }

        // Action passed from MainWindow to safely update UI from background thread
        private readonly Action<bool> _safeConnectionUpdate;

        public bool IsConnected => _clientService.IsConnected;

        // Modified Constructor to accept the safe UI update method
        public TCPClientViewModel(Action<bool> safeConnectionUpdate)
        {
            _clientService = new TcpClientService();
            _safeConnectionUpdate = safeConnectionUpdate;

            // 1. Subscribe to events for receiving data and connection status
            _clientService.OnMessageReceived += HandleMessageReceived;
            _clientService.OnConnectionChanged += HandleConnectionChanged;
        }

        // --- Connection Handling ---
        public async Task ConnectToServerAsync()
        {
            Console.WriteLine($"Attempting to connect to {_serverHost}:{_serverPort}...");
            _connectCts = new CancellationTokenSource();

            try
            {
                await _clientService.ConnectAsync(_serverHost, _serverPort, _connectCts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                // Handle connection failure
            }
        }

        public async Task DisconnectFromServerAsync()
        {
            if (IsConnected)
            {
                Console.WriteLine("Disconnecting...");
                _connectCts.Cancel();
                await _clientService.DisconnectAsync();
            }
        }

        // --- Sending Data ---
        public async Task SendDataAsync(string data)
        {
            try
            {
                Console.WriteLine($"Sending: \"{data.Replace("\n", "\\n")}\"");
                await _clientService.SendAsync(data);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Send failed (Not connected): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send failed: {ex.Message}");
            }
        }

        // --- Event Handlers (Receiving Data & Connection Status) ---

        private void HandleMessageReceived(string message)
        {
            // Data received successfully from the server
            Console.WriteLine($"**RECEIVED**: \"{message.Replace("\n", "\\n")}\"");
            // NOTE: If you need to update a collection in the UI, do so here, 
            // using the Dispatcher via another passed-in Action if necessary.
            UpdateDefectAferReceiveMessage?.Invoke(message);


        }
        private void HandleConnectionChanged(bool isNowConnected)
        {
            // Connection status changed notification (called from the background thread)
            Console.WriteLine($"***CONNECTION STATUS CHANGED***: IsConnected = {isNowConnected}");
            // CRITICAL FIX: Use the injected Action to marshal the call back to the UI thread
            _safeConnectionUpdate?.Invoke(isNowConnected);
        }
        // --- Cleanup ---
        public void Dispose()
        {
            _connectCts.Dispose();
            _clientService.Dispose();
            _clientService.OnMessageReceived -= HandleMessageReceived;
            _clientService.OnConnectionChanged -= HandleConnectionChanged;
        }
    }
}
