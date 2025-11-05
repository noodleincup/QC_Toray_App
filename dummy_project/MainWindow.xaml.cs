using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;
using dummy_project.Network; // Assuming this namespace based on your file

namespace dummy_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ClientViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            // Set DataContext for data binding
            this.DataContext = this;

            // Initialize ViewModel and pass in the UI update action
            viewModel = new ClientViewModel(UpdateConnectionStatusSafely);

            OpenConsoleWindows();


        }

        private string _connectionStaus = "Disconnected";

        public string ConnectionStatus
        {
            get { return _connectionStaus; }
            set
            {
                if (_connectionStaus != value)
                {
                    _connectionStaus = value;
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // --- NEW METHOD FOR SAFE UI UPDATE ---
        // This method ensures any update from a background thread is run on the UI thread.
        private void UpdateConnectionStatusSafely(bool isConnected)
        {
            // Use the Dispatcher to ensure the UI update runs on the main thread
            Dispatcher.Invoke(() =>
            {
                ConnectionStatus = isConnected ?
                    "Connected" ://$"Connected to {ClientViewModel.ServerHost}:{ClientViewModel.ServerPort}" : 
                    "Disconnected";
            });
        }

        #region Button Click Handlers
        private async void btnSendData_Click(object sender, RoutedEventArgs e) // Changed to async void for event handler
        {
            Console.WriteLine($"[LOG] Send Data clicked at {DateTime.Now}");

            if (viewModel.IsConnected)
            {
                // **CORRECTION:** Only send data here. Do NOT disconnect immediately.
                // The text box 'txtDataToSend' is assumed to exist in XAML.
                await viewModel.SendDataAsync(txtDataToSend.Text);
            }
            else
            {
                Console.WriteLine("ERROR: Cannot send data. Not connected.");
                //MessageBox.Show("Please connect to the server first.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e) // Changed to async void for event handler
        {
            if (!viewModel.IsConnected)
            {
                viewModel.ServerHost = (txbIp.Text != "") ? txbIp.Text: viewModel.ServerHost; // Assuming txbPort is a TextBox for host
                viewModel.ServerPort = (txbIp.Text != "") ? int.Parse(txbPort.Text) : viewModel.ServerPort; // Assuming txbHost is a TextBox for port

                // 1. Connect
                await viewModel.ConnectToServerAsync();
            }
            else
            {
                // If connected, this button can act as a Disconnect button
                await viewModel.DisconnectFromServerAsync();
            }
        }
        #endregion

        // Open Console Window
        private void OpenConsoleWindows() 
        {
            // **Call AllocConsole to create the console window**
            if (ConsoleHelper.AllocConsole())
            {
                Console.Title = "Application Debug Log";
                Console.WriteLine("Console window successfully attached.");
            }
            else
            {
                // The console might already exist or a failure occurred
            }
        }

    }

    // --- ViewModel Definition (modified constructor) ---
    public class ClientViewModel : IDisposable
    {
        private readonly TcpClientService _clientService;
        private string _serverHost = "192.168.0.123";
        private int _serverPort = 23;
        private CancellationTokenSource _connectCts = new CancellationTokenSource();

        public string ServerHost { get => _serverHost; set => _serverHost = value; }
        public int ServerPort { get => _serverPort; set => _serverPort = value; }

        // Action passed from MainWindow to safely update UI from background thread
        private readonly Action<bool> _safeConnectionUpdate;

        public bool IsConnected => _clientService.IsConnected;

        // Modified Constructor to accept the safe UI update method
        public ClientViewModel(Action<bool> safeConnectionUpdate)
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
