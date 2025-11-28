using dummy_project.Network; // Assuming this namespace based on your file
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HandleDatabase;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;

namespace dummy_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ClientViewModel viewModel;
        private LotOverviewViewModel lotOverviewView;
        private DatabaseHandler databaseHandler;

        public MainWindow()
        {
            InitializeComponent();

            // Set DataContext for data binding
            this.DataContext = this;

            // Initialize ViewModel and pass in the UI update action
            viewModel = new ClientViewModel(UpdateConnectionStatusSafely);

            OpenConsoleWindows();

            lotOverviewView = LotOverviewViewModel.Instance;

            this.DataContext = lotOverviewView;

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) 
        {
            using (var cts = new CancellationTokenSource(2000))
            {
                await lotOverviewView.LoadSampleGroupAsync(cts.Token);
            }
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

    public class LotOverviewViewModel : INotifyPropertyChanged
    {
        // Private static field to hold the single instance
        private static readonly Lazy<LotOverviewViewModel> lazyInstance = new Lazy<LotOverviewViewModel>(() => new LotOverviewViewModel());
        private DatabaseHandler databaseHandler = new DatabaseHandler("Server=192.168.0.123\\SQLEXPRESS; Database=camera_inspection; User Id=sa; Password=1234;TrustServerCertificate=True;");
        private const string TABLE = "tb_master_SamepleGroupName";
        private const string CONNECTION_STRING = "Server=192.168.0.123\\SQLEXPRESS; Database=camera_inspection; User Id=sa; Password=1234;TrustServerCertificate=True;";

        private LotOverviewViewModel()
        {
            // Initialize ObservableCollections here or in a load method
            SampleGroups = new ObservableCollection<SampleGroup> { 
                new SampleGroup {ID = 1, SampleName = "Apple" },
                new SampleGroup {ID = 2, SampleName = "Banana" }
            };
            Patterns = new ObservableCollection<Pattern>();


            
        }

        public void Test_Display_TABLE() 
        {
            databaseHandler = new DatabaseHandler(CONNECTION_STRING);
            DataTable dt = databaseHandler.GetTableDatabaseAsDataTable(TABLE);

            Console.WriteLine($"Amount rows: {dt.Rows.Count}");
        }

        // Public static property to provide global access to the instance
        public static LotOverviewViewModel Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }

        // Data for the first ComboBox
        public ObservableCollection<SampleGroup> SampleGroups { get; set; }
        // SelectedGroup property must fire PropertyChanged
        private SampleGroup _selectedGroup;
        public SampleGroup SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup != value)
                {
                    _selectedGroup = value;
                    OnPropertyChanged();
                    // Add logic here to load Patterns based on selected group if needed
                }
            }
        }

        // --- Data for the second ComboBox ---
        public ObservableCollection<Pattern> Patterns { get; set; }

        // SelectedPattern property must fire PropertyChanged
        private Pattern _selectedPattern;
        public Pattern SelectedPattern
        {
            get => _selectedPattern;
            set
            {
                if (_selectedPattern != value)
                {
                    _selectedPattern = value;
                    OnPropertyChanged();
                }
            }
        }


        public ICommand LoadGroupsCommand { get; }


        public async Task LoadSampleGroupAsync(CancellationToken cancellationToken)
        {

            // 1. **Immediate Action:** Clear the ObservableCollection
            // This is a safe and fast operation.
            SampleGroups.Clear();

            // 2. **Check for Cancellation Before Starting**
            // If the timeout already occurred (e.g., in the calling code) before 
            // LoadSampleGroup started, we exit immediately.
            cancellationToken.ThrowIfCancellationRequested();

            DataTable sampleGroupDt = GetSampleGroupsFromDatabase2();

            Console.WriteLine($"Amount rows: {sampleGroupDt.Rows.Count}");

            foreach (DataRow row in sampleGroupDt.Rows)
            {
                SampleGroup sampleGroup = new SampleGroup()
                {
                    ID = Convert.ToInt32(row["id"]),
                    SampleName = row["sampleName"]?.ToString() ?? string.Empty,
                    UpdatedBy = row["updateBy"]?.ToString() ?? string.Empty,
                    UpdateDate = row["updateDate"] != DBNull.Value ? Convert.ToDateTime(row["updateDate"]) : DateTime.MinValue
                };

                SampleGroups.Add(sampleGroup);
            }

        }

        private async Task<DataTable> GetSampleGroupsFromDatabase(CancellationToken cancellationToken)
        {
            // Fast-fail if already cancelled
            cancellationToken.ThrowIfCancellationRequested();

            // databaseHandler.GetTableDatabaseAsDataTable(...) is synchronous.
            // Run it on the thread-pool so it doesn't block the caller; pass the token to Task.Run
            // NOTE: passing the token to Task.Run only cancels starting/registration and will not
            // abort an in-progress synchronous ADO.NET call. Prefer async DB APIs if possible.
            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return databaseHandler.GetTableDatabaseAsDataTable("tb_master_SamepleGroupName");
            }, cancellationToken).ConfigureAwait(false);
        }

        private DataTable GetSampleGroupsFromDatabase2()
        {
            return databaseHandler.GetTableDatabaseAsDataTable("tb_master_SamepleGroupName");
        }


        // Boilerplate INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SampleGroup
    {
        public int ID { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdatedBy { get; set; }

        public string SampleName { get; set; }
    }

    public class Pattern
    {
        public int PatternID { get; set; }
        public string PatternName { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Description { get; set; }
    }
}
