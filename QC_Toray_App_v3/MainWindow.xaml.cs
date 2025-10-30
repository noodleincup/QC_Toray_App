using ControlzEx.Standard;
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
        private bool _isDragging = false;
        private Point _startPoint;
        private bool loginStatus = false;

        private string lotData = "";
        private string gradeData = "";

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            // Attach event listener when LoginUserControl is added
            var loginControl = new LoginUserControl();
            loginControl.ChangePageRequested += OnChangePageRequested;
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
            GridMain.Children.Add(usc);
        }
        #endregion
        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.UserControl usc = null;
            GridMain.Children.Clear();

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "Login":
                    usc = new LoginUserControl();
                    GridMain.Children.Add(usc);
                    break;
                case "MainTable":
                    MainTable_UserControl mainTable = new MainTable_UserControl();
                    mainTable.ChangePageRequested += OnChangePageRequested;
                    mainTable.UpdateLotAndGradeData += OnUpdateLotAndGradeData; // OnUpdateLotAndGradeData
                    GridMain.Children.Add(mainTable);
                    break;
                case "LotOverview":
                    LotOverviewUserControl lotOverview = new LotOverviewUserControl(lotData, gradeData);
                    lotOverview.ChangePageRequested += OnChangePageRequested;
                    GridMain.Children.Add(lotOverview);
                    break;
                case "Operating":
                    usc = new OperationUserControl();
                    GridMain.Children.Add(usc);
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

        private void OnUpdateLotAndGradeData(object sender, string lot_grade)
        {
            string[] parts = lot_grade.Split(',');
            lotData = parts[0];
            gradeData = parts[1];

            //MessageBox.Show($"Lot Data: {lotData}\nGrade Data: {gradeData}", "Lot and Grade Data Updated", MessageBoxButton.OK, MessageBoxImage.Information);
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