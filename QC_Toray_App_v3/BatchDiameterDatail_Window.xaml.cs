using QC_Toray_App_v3.UserControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QC_Toray_App_v3
{
    /// <summary>
    /// Interaction logic for BatchDiameterDatail_Window.xaml
    /// </summary>
    /// 


    public partial class BatchDiameterDatail_Window : Window, INotifyPropertyChanged
    {
        #region Declare Constants and Variables
        public string ResultString { get; private set; }
        private const int BATCH_ROW_COUNT = 30;

        // Private fields to hold the initial data
        private readonly string _initialGrade;
        private readonly string _initialLot;
        private readonly string _initialBtachNo;

        private TCPClientViewModel viewModel = TCPClientViewModel.Instance;

        private const string CONNECT = "Connect";
        private const string DISCONNECT = "Disconnect";

        private string measureDiameterLMessage = "measureL";
        private string measureDiameterABMessage = "measureAB";
        private string resetDefectsMessage = "resetDefects987654321";

        private string _connectionStaus = DISCONNECT;
        private bool _tcpInitialized = false;
        private bool _eventsSubscribed = false;

        private int ELEMENT_NUM = 30;

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

        // Parameterized constructor to receive the three strings
        public BatchDiameterDatail_Window(string grade, string lot, string batchNo)
        {
            InitializeComponent();

            viewModel = TCPClientViewModel.Instance;

            // Defer TCP initialization to Loaded to avoid duplicate/early initialization
            this.Loaded += BatchDiameterDetail_Winodws_Loaded;
            this.Unloaded += BatchDiameterDetail_Winodws_Unloaded;

            // Store the data
            _initialGrade = grade;
            _initialLot = lot;
            _initialBtachNo = batchNo;

            // Display the data in the TextBlocks
            txbGrade.Text = _initialGrade;
            txbLot.Text = _initialLot;
            txbBatchNumber.Text = _initialBtachNo;

            InitializeBatchDiameterTable();
        }


        #region Loading and Unloading UserControl
        private async void BatchDiameterDetail_Winodws_Loaded(object? sender, RoutedEventArgs e)
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

        private void BatchDiameterDetail_Winodws_Unloaded(object? sender, RoutedEventArgs e)
        {
            // Unsubscribe to avoid retaining UI references
            if (_eventsSubscribed)
            {
                viewModel.ConnectionStatusChanged -= UpdateConnectionStatusSafely;
                viewModel.MessageReceived -= OnMessageReceivedFromServer;
                _eventsSubscribed = false;
            }
        }

        

        #endregion

        #region UI handling methods
        public void InitializeBatchDiameterTable()
        {


            // *** NEW: Use the Grid and create RowDefinitions ***
            // 1. Clear any existing content and row definitions
            grdDiameterTable.Children.Clear();
            grdDiameterTable.RowDefinitions.Clear();

            // 2. Define the row count: 1 (Header) + BATCH_ROW_COUNT (30) + 1 (Average) + 1 (Judgement) = 33 rows (based on your code logic)
            // Header (1) + Loop (30) + Average (1) + Judgement (1) = 33 rows total.
            int totalRowCount = BATCH_ROW_COUNT + 3; // Assuming BATCH_ROW_COUNT is 30.

            // 3. Create a RowDefinition for each row and set the Height to '*' for proportional distribution.
            for (int i = 0; i < totalRowCount; i++)
            {
                // '*' is proportional sizing. It tells the Grid to give this row an equal share of the available vertical space.
                grdDiameterTable.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            int rowIndex = 0;

            // 4) Define and place the header row (Row 0)
            BatchCell_UserControl header = new BatchCell_UserControl();
            header.cols0.Text = "";
            header.cols1.Text = "A";
            header.cols2.Text = "B";
            header.cols3.Text = "L";

            header.cols1.IsReadOnly = true;
            header.cols2.IsReadOnly = true;
            header.cols3.IsReadOnly = true;

            header.cols1.FontSize = 10;
            header.cols2.FontSize = 10;
            header.cols3.FontSize = 10;
            Grid.SetRow(header, rowIndex++);
            grdDiameterTable.Children.Add(header);

            // 5) Create 30 batch rows (Row 1 to Row 30)
            for (int i = 1; i <= BATCH_ROW_COUNT; i++)
            {
                BatchCell_UserControl batchRow = new BatchCell_UserControl();
                batchRow.cols0.Text = i.ToString(); // Set the row number
                Grid.SetRow(batchRow, rowIndex++);
                grdDiameterTable.Children.Add(batchRow);
            }

            // 6) Add the average row (Row 31)
            BatchCell_UserControl averageRow = new BatchCell_UserControl();
            averageRow.cols0.Text = "Average";
            Grid.SetRow(averageRow, rowIndex++);
            grdDiameterTable.Children.Add(averageRow);

            // 7) Add the judgement row (Row 32)
            BatchCell_UserControl judgementRow = new BatchCell_UserControl();
            judgementRow.cols0.Text = "Judgement";
            Grid.SetRow(judgementRow, rowIndex++);
            grdDiameterTable.Children.Add(judgementRow);

            //// 8) Add buttons at the bottom
            //StackPanel buttonPanel = new StackPanel
            //{
            //    Orientation = Orientation.Horizontal,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    Margin = new Thickness(0, 0, 0, 0)
            //};
            //Button btnOK = new Button
            //{
            //    Content = "OK",
            //    Width = 80,
            //    Margin = new Thickness(10, 0, 10, 0)
            //};
            //btnOK.Click += btnOK_Clicked;
            //Button btnCancel = new Button
            //{
            //    Content = "Cancel",
            //    Width = 80,
            //    Margin = new Thickness(10, 0, 10, 0)
            //};
            //btnCancel.Click += btnCancel_Clicked;


            //buttonPanel.Children.Add(btnOK);
            //buttonPanel.Children.Add(btnCancel);

            //Grid.SetRow(buttonPanel, rowIndex++);
            //grdDiameterTable.Children.Add(buttonPanel);
        }

        private void ChangeBtnTcpStatusUI(bool isConnected)
        {
            Brush blueColor = btnSave.Background;
            Brush redColor = (Brush)new BrushConverter().ConvertFromString("#FFC864C8");

            if (isConnected)
            {
                btnTcpStatus.Content = DISCONNECT;
                btnTcpStatus.Background = redColor;
                btnTcpStatus.BorderBrush = redColor;
            }
            else
            {
                btnTcpStatus.Content = CONNECT;
                btnTcpStatus.Background = blueColor;
                btnTcpStatus.BorderBrush = blueColor;
            }
        }

        private void UpdateBatchDiameterTable(string message)
        {
            UIElement[] elements = grdDiameterTable.Children.Cast<BatchCell_UserControl>().ToArray();

            string[] rawData = GetRawData(message);

            string typeData = GetTypeData(message);

            if (!IsTypeValid(typeData))
            {
                throw new Exception($"Invalid type data received: {typeData}");
            }

            if (typeData == "L") { LoadDataLToTable(rawData, elements); }
            else { LoadDataABToTable(rawData, elements); }

            UpdateAverageUI(elements);
        }

        private string[] GetRawData(string message) 
        { 
            return message.Split(',');
        }

        private string GetTypeData(string message)
        {
            string[] rawData = GetRawData(message);
            char[] charToTrim = { ' ', '"'};

            // type:AB or type:L from "0:AB" , "0:L" 
            return rawData[0].Split(":")[1].Trim(charToTrim);
        }

        private bool IsTypeValid(string typeData)
        {
            return Array.Exists(new string[] { "L", "AB" }, data => data == typeData);
        }

        private string ModifyDigitPoint(string value, int digit)
        {
            int digitIndex = value.IndexOf('.');
            int desiredLength = digitIndex + digit + 1;

            if (digitIndex != -1) 
            {
                return value.Substring(0, desiredLength);
            }

            return value;
        }

        private void LoadDataLToTable(string[] rawData, UIElement[] elements)
        {
            // - 3 refer from header, average and judgement , -1 refer for type data as index 0
            int elementNum = ELEMENT_NUM;
            int inputNum = rawData.Length - 1;

            if (elementNum != inputNum)
            {
                throw new Exception($"Amount L data {elementNum} not equal to number of input {inputNum}");
            }

            for (int i = 1; i < ELEMENT_NUM + 1; i++)
            {
                if (elements[i] is BatchCell_UserControl batchCell)
                {
                    //string id = batchCell.cols0.Text;

                    string[] elementData = rawData[i].Split(":");

                    int id = int.Parse(elementData[0]);
                    string dataL = elementData[1].Trim();

                    if (dataL != null && id == i) 
                    { 
                        batchCell.cols3.Text = ModifyDigitPoint(dataL, 2);
                    }
                    else
                    {
                        throw new Exception("Invalid Index or L Data");
                    }
                }
            }
        }

        private void LoadDataABToTable(string[] rawData, UIElement[] elements)
        {
            // - 3 refer from header, average and judgement , -1 refer for type data as index 0
            int elementNum = ELEMENT_NUM;
            int inputNum = rawData.Length - 1;

            if (elementNum != inputNum)
            {
                throw new Exception($"Amount AB data {elementNum} not equal to number of input {inputNum}");
            }

            for (int i = 1; i < ELEMENT_NUM + 1; i++)
            {
                if (elements[i] is BatchCell_UserControl batchCell)
                {
                    //string id = batchCell.cols0.Text;

                    string[] elementData = rawData[i].Split(":");

                    int id = int.Parse(elementData[0]);
                    string dataA = elementData[1].Trim();
                    string dataB = elementData[2].Trim();

                    if (dataA != null && dataB != null && id == i)
                    {
                        batchCell.cols1.Text = ModifyDigitPoint(dataA, 2);
                        batchCell.cols2.Text = ModifyDigitPoint(dataB, 2);
                    }
                    else
                    {
                        throw new Exception("Invalid Index or AB Data");
                    }
                }
            }
        }

        private void UpdateAverageUI(UIElement[] elements)
        {
            UIElement avgUI = elements[elements.Length-2];
            int eleNum = elements.Length - 3;

            

            float[] avgAll = {0, 0, 0};
            for (int i = 1; i < elements.Length - 2; i++) 
            {
                if (elements[i] is BatchCell_UserControl batchCell)
                {

                    float dataA, dataB, dataL;

                    if (float.TryParse(batchCell.cols1.Text, out dataA)) { avgAll[0] += dataA; }
                    if (float.TryParse(batchCell.cols2.Text, out dataB)) { avgAll[1] += dataB; }
                    if (float.TryParse(batchCell.cols3.Text, out dataL)) { avgAll[2] += dataL; }
                }
            }

            avgAll = avgAll.Select(x => (float)(x/eleNum)).ToArray();

            if (avgUI is not BatchCell_UserControl avgBatchRow)
            {
                throw new Exception("Invalid Average row UI");
            }
            else
            {
                avgBatchRow.cols1.Text = avgAll[0].ToString("F2");
                avgBatchRow.cols2.Text = avgAll[1].ToString("F2");
                avgBatchRow.cols3.Text = avgAll[2].ToString("F2");

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

                ChangeBtnTcpStatusUI(isConnected);

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

        private void OnMessageReceivedFromServer(string message)
        {
            // This callback comes from background thread -> marshal to UI thread
            //Dispatcher.Invoke(() => GetDefectDataAndUpdateUI(message));
            //Dispatcher.Invoke(() => UpdateBatchDiameterTable(message));
            if (Dispatcher.CheckAccess())
            {
                UpdateBatchDiameterTable(message); // already on UI thread
            }
            else
            {
                // Post the UI update to the UI thread without blocking the receive thread
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        UpdateBatchDiameterTable(message);
                    }
                    catch (Exception ex)
                    {
                        // log — prevents exceptions in UI code from bubbling back into the receive thread
                        Console.WriteLine($"UpdateBatchDiameterTable failed: {ex.Message}");
                    }
                }));
            }
        }
        #endregion

        #region Button Click Handlers
        private void btnSaveDiameterData_Clicked(object sender, RoutedEventArgs e)
        {
            // Collect data from the table and form the ResultString
            StringBuilder resultBuilder = new StringBuilder();
            foreach (UIElement element in grdDiameterTable.Children)
            {
                if (element is BatchCell_UserControl batchCell)
                {
                    // Skip header and buttons
                    if (batchCell.cols0.Text == "" || batchCell.cols0.Text == "Average" || batchCell.cols0.Text == "Judgement")
                        continue;
                    resultBuilder.AppendLine($"{batchCell.cols0.Text},{batchCell.cols1.Text},{batchCell.cols2.Text},{batchCell.cols3.Text}");
                }
            }
            ResultString = resultBuilder.ToString();
            this.DialogResult = true; // Indicate success
            this.Close();
        }

        private async void btnTcpStatus_Clicked(object sender, RoutedEventArgs e)
        {
            if ( ConnectionStatus == CONNECT)
            {
                await DisconnectTcpServer().ConfigureAwait(false);
            }
            else
            {
                await ConnectTcpServer().ConfigureAwait(false);
            }
        }

        private async void btnMeasureL_Clicked(object sender, RoutedEventArgs e)
        {
            // Send grab image command to server
            await SendMessageToServer(measureDiameterLMessage).ConfigureAwait(false);
        }

        private async void btnMeasureAB_Clicked(object sender, RoutedEventArgs e)
        {
            // Send grab image command to server
            await SendMessageToServer(measureDiameterABMessage).ConfigureAwait(false);
        }
        #endregion
    }
}
