using HandleDatabase;
using QC_Toray_App_v3.library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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


namespace QC_Toray_App_v3
{
    /// <summary>
    /// Interaction logic for LotOverviewUserControl.xaml
    /// </summary>
    public partial class LotOverviewUserControl : System.Windows.Controls.UserControl
    {
        public event EventHandler<string> ChangePageRequested;
        private const int DEFAULT_BATCH_NUM= 30; // Default number of BatchDetailItem controls
        private string userLotData;
        private string userGradeData;
        private string userOrderNo;

        StackPanel _buttonPanel;
        public LotOverviewUserControl(string orderNo, string lotData, string gradeData)
        {
            InitializeComponent();
            initializeBatchDetail();
            ToggleIsEnableBatchItem();

            // Set initial values for Lot and Grade
            userLotData = lotData;
            userGradeData = gradeData;
            userOrderNo = orderNo;

            Console.WriteLine($"LotOverviewUserControl initialized with\n Lot: {userLotData}\n Grade: {userGradeData}\n OrderNo: {userOrderNo} ");

            txbLot.Text = userLotData;
            cbxName.Text = userGradeData;
            tblOrderNo.Text = userOrderNo;

            cbxSTD.IsEnabled = true;
            cbxPattern.IsEnabled = true;
            cbxMasterStandard.IsEnabled = true;

            txbBatchNumber.IsEnabled = false;
            txbStart.IsEnabled = false;

            this.DataContext = LotOverviewViewModel.Instance;
        }

        private async void LotOverviewUserControl_Loaded(object sender, RoutedEventArgs e)
        {

            await LotOverviewViewModel.Instance.LoadSampleGroupAsync();
            await LotOverviewViewModel.Instance.LoadPatternAsync();
            await LotOverviewViewModel.Instance.LoadItemDiameter();
            // 🎯 Setting the DataContext in the code-behind
        }

        public void initializeBatchDetail(int startNum)
        {
            wrpBatchDetail.Children.Clear();
            // 0. Create default BatchDetailItem controls
            for (int i = 0; i < DEFAULT_BATCH_NUM; i++)
            {
                BatchDetailItem item = new BatchDetailItem();
                item.ChangePageRequested += OnChangePageRequested;
                item.ItemValue = (i + 1 + startNum).ToString();
                wrpBatchDetail.Children.Add(item);
            }

            //// 1. Create the StackPanel (the container)
            //_buttonPanel = new StackPanel
            //{
            //    Orientation = Orientation.Horizontal,
            //    Margin = new Thickness(0, 10, 0, 0), // Optional: add some top margin
            //    Width = 300 // Optional: match the width of the UserControls
            //};

            //// 2. Create the 'Add' Button
            //Button addButton = new Button
            //{
            //    Content = "Add",
            //    Width = 100,
            //    Height = 30,
            //    Margin = new Thickness(20, 5, 5, 5)
            //};
            //// Attach click event handler
            //addButton.Click += AddDetail_Click;

            //// 3. Create the 'Remove' Button
            //Button removeButton = new Button
            //{
            //    Content = "Remove",
            //    Width = 100,
            //    Height = 30,
            //    Margin = new Thickness(20, 5, 5, 5),
            //    Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC864C8"),
            //    BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC864C8")
            //};
            //// Attach click event handler
            //removeButton.Click += RemoveDetail_Click;

            //// 4. Add the buttons to the StackPanel
            //_buttonPanel.Children.Add(addButton);
            //_buttonPanel.Children.Add(removeButton);

            //// 5. Add the StackPanel to the WrapPanel as the last child
            //// This ensures the button group is always at the end, even 
            //// if the WrapPanel wraps to a new line.
            //wrpBatchDetail.Children.Add(_buttonPanel);
        }

       
        #region Button Click Handlers
        private void AddDetail_Click(object sender, RoutedEventArgs e)
        {
            // 1. Temporarily remove the button panel
            wrpBatchDetail.Children.Remove(_buttonPanel);

            // 2. Create and add the new UserControl
            BatchDetailItem newItem = new BatchDetailItem();
            newItem.ItemValue = (wrpBatchDetail.Children.Count + 1).ToString();
            wrpBatchDetail.Children.Add(newItem);

            // 3. Re-add the button panel to ensure it is the last child
            wrpBatchDetail.Children.Add(_buttonPanel);
        }

        private void RemoveDetail_Click(object sender, RoutedEventArgs e)
        {
            // 1. Temporarily remove the button panel
            wrpBatchDetail.Children.Remove(_buttonPanel);

            // 2. Find and remove the last BatchDetailItem (assuming it's not the button panel itself)
            // We iterate backwards to easily find the last *actual* item
            for (int i = wrpBatchDetail.Children.Count - 1; i >= 0; i--)
            {
                if (wrpBatchDetail.Children[i] is BatchDetailItem)
                {
                    wrpBatchDetail.Children.RemoveAt(i);
                    break; // Stop after removing one item
                }
            }

            // 3. Re-add the button panel to ensure it is the last child
            // This is safe even if no BatchDetailItem was found and removed.
            wrpBatchDetail.Children.Add(_buttonPanel);
        }

