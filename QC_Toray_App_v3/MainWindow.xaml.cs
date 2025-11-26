using ControlzEx.Standard;
using QC_Toray_App_v3.UserControl;
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

namespace QC_Toray_App_v3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;
        public static MainWindow Instance => _instance ??= new MainWindow();


        // For app window dragging
        private bool _isDragging = false;
        private Point _startPoint;
        private bool loginStatus = false;


        // Properties to hold Lot, Grade and Batch data
        private string lotData = "";
        public string LotData
        {
            get { return lotData; }
        }

        private string gradeData = "";
        public string GradeData
        {
            get { return gradeData; }
        }

        private string orderNo = "";
        public string OrderNo
        {
            get { return orderNo; }
            set { orderNo = value; }
        }

        private string batchNum = "";
        public string BatchNum
        {
            get { return batchNum; }
            set { 
                batchNum = value; 
                //MessageBox.Show($"Batch Number set to: {batchNum}", "Batch Number Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            OpenConsoleWindows();

            // Singleton Pattern Implementation
            if (_instance == null)
            {
                _instance = this;
            }


            this.DataContext = this;

            // Attach event listener when LoginUserControl is added
            var loginControl = new LoginUserControl();
            loginControl.ChangePageRequested += OnChangePageRequested;
            loginControl.ChangeUserName += OnChangeUserName;
            GridMain.Children.Add(loginControl);
        }
        #region Menu Buttons functions
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void LogoutButton_Clicked(object sender, RoutedEventArgs e)
        {
            GlobalState.Instance.IsFeatureEnabled = false;
            LoginUserControl usc = new LoginUserControl();
            usc.ChangePageRequested += OnChangePageRequested;
            usc.ChangeUserName += OnChangeUserName;
            GridMain.Children.Add(usc);
        }
        #endregion
        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.UserControl usc = null;

            // Close TCP connection if Operating page is open
            if (IsOperatingPageOpen())
            {
                ClosedTcpConnectionIfOperatingPage();
            }

            GridMain.Children.Clear();

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "Login":
                    LoginUserControl loginUserControl = new LoginUserControl();
                    loginUserControl.ChangePageRequested += OnChangePageRequested;
                    loginUserControl.ChangeUserName += OnChangeUserName;
                    GridMain.Children.Add(loginUserControl);

                    break;
                case "MainTable":
                    MainTable_UserControl mainTable = new MainTable_UserControl();
                    mainTable.ChangePageRequested += OnChangePageRequested;
                    mainTable.UpdateLotAndGradeData += OnUpdateLotAndGradeData; // OnUpdateLotAndGradeData
                    GridMain.Children.Add(mainTable);
                    break;
                case "LotOverview":
                    LotOverviewUserControl lotOverview = new LotOverviewUserControl(orderNo, lotData, gradeData);
                    lotOverview.ChangePageRequested += (s, e) => OnChangePageRequested(s, e);
                    GridMain.Children.Add(lotOverview);
                    break;
                case "SetManagePattern":
                    usc = new ManagePattern_UserControl();
                    GridMain.Children.Add(usc);
                    break;
                case "SetPattern":
                    usc = new SetPattern_UserControl();
                    GridMain.Children.Add(usc);
                    break;
                case "SetSampleType":
                    usc = new SetSampleType_UserControl();
                    GridMain.Children.Add(usc);
                    break;
                case "Operating":
                    //OperationUserControl operationUser = new OperationUserControl(
                    //    lotData: LotData,
                    //    batchNum: BatchNum);
                    ////MessageBox.Show($"Lot Data: {LotData}\nBatch Number: {BatchNum}", "Operating Page Data", MessageBoxButton.OK, MessageBoxImage.Information);
                    //GridMain.Children.Add(operationUser);
                    break;
                case "Report1":
                    usc = new Report1_UserControl();
                    GridMain.Children.Add(usc);
                    break;
                case "Report2":
                    usc = new Report2_UserControl();
                    GridMain.Children.Add(usc);
                    break;
                case "Master":
                    usc = new Master_UserControl();
                    GridMain.Children.Add(usc);
                    break;
                default:
                    break;
            }
        }

        private void OnChangePageRequested(object sender, string pageName)
        {
            // Simulate selecting a ListViewItem to trigger the switch
            var listViewItem = ListViewMenu.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Name == pageName);
            if (listViewItem != null)
            {
                ListViewMenu.SelectedItem = listViewItem;
                ListViewMenu_SelectionChanged(ListViewMenu, null); // Call the existing function
            }
        }

        private void ClosedTcpConnectionIfOperatingPage()
        {
            // Close TCP connection here
            var operatingControl = GridMain.Children.OfType<OperationUserControl>().FirstOrDefault();

            if (operatingControl.ConnectionStatus == "Connected")
            {
                operatingControl.DisconnectTcpServer();
            }

        }

        private bool IsOperatingPageOpen()
        {
            UIElementCollection children = GridMain.Children.Count > 0 ? GridMain.Children : null;

            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child is OperationUserControl)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Overloaded function to change page with lot and grade data to operation user control
        private void OnChangePageRequested(object sender, (string pageName, string lotData, string batchNum) args)
        {
            string pageName = args.pageName;
            string lotData = args.lotData;
            string batchNum = args.batchNum;
            // Simulate selecting a ListViewItem to trigger the switch
            var listViewItem = ListViewMenu.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Name == pageName);
            if (listViewItem != null)
            {
                ListViewMenu.SelectedItem = listViewItem;
                if (pageName == "Operating")
                {
                    //OperationUserControl operationControl = new OperationUserControl(lotData, batchNum);
                    //GridMain.Children.Clear();
                    //GridMain.Children.Add(operationControl);
                }
                else
                {
                    ListViewMenu_SelectionChanged(ListViewMenu, null); // Call the existing function
                }
            }
        }

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


        private void OnUpdateLotAndGradeData(object sender, string lot_grade_orderNo)
        {
            string[] parts = lot_grade_orderNo.Split(',');
            lotData = parts[0];
            gradeData = parts[1];
            orderNo = parts[2];

            //Console.WriteLine($"Lot Data updated to: {lotData}");
            //Console.WriteLine($"Grade Data updated to: {gradeData}");

        }

        private void OnChangeUserName(object sender, string userName)
        {
            txtUsername.Text = userName;
        }



        #region Drag Window Functions
        // Handle Draggable Header Mouse Events
        private void DraggableHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _startPoint = e.GetPosition(this);
            DraggableHeader.CaptureMouse();
        }

        private void DraggableHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPoint = e.GetPosition(this);
                double offsetX = currentPoint.X - _startPoint.X;
                double offsetY = currentPoint.Y - _startPoint.Y;
                Left += offsetX;
                Top += offsetY;
            }
        }

        private void DraggableHeader_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            DraggableHeader.ReleaseMouseCapture();
        }
        #endregion
    }
}