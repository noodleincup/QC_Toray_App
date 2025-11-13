using HandleDatabase;
using QC_Toray_App_v3.library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace QC_Toray_App_v3
{
    /// <summary>
    /// Interação lógica para UserControlCreate.xam
    /// </summary>
    public partial class OperationUserControl : Window, INotifyPropertyChanged// System.Windows.Controls.UserControl, INotifyPropertyChanged
    {

        #region Declare Constants and Variables

        private string PIC = "Mr. Donald";
        private int batch_no = 15;
        private TCPClientViewModel viewModel = TCPClientViewModel.Instance;

        private const string CONNECT = "Connected";
        private const string DISCONNECT = "Disconnected";
        private const int DEFECT_DATA_LENGTH = 23;

        private string grabImageMessage = "grabM";
        private string resetDefectsMessage = "resetDefects987654321";

        private string _connectionStaus = DISCONNECT;
        private bool _tcpInitialized = false;
        private bool _eventsSubscribed = false;

        private UniformGrid[] UniformSample;
        private int sampleIndex = 0;

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

        #endregion

        public OperationUserControl(string lotData="111111", string batchNum="1")
        {
            InitializeComponent();

            // Initialize UniformSample array
            UniformSample = new UniformGrid[] { uniSample1, uniSample2, uniSample3, uniSample4, uniSample5, uniSummary};
            UniformSample[sampleIndex].Children.OfType<TextBlock>().ToList()[0].FontWeight = FontWeights.Bold;

            // Use the app-wide singleton
            viewModel = TCPClientViewModel.Instance;

            // Defer TCP initialization to Loaded to avoid duplicate/early initialization
            this.Loaded += OperationUserControl_Loaded;
            this.Unloaded += OperationUserControl_Unloaded;

            // Bind DataContext
            this.DataContext = this;

            // Set initial values for Lot and Batch Number
            txbLot.Text = lotData;
            txbNo.Text = batchNum;

            DateTime dateTime = DateTime.Now;
            txblDate.Text = dateTime.ToString("yyyy-MM-dd");

            List<Batch> batchs = new List<Batch>();
            batchs.Add(new Batch() { Batch_Number = 1, Judgement = 1 });
            batchs.Add(new Batch() { Batch_Number = 2, Judgement = 1 });
            batchs.Add(new Batch() { Batch_Number = 3, Judgement = 0 });
            batchs.Add(new Batch() { Batch_Number = 4, Judgement = 1 });

            //dgBatchHis.ItemsSource = batchs;

            //Display_Data();
        }


        #region Loading and Unloading UserControl
        private async void OperationUserControl_Loaded(object? sender, RoutedEventArgs e)
        {
            if (_tcpInitialized) return;
            _tcpInitialized = true;

            // Subscribe to singleton events (safe to unsubscribe first to avoid duplicate handlers)
            if (!_eventsSubscribed)
            {
                viewModel.ConnectionStatusChanged -= UpdateConnectionStatusSafely;
                viewModel.ConnectionStatusChanged += UpdateConnectionStatusSafely;

                viewModel.MessageReceived -= OnMessageReceivedFromServer;
                viewModel.MessageReceived += OnMessageReceivedFromServer;

                _eventsSubscribed = true;
            }

            try
            {
                // Request connect — viewModel logs the caller and stack trace if duplicate connects occur
                await viewModel.ConnectToServerAsync().ConfigureAwait(false);

                // send initial reset only if connected
                if (viewModel.IsConnected)
                {
                    await SendMessageToServer(resetDefectsMessage).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => Console.WriteLine($"TCP init failed: {ex.Message}"));
            }
        }

        private void OperationUserControl_Unloaded(object? sender, RoutedEventArgs e)
        {
            // Unsubscribe to avoid retaining UI references
            if (_eventsSubscribed)
            {
                viewModel.ConnectionStatusChanged -= UpdateConnectionStatusSafely;
                viewModel.MessageReceived -= OnMessageReceivedFromServer;
                _eventsSubscribed = false;
            }
        }

        private void OnMessageReceivedFromServer(string message)
        {
            // This callback comes from background thread -> marshal to UI thread
            Dispatcher.Invoke(() => GetDefectDataAndUpdateUI(message));
        }

        #endregion

        #region Keyboard Events
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                MessageBox.Show("Spacebar Pressed!");
                e.Handled = true; // Prevents further propagation of the event
            }
        }

        // Keyboard Click
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                // Ensure sending uses awaited path and single-connection behavior
                _ = Dispatcher.InvokeAsync(async () => await SendMessageToServer(grabImageMessage).ConfigureAwait(false));

                e.Handled = true; // Stops further event bubbling
            }
            else if (e.Key == Key.Enter) {
                //MessageBox.Show("Enter Pressed in PreviewKeyDown!");

                // Get data and OK Trick
                //Batch batch = PrepareData(true);

                // Insert data
                bool success = true; // Insert_Data(batch);
                                     //if (success) { MessageBox.Show("Insert Success"); } else { MessageBox.Show("Insert Success"); }
                                     
                if (viewModel.IsConnected)
                {
                    _ = Dispatcher.InvokeAsync(async() => await SendMessageToServer(resetDefectsMessage).ConfigureAwait(false));
                }
                UpdateSelectUI(sampleIndex);
                UpdateSampleIndex();

                if (sampleIndex == 5)
                {
                    SummaryDefecAllType();
                }


                e.Handled = true; // Stops further event bubbling

                
            }
        }

        private void UpdateSelectUI(int index)
        {
            Dispatcher.Invoke(() =>
            {
                int nextIndex = (index + 1) % UniformSample.Length;
                //Brush defaultColor = UniformSample[nextIndex].Background;

                //UniformSample[index].Background = defaultColor;
                //UniformSample[nextIndex].Background = Brushes.LightBlue;

                FontWeight fontWeight = UniformSample[nextIndex].Children.OfType<TextBlock>().ToList()[0].FontWeight;
                UniformSample[index].Children.OfType<TextBlock>().ToList()[0].FontWeight = fontWeight;
                UniformSample[nextIndex].Children.OfType<TextBlock>().ToList()[0].FontWeight = FontWeights.Bold;

            });
        }
        private void UpdateSampleIndex()
        {
            sampleIndex = (sampleIndex + 1) % UniformSample.Length;
        }

        private void SummaryDefecAllType()
        {
            var allTxbSummary = UniformSample[UniformSample.Length-1].Children.OfType<TextBox>().ToList();
            UIElement eachTxbSummary;

            for (int i = 0; i < DEFECT_DATA_LENGTH; i++)
            {
                eachTxbSummary = allTxbSummary[i];
                int sumValue = 0;
                for (int j = 0; j < UniformSample.Length -1; j++)
                {
                    var sampleAllTextBoxes = UniformSample[j].Children.OfType<TextBox>().ToList();
                    UIElement sampleTxbElement = sampleAllTextBoxes[i];
                    int currentValue = 0;
                    if (!int.TryParse(((TextBox)sampleTxbElement).Text, out currentValue))
                    {
                        continue;
                    }
                     sumValue += currentValue;
                }

                ((TextBox)eachTxbSummary).Text = sumValue.ToString();
            }
        }

        #endregion



        #region TCP Methods

        private async Task ConnectTcpServer()
        {
            await viewModel.ConnectToServerAsync().ConfigureAwait(false);
        }

        // set as private for event from MainWindow
        public async Task DisconnectTcpServer()
        {
            if (viewModel != null && viewModel.IsConnected == true)
            {
                await viewModel.DisconnectFromServerAsync().ConfigureAwait(false);
                UpdateConnectionStatusSafely(false);
            }
        }

        private void UpdateConnectionStatusSafely(bool isConnected)
        {
            // Use the Dispatcher to ensure the UI update runs on the main thread
            Dispatcher.Invoke(() =>
            {
                ConnectionStatus = isConnected ? CONNECT : DISCONNECT;
            });
        }

        private async Task SendMessageToServer(string message)
        {
            if (viewModel == null) return;

            // Ensure connected before sending. If not connected, try to connect once.
            if (!viewModel.IsConnected)
            {
                await ConnectTcpServer().ConfigureAwait(false);
            }

            if (viewModel.IsConnected)
            {
                await viewModel.SendDataAsync(message).ConfigureAwait(false);
            }
            else
            {
                // optional: log or notify that send failed due to no connection
                Dispatcher.Invoke(() => Console.WriteLine($"Send skipped, not connected: \"{message}\""));
            }
        }

        #endregion

        #region Button Click Handlers

        private async void btnReconnect_Click(object sender, RoutedEventArgs e)
        {
            await ConnectTcpServer().ConfigureAwait(false);
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Run Clicked");
            await SendMessageToServer(grabImageMessage).ConfigureAwait(false);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SummaryDefecAllType();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //// Get data and OK Trick
            //Batch batch = PrepareData(true);
            //// Insert data
            //bool success = Insert_Data(batch);
            //if (success) { MessageBox.Show("Insert Success"); } else { MessageBox.Show("Insert Failed"); }
            this.DialogResult = true;
            this.Close();
        }

        private void btnNG_Click(object sender, RoutedEventArgs e)
        {
            //// Get data and NG Trick
            //Batch batch = PrepareData(false);
            //// Insert data
            //bool success = Insert_Data(batch);
            //if (success) { MessageBox.Show("Insert Success"); } else { MessageBox.Show("Insert Failed"); }
            this.DialogResult = false;
            this.Close();
        }

        #endregion


        #region Data Handling Methods
        // Get the data from SQL to datagrid on interface
        private void Display_Data()
        {
            // Get Connection String for connecting database
            string conn = DatabaseConfig.ConnectionString;

            // Define Users Database Object
            ItemsHandle itemsDb = new ItemsHandle(conn);

            // Get Item Data
            DataTable items = itemsDb.GetItems();

            // Prepare List for displaying
            List<Batch> lists = new List<Batch>();

            // Insert data to list
            for(int i = 0; i < items.Rows.Count; i++)
            {
                Batch batch = new Batch();
                batch.GetFromDataTable(items.Rows[i]);
                lists.Add(batch);
            }

            // Add data to DataGrid
            //dgBatchHis.ItemsSource = lists;

        }

        // Insert from data table to sql server 
        private bool Insert_Data(Batch batch)
        {
            // Get Connection String for connecting database
            string conn = DatabaseConfig.ConnectionString;

            // Define Users Database Object
            ItemsHandle itemsDb = new ItemsHandle(conn);

            // Prepare data table for inserting
            DataTable dt = batch.ConvertBatchToDataTable();

            try
            {
                // Insert data
                bool result = itemsDb.InsertItems(dt);
                return result;
            }
            catch (Exception ex) { 
                MessageBox.Show(ex.ToString());
                return false;
            }
        }


        // Insert data on text box to batch object
        private Batch PrepareData(bool judge)
        {
            Batch insertBatch = new Batch();
            insertBatch.Lot = txbLot.Text;
            insertBatch.Batch_Number = batch_no;
            insertBatch.Pallet_Size = tgPalletSize.IsChecked == true ? 1 : 0;
            insertBatch.MissDB = Convert.ToInt32(txbMissDb.Text);
            insertBatch.MissTP = Convert.ToInt32(txbMissTp.Text);
            insertBatch.MissFf = Convert.ToInt32(txbMissFf.Text);
            insertBatch.LinkDB = Convert.ToInt32(txbLinkDb.Text);
            insertBatch.LinkTP = Convert.ToInt32(txbLinkTp.Text);
            insertBatch.LinkFf = Convert.ToInt32(txbLinkFf.Text);
            insertBatch.LinkFF = Convert.ToInt32(txbLinkFF.Text);
            insertBatch.Defect = Convert.ToInt32(txbDefect.Text);
            insertBatch.GF1 = Convert.ToInt32(txbGF1.Text);
            insertBatch.GF2 = Convert.ToInt32(txbGF2.Text);
            insertBatch.GF3 = Convert.ToInt32(txbGF3.Text);
            insertBatch.Meya = Convert.ToInt32(txbMeya.Text);
            insertBatch.Meya_NoChg = Convert.ToInt32(txbMeya_NoChg.Text);
            insertBatch.ForeignMat = Convert.ToInt32(txbForeignMat.Text);
            insertBatch.BlackSpot_SS = Convert.ToInt32(txbBlackSpot_SS.Text);
            insertBatch.BlackSpot_S = Convert.ToInt32(txbBlackSpot_S.Text);
            insertBatch.BlackSpot_M = Convert.ToInt32(txbBlackSpot_M.Text);
            insertBatch.BlackSpot_L = Convert.ToInt32(txbBlackSpot_L.Text);
            insertBatch.ColorAbnomal = Convert.ToInt32(txbColorAbnormal.Text);
            insertBatch.Macaroni_SS = Convert.ToInt32(txbMacaroni_SS.Text);
            insertBatch.Macaroni_S = Convert.ToInt32(txbMacaroni_S.Text);
            insertBatch.Macaroni_M = Convert.ToInt32(txbMacaroni_M.Text);
            insertBatch.Macaroni_L = Convert.ToInt32(txbMacaroni_L.Text);
            insertBatch.Remark = txbRemark.Text;
            insertBatch.Judgement = judge ? 1:0;
            insertBatch.PIC = PIC;
            
            return insertBatch;
        }
        #endregion

        #region Defect Input Handlers

        private void GetDefectDataAndUpdateUI(string message)
        {
            string[] defectData = message.Split(',');

            if(defectData.Length == DEFECT_DATA_LENGTH)
            {

                var allTextBoxes = UniformSample[sampleIndex].Children.OfType<TextBox>().ToList();
                UIElement txbElement;

                for (int i = 0; i < DEFECT_DATA_LENGTH; i++)
                {
                    //txbElement = allTextBoxes.Find(tb => tb.Name == $"txb{defectData[i].Split(':')[0]}");
                    txbElement = allTextBoxes[i];
                    ((TextBox)txbElement).Text = defectData[i].Split(':')[1];
                    //txbElement.Dispatcher.Invoke(() =>
                    //{
                    //    if (txbElement is TextBox)
                    //    {
                    //        ((TextBox)txbElement).Text = defectData[i].Split(':')[1];
                    //    }
                    //});
                }
            }

            Console.WriteLine("Defect Data Updated from Server.");

        }

        #endregion
    }

    // Bliding Data to Interface
    public class JudgementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int judgement)
            {
                return judgement == 1 ? "OK" : "NG";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string judgementStr)
            {
                return judgementStr == "OK" ? 1 : 0;
            }
            return 0;
        }
    }
}
