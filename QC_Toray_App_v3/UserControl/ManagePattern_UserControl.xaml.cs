using HandleDatabase;
using QC_Toray_App_v3.library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QC_Toray_App_v3.UserControl
{
    /// <summary>
    /// Interaction logic for ManagePattern_UserControl.xaml
    /// </summary>
    public partial class ManagePattern_UserControl : System.Windows.Controls.UserControl
    {
        public event EventHandler<string> ChangePageRequested;
        public ObservableCollection<PatternItem> PatternList { get; set; } = new ObservableCollection<PatternItem>();

        private DatabaseHandler databaseHandler = new DatabaseHandler(DatabaseConfig.ConnectionString1);
        private DataTable dt;
        private string MANNAGE_PATTREN_TABLE = DatabaseConfig.MasterPatternTableName;
        private string UPDATE_INSERT_PATTERN_PROCEDURE = DatabaseConfig.InserOrUpdateMasterPatternProcedure;
        private const int TimeoutMilliseconds = 2000; // 2 seconds


        public ManagePattern_UserControl()
        {
            InitializeComponent();

            dgManagePattern.ItemsSource = PatternList;

            LoadDataToDataGrid_2().ConfigureAwait(false);
        }

        private async void ManagePattern_UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => // detach when run only once
            {
                { Console.WriteLine("Hello World"); }
            });
        }

        private async Task LoadDataToDataGrid_2()
        {
            Task<DataTable> databaseTask = Task.Run(() =>
            {
                // This runs on a thread pool thread, not the UI thread
                return databaseHandler.GetTableDatabaseAsDataTable(MANNAGE_PATTREN_TABLE);
            });

            try
            {
                // 2. Wait for the database task to complete, but only up to the timeout
                if (await Task.WhenAny(databaseTask, Task.Delay(TimeoutMilliseconds)) == databaseTask)
                {
                    // The databaseTask finished within the timeout
                    dt = await databaseTask; // Get the result and re-throw any exception from the task
                    databaseHandler.ShowDataTable(dt);

                    AddPatternDataToList(dt);
                }
                else
                {
                    // The Task.Delay finished first, indicating a timeout
                    MessageBox.Show($"Database loading operation timed out (more than {(int)(TimeoutMilliseconds/1000)} seconds). Skipping data binding.", "Timeout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // The databaseTask might still be running in the background. 
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions from the database operation itself
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddPatternDataToList(DataTable patternDt)
        {
            if (dt != null)
            {
                PatternList.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    PatternList.Add(new PatternItem
                    {
                        No = Convert.ToInt32(row[0]),
                        Name = row[1].ToString(),
                        Description = row[2].ToString()
                    });
                }
            }
        }

        private async Task LoadDataToDataGrid()
        {
            DataTable dataTable = databaseHandler.GetTableDatabaseAsDataTable(DatabaseConfig.MasterPatternTableName);

            // 1. Run the potentially slow database operation on a background thread
            Task<DataTable> databaseTask = Task.Run(() =>
            {
                // This runs on a thread pool thread, not the UI thread
                return databaseHandler.GetTableDatabaseAsDataTable(MANNAGE_PATTREN_TABLE);
            });

            try
            {
                // 2. Wait for the database task to complete, but only up to the timeout
                if (await Task.WhenAny(databaseTask, Task.Delay(TimeoutMilliseconds)) == databaseTask)
                {
                    // The databaseTask finished within the timeout
                    dt = await databaseTask; // Get the result and re-throw any exception from the task


                    DataView view = new DataView(dt);
                    DataTable displayData = view.ToTable(false, dt.Columns[0].ColumnName, dt.Columns[1].ColumnName, dt.Columns[2].ColumnName);

                    // 4. Process and bind the data on the UI thread
                    //TrimDataTable(dt);
                    dgManagePattern.ItemsSource = displayData.DefaultView;
                    ConfigureDataGrid();
                }
                else
                {
                    // The Task.Delay finished first, indicating a timeout
                    MessageBox.Show("Database loading operation timed out (more than 2 seconds). Skipping data binding.", "Timeout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // The databaseTask might still be running in the background. 
                    // Depending on your databaseHandler, you might want a CancellationToken here to stop it.
                    // For simplicity, we just move on.
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions from the database operation itself
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConfigureDataGrid()
        {
            // 1. Make columns stretch to use all available width.
            // DataGridLength.Star will distribute the remaining space proportionally.
            dgManagePattern.ColumnWidth = new DataGridLength(1, DataGridLengthUnitType.Star);

            // 2. Prevent user from resizing columns.
            dgManagePattern.CanUserResizeColumns = false;

            // but may still be used if you set a specific width for column 0.
            //double standardColumnWidth = 200;

            // Set Style Header
            var modernHeaderStyle = new Style(typeof(DataGridColumnHeader));
            modernHeaderStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Color.FromRgb(250, 250, 250)))); // Light Gray
            modernHeaderStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            modernHeaderStyle.Setters.Add(new Setter(VerticalContentAlignmentProperty, VerticalAlignment.Center));
            //modernHeaderStyle.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch));

            // All other columns - Center aligned
            for (int i = 1; i < dgManagePattern.Columns.Count; i++)
            {
                 //Set uniform column width
                dgManagePattern.Columns[i].Width = dgManagePattern.ColumnWidth;

                dgManagePattern.Columns[i].CellStyle = new Style(typeof(DataGridCell))
                {
                    Setters = {
                                new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center),  // Center horizontal alignment
                                new Setter(VerticalAlignmentProperty, VerticalAlignment.Center),  // Center vertical alignment
                                new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),  // Ensure text is centered
                                new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center),
                                new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center),
                                //new Setter(TextBlock.IsRead, 14.0),
                                //new Setter(TextBox.VerticalAlignmentProperty, VerticalAlignment.Center),
                                //new Setter(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Center),
                                //new Setter(TextBox.TextAlignmentProperty, TextAlignment.Center),
                                new Setter(TextBox.IsReadOnlyProperty, true)
                    }
                };

                // Center align headers
                dgManagePattern.Columns[i].HeaderStyle = modernHeaderStyle;
            }

            // If you want column 0 to be a fixed size (e.g., 50 pixels) and the rest to stretch:
            if (dgManagePattern.Columns.Count > 0)
            {
                dgManagePattern.Columns[0].Width = 80; // Fixed width of 50 for the first column
            }
        }

        #region Button Events

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PatternItem lastPattern = PatternList.LastOrDefault();

            PatternItem newPatternItem = new PatternItem
            {
                No = (lastPattern is null)? 1 : lastPattern.No + 1,
                Name = "New Pattern",
                Description = "Description"
            };

            PatternList.Add(newPatternItem);
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, object> patternData = new Dictionary<string, object>
                {
                    { "@No", 0 },
                    { "@Name", "" },
                    { "@Description", "" },
                    { "@User", GlobalState.Instance.UserName }
                };

                // Save PatternList to database
                foreach (var pattern in PatternList)
                {
                    patternData["@No"] = pattern.No;
                    patternData["@Name"] = pattern.Name;
                    patternData["@Description"] = pattern.Description;

                    DataSet ds = databaseHandler.ExecuteStoredProcedure(UPDATE_INSERT_PATTERN_PROCEDURE, patternData);

                    string? message = ds.Tables[0].Rows[0][0].ToString();

                    Console.WriteLine($"Save Pattern No {pattern.No} by {patternData["@User"]}: {message}");
                }

                MessageBox.Show("All patterns have been saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        #endregion
    }

    public class PatternItem : INotifyPropertyChanged
    {
        private int _no;
        private string _name;
        private string _description;

        public int No
        {
            get { return _no; }
            set
            {
                if (_no != value)
                {
                    _no = value;
                    OnPropertyChanged(nameof(No));
                }
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        // Boilerplate INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