        private void btnBatchDiameter_Clicked(object sender, RoutedEventArgs e)
        {
            var batchDiameterWindow = new BatchDiameterDatail_Window(cbxName.SelectedValue?.ToString(), userLotData, txbBatchNumber.Text)
            {
                Owner = Window.GetWindow(this)
            };
            bool? dialogResult = batchDiameterWindow.ShowDialog();

            if (dialogResult == true)
            {
                MessageBox.Show($"dialogResult: {dialogResult}");
            }
            else
            {
                MessageBox.Show("Batch Diameter window was closed without confirmation.");
            }
        }

        private void txbBatchNumber_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var batchDiameterWindow = new BatchDiameterDatail_Window(cbxName.Text, txbLot.Text, txbBatchNumber.Text)
            {
                Owner = Window.GetWindow(this)
            };
            bool? dialogResult = batchDiameterWindow.ShowDialog();

            if (dialogResult == true)
            {
                MessageBox.Show($"dialogResult: {dialogResult}");
                //Console.WriteLine(batchDiameterWindow.ResultString);
            }
            else
            {
                MessageBox.Show("Batch Diameter window was closed without confirmation.");
            }
        }

        private void btnRecord_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Record button clicked!");
        }
        private void btnSendData_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Send Data button clicked!");
        }
        
        private void cbxSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var child in wrpBatchDetail.Children)
            {
                if (child is BatchDetailItem item)
                {
                    item.IsItemSelected = true;
                }
            }
        }

        private void cbxSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var child in wrpBatchDetail.Children)
            {
                if (child is BatchDetailItem item)
                {
                    item.IsItemSelected = false;
                }
            }
        }

        private void btnInitial_Click(object sender, RoutedEventArgs e)
        {
            cbxSTD.IsEnabled = false;
            cbxPattern.IsEnabled = false;
            cbxMasterStandard.IsEnabled = false;

            cbxSelectAll.IsEnabled = true;
            txbBatchNumber.IsEnabled = true;
            txbStart.IsEnabled = true;

            ToggleIsEnableBatchItem();
            
        }
       
        #endregion

        private void txbBatchStart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox? txb = sender as TextBox;

                if (txb == null) { 
                    MessageBox.Show("object is null");
                    return;
                }

                MessageBox.Show($"TextBox is press with enter {txb.Name}, {txb.Text}");
            }
        }

        private void ToggleIsEnableBatchItem()
        {
            foreach (var child in wrpBatchDetail.Children) 
            { 
                if (child is BatchDetailItem item) 
                {
                    
                    item.IsEnabled = item.IsEnabled? false : true;
                } 
            }
        }

        private void OnChangePageRequested(object sender, string pageName)
        {
            ChangePageRequested?.Invoke(this, pageName);

        }
    }

    public class LotOverviewViewModel : INotifyPropertyChanged
    {
        // Private static field to hold the single instance
        private static readonly Lazy<LotOverviewViewModel> lazyInstance = new Lazy<LotOverviewViewModel>(() => new LotOverviewViewModel());
        private DatabaseHandler databaseHandler = new DatabaseHandler(DatabaseConfig.ConnectionString1);


        private LotOverviewViewModel()
        {
            // Initialize ObservableCollections here or in a load method
            SampleGroups = new ObservableCollection<SampleGroup>();
            Patterns = new ObservableCollection<Pattern>();
            ItemDiameters = new ObservableCollection<ItemDiameter>();
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

        // --- Data for the third ComboBox ---
        public ObservableCollection<ItemDiameter> ItemDiameters { get; set; }
        private ItemDiameter _selectedItemDiameter;
        public ItemDiameter SelectedItemDiameter
        {
            get => _selectedItemDiameter;
            set
            {
                if (_selectedItemDiameter != value)
                {
                    _selectedItemDiameter = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task LoadSampleGroupAsync()
        {
            using (var cts = new CancellationTokenSource(DatabaseConfig.TimeoutMs)) 
            {
                //cts.CancelAfter(DatabaseConfig.TimeoutMs);
                var cancellationToken = cts.Token;

                Console.WriteLine("Load Sample Groups Process");
                //Task<DataTable> task =  GetSampleGroupsFromDatabase(cancellationToken);
                DataTable sampleGroupDt = await GetSampleGroupsFromDatabase(cancellationToken);
                Console.WriteLine("End load process");

                try
                {
                    //cts.Cancel();

                    //DataTable sampleGroupDt = task.Result;

                    await Task.Run(() => {

                        Console.WriteLine("Update SampleGroups collection Process");

                        Application.Current.Dispatcher.Invoke(() => 
                        {
                            SampleGroups.Clear();
                            for (int i = 0; i < sampleGroupDt.Rows.Count; i++)
                            {
                                SampleGroup sampleGroup = new SampleGroup()
                                {
                                    ID = Convert.ToInt32(sampleGroupDt.Rows[i]["id"]),
                                    SampleName = sampleGroupDt.Rows[i]["sampleName"].ToString(),
                                    UpdatedBy = sampleGroupDt.Rows[i]["updateBy"].ToString(),
                                    UpdateDate = Convert.ToDateTime(sampleGroupDt.Rows[i]["updateDate"])
                                };

                                SampleGroups.Add(sampleGroup);

                            }
                            SelectedGroup = SampleGroups[0];
                            Console.WriteLine($"Amount rows: {sampleGroupDt.Rows.Count}");

                            Console.WriteLine("End update process");
                        });
                        
                    });
                }
                catch (OperationCanceledException)
                {
                    // This is caught if the CTS token is canceled (due to timeout or manual cancellation)
                    MessageBox.Show("Data loading timed out after 2 seconds or was cancelled.");
                }
                catch (Exception ex) 
                {
                    MessageBox.Show($"Error: {ex}");
                }
            }
        }

        private async Task<DataTable> GetSampleGroupsFromDatabase(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return databaseHandler.GetTableDatabaseAsDataTable(DatabaseConfig.SampleGroupTableName);
        }

        public async Task LoadPatternAsync()
        {
            DataTable patternDt;
            try
            {
                using (var cts = new CancellationTokenSource(DatabaseConfig.TimeoutMs))
                {
                    Console.WriteLine("Load Pattern process");
                    patternDt = await GetPatternFromDatabaseAsync(cts.Token);
                    databaseHandler.DisplayHeader(patternDt);
                    databaseHandler.ShowDataTable(patternDt);
                    Console.WriteLine("Load Pattern end.");
                }
                Console.WriteLine("Update Patterns");
                await Task.Run(() => 
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Patterns.Clear();
                        foreach (DataRow row in patternDt.Rows)
                        {
                            Pattern pattern = new Pattern()
                            {
                                PatternID = Convert.ToInt32(row["patternID"]),
                                UpdateDate = Convert.ToDateTime(row["updateDate"]),
                                UpdatedBy = row["updateBy"].ToString(),
                                PatternName = row["patternName"].ToString(),
                                Description = row["description"].ToString()
                            };

                            Patterns.Add(pattern);
                        }
                        if (patternDt.Rows.Count > 0)
                        {
                            SelectedPattern = Patterns[0];
                        }
                    });
                });
                Console.WriteLine("Update Patterns end.");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Data loading timed out after 2 seconds or was cancelled.",
                    "Load Time out",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e}",
                    "Error occur",
                     MessageBoxButton.OK,
                     MessageBoxImage.Error);
            }
        }

        private async Task<DataTable> GetPatternFromDatabaseAsync(CancellationToken cancellationToken) 
        {
            cancellationToken.ThrowIfCancellationRequested();
            return databaseHandler.GetTableDatabaseAsDataTable(DatabaseConfig.MasterPatternTableName);
        }

        public async Task LoadItemDiameter() 
        {
            DataTable masterDiameterDt;
            try
            {
                using (var cts = new CancellationTokenSource(DatabaseConfig.TimeoutMs))
                {
                    Console.WriteLine("Load Master Diameter from database");
                    masterDiameterDt = await GetMasterDiameterFromDatabaseAsync(cts.Token);
                    databaseHandler.DisplayHeader(masterDiameterDt);
                    databaseHandler.ShowDataTable(masterDiameterDt);
                    Console.WriteLine("End Load");

                }

                await Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    ItemDiameters.Clear();
                    foreach (DataRow row in masterDiameterDt.Rows)
                    {
                        ItemDiameter itemDiameter = new ItemDiameter()
                        {
                            ID = Convert.ToInt32(row["ID"]),
                            ItemName = row["itemName"].ToString(),
                            A_Min = Convert.ToInt32(row["a_min"]),
                            A_Max = Convert.ToInt32(row["a_max"]),
                            B_Min = Convert.ToInt32(row["b_min"]),
                            B_Max = Convert.ToInt32(row["b_max"]),
                            L_Min = Convert.ToInt32(row["l_min"]),
                            L_Max = Convert.ToInt32(row["l_max"])
                        };

                        ItemDiameters.Add(itemDiameter);
                    }
                    SelectedItemDiameter = ItemDiameters[0];
                });
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Data loading timed out after 2 seconds or was cancelled.",
                    "Load Time out",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e}",
                    "Error occur",
                     MessageBoxButton.OK,
                     MessageBoxImage.Error);
            }
        }

        private async Task<DataTable> GetMasterDiameterFromDatabaseAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return databaseHandler.GetTableDatabaseAsDataTable(DatabaseConfig.MasterDiameterTableName);
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

    public class ItemDiameter 
    { 
        public int ID { get; set; }
        public string ItemName { get; set; }
        public double A_Min {  get; set; }
        public double A_Max { get; set; }
        public double B_Min { get; set; }
        public double B_Max { get; set; }
        public double L_Min { get; set; }
        public double L_Max { get; set; }

    }
}
