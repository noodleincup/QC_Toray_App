using HandleDatabase;
using Org.BouncyCastle.Asn1.X509;
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
    /// Interaction logic for SetPattern_UserControl.xaml
    /// </summary>
    public partial class SetPattern_UserControl : System.Windows.Controls.UserControl
    {

        public ObservableCollection<GradeItem> GradeItems { get; set; } = new ObservableCollection<GradeItem>();
        public ObservableCollection<string> PatternNameItems { get; set; } = new ObservableCollection<string>();

        private DatabaseHandler databaseHandler = new DatabaseHandler(library.DatabaseConfig.ConnectionString1);
        private readonly string MASTER_GRADE_PATTERN_TABLE_NAME = DatabaseConfig.MasterGradePatternTableName;
        private readonly string PATTERN_TABLE_NAME = DatabaseConfig.MasterPatternTableName;
        
        private Dictionary<int, string> patternIdWithName = new Dictionary<int, string>();
        private readonly string[] masterPatternTableColumnName =
        {
            "ID", "updateDate", "updateBy", "gradeCode", "gradeName", "patternID"
        };
        

        private const int CANCELLATION_TIMEOUT_MS = 2000;
        private const int PATTERN_ID_COLUMN_INDEX = 0;
        private const int DESCRIPTION_COLUMN_INDEX = 1;

        public SetPattern_UserControl()
        {
            InitializeComponent();

            // Bidding data to DataGrid
            dgSetPattern.ItemsSource = GradeItems;

            this.Loaded += UserControl_Loaded;

            this.DataContext = this;
        }

        #region Load Data to DataGrid with Timeout
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Load data from database
            await LoadGradeItemsFromDatabaseAsync();
        }

        private async Task LoadGradeItemsFromDatabaseAsync() 
        {
            // 1. Create a CancellationTokenSource for timeout and control

            using (var cts = new CancellationTokenSource())
            {
                // 2. Set the timeout after which the token will be cancelled.
                cts.CancelAfter(CANCELLATION_TIMEOUT_MS);
                var cancellationToken = cts.Token;

                // Start the database tasks with the cancellation token.
                Task<DataTable> task1 = GetDataTableFromDatabaseAsync(cancellationToken);
                Task<Dictionary<int, string>> task2 = GetDictionaryPatternAsync(cancellationToken);

                // Create a single Task that completes when *both* tasks finish.
                var allTasks = Task.WhenAll(task1, task2);

                try
                {
                    // 3. Await the Task.WhenAll operation.
                    // If the timeout is hit, the CancellationTokenSource will raise 
                    // a TaskCanceledException in the awaiting thread.
                    await allTasks;

                    // 4. If successful, explicitly cancel the CancellationTokenSource's timer 
                    // (not strictly necessary but good practice).
                    cts.Cancel();

                    // 5. Get the results and proceed with processing (runs on UI thread non-blockingly).
                    DataTable table1 = task1.Result;
                    patternIdWithName = task2.Result;

                    // 6. Run the blocking process on a new non-UI thread.
                    await Task.Run(() =>
                    {
                        ProcessAndBindData(table1, patternIdWithName);
                    });

                    //MessageBox.Show("✅ Data loading and processing complete within the 2-second limit.");
                }
                catch (OperationCanceledException)
                {
                    // This is the exception caught when the cts.CancelAfter(2000) timeout occurs.
                    MessageBox.Show("⏱️ Timeout exceeded! Database fetching took more than 2 seconds and was canceled.", "Operation Timeout", MessageBoxButton.OK, MessageBoxImage.Warning);

                    // Optional: You may want to check the specific token that caused the cancellation
                    // to ensure it was the timeout, though for this code structure, it's the most likely cause.
                }
                catch (Exception ex)
                {
                    // Catch any other exceptions (e.g., database connection errors).
                    MessageBox.Show($"❌ An error occurred during fetching: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Task<DataTable> GetDataTableFromDatabaseAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                // Add logic to check the token periodically in a real-world complex scenario, 
                // though Task.Run will handle the cancellation check upon completion.
                cancellationToken.ThrowIfCancellationRequested();

                // This is your actual blocking database call.
                return databaseHandler.GetTableDatabaseAsDataTable(MASTER_GRADE_PATTERN_TABLE_NAME);
            }, cancellationToken); // Pass the token to Task.Run
        }

        private Task<Dictionary<int, string>> GetDictionaryPatternAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                Dictionary<int, string> dictPattern = new Dictionary<int, string>();

                DataTable patternDt = databaseHandler.GetTableDatabaseAsDataTable(PATTERN_TABLE_NAME);

                foreach (DataRow row in patternDt.Rows)
                {
                    int patternId = Convert.ToInt32(row[PATTERN_ID_COLUMN_INDEX]);
                    string? patternName = row[DESCRIPTION_COLUMN_INDEX].ToString();

                    if (patternId != 0 && patternName != null)
                    { 
                        dictPattern[patternId] = patternName;

                        PatternNameItems.Add(patternName);

                    }
                }

                if (dictPattern.Count == 0)
                {
                    Console.WriteLine("No patterns were added to the dictionary.");
                    return new Dictionary<int, string>();
                }

                return dictPattern;
            }, cancellationToken);
        }

        private void ProcessAndBindData(DataTable gradePatternTable, Dictionary<int, string> patternDict)
        {
            // Clear existing items
            Application.Current.Dispatcher.Invoke(() => GradeItems.Clear());
            int no = 1;

            if (patternDict.Count == 0)
            {
                Console.WriteLine("Pattern dictionary is empty.");
            }

            foreach (DataRow row in gradePatternTable.Rows)
            {
                string? grade = row[masterPatternTableColumnName[4]]?.ToString();
                string? gradeCode = row[masterPatternTableColumnName[3]]?.ToString();
                DateTime updateDate = row[masterPatternTableColumnName[1]] != DBNull.Value ? Convert.ToDateTime(row["UpdateDate"]) : DateTime.MinValue;
                string? updateBy = row[masterPatternTableColumnName[2]]?.ToString();
                int patternId = Convert.ToInt32(row[masterPatternTableColumnName[5]]);
                
                //Console.WriteLine($"Pattern ID: {patternDict[0]}");
                string patternName = patternId != 0 && patternDict.ContainsKey(patternId) ? patternDict[patternId] : string.Empty;
                GradeItem item = new GradeItem
                {
                    No = no++,
                    Grade = grade ?? string.Empty,
                    GradeCode = gradeCode ?? string.Empty,
                    UpdateDate = updateDate,
                    UpdateBy = updateBy ?? string.Empty,
                    Pattern = patternName ?? string.Empty,
                    //Description = description
                };
                // Add item to ObservableCollection on the UI thread
                Application.Current.Dispatcher.Invoke(() => GradeItems.Add(item));
            }
        }
        #endregion

        // New: handle ComboBox SelectionChanged inside DataGrid row
        private async void PatternComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo)
            {
                if (combo.DataContext is GradeItem item)
                {
                    string message =
                        $"No: {item.No}\n" +
                        $"Grade: {item.Grade}\n" +
                        $"Grade Code: {item.GradeCode}\n" +
                        $"Update Date: {item.UpdateDate}\n" +
                        $"Update By: {item.UpdateBy}\n" +
                        $"Pattern: {item.Pattern}\n";
                    Console.WriteLine($"Selected Pattern Changed: {message}");

                    foreach (var kvp in patternIdWithName)
                    {
                        if (kvp.Value == item.Pattern)
                        {
                            Console.WriteLine($"Matched Pattern ID: {kvp.Key} for Pattern Name: {kvp.Value}");
                            // Update the database with the new pattern ID

                            using (var cts = new CancellationTokenSource())
                            {
                                cts.CancelAfter(CANCELLATION_TIMEOUT_MS);
                                var cancellationToken = cts.Token;
                                Task task = UpdatePatternIdToDatabaseAsync(item.No, kvp.Key, cancellationToken);

                                try
                                {
                                    await task;
                                    cts.Cancel();
                                }
                                catch (OperationCanceledException)
                                {
                                    MessageBox.Show("⏱️ Timeout exceeded! Database update took more than 2 seconds and was canceled.", "Operation Timeout", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"❌ An error occurred during updating: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            break; // Exit the loop once we've found the match
                        }
                    }
                }
            }
        }

        private async Task UpdatePatternIdToDatabaseAsync(int id, int patternId, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                Dictionary<string, object> updateValues = new Dictionary<string, object>
                {
                    { "patternID", patternId }
                };

                bool result = databaseHandler.UpdateDataInTableById(updateValues, MASTER_GRADE_PATTERN_TABLE_NAME, "ID", id);
                if (result)
                {
                    Console.WriteLine($"Successfully updated patternID to {patternId} for record ID {id}.");
                }
                else
                {
                    Console.WriteLine($"Failed to update patternID for record ID {id}.");
                }
            }, cancellationToken);
        }

        private void MyDataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 1. Get the DataGrid control that fired the event.
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            // 2. Determine the hit object: Where exactly did the user click?
            // We use VisualTreeHelper to find the element at the click position.
            DependencyObject hitTest = dataGrid.InputHitTest(e.GetPosition(dataGrid)) as DependencyObject;

            // 3. Check if the click landed OUTSIDE of a DataGridRow.
            // If the click lands on a row/cell, hitTest will contain a DataGridRow/Cell.
            // We check if the hit object is NOT a DataGridRow, which means the click was on the background area.
            if (hitTest != null && FindVisualParent<DataGridRow>(hitTest) == null)
            {
                // 4. Clear the selection and focus.
                dataGrid.UnselectAll();

                // To ensure the keyboard focus is removed, 
                // we set the focus to the DataGrid control itself.
                dataGrid.Focus();

                // 5. Mark the event as handled to stop further processing 
                // (e.g., preventing a double-click event on the background).
                e.Handled = true;

                return;
            }
        }

        /// <summary>
        /// Helper method to find a visual parent of a specific type.
        /// Used here to check if the clicked element is nested inside a DataGridRow.
        /// </summary>
        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is T typedParent)
                {
                    return typedParent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        // Debugging helper to display dictionary contents
        private void DisplayDictionaryContents(Dictionary<int, string> dict)
        {
            foreach (var kvp in dict)
            {
                Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
        }
    }

    public class GradeItem : INotifyPropertyChanged
    {

        private int _no;
        private string _grade;
        private string _gradeCode;
        private DateTime _updateDate;
        private string _updateBy;
        private string _pattern;
        private string _description;

        public int No
        {
            get { return _no; }
            set{ _no = value; OnPropertyChanged(nameof(No)); }
        }

        public string Grade
        {
            get { return _grade; }
            set { _grade = value; OnPropertyChanged(nameof(Grade)); }
        }

        public string GradeCode
        {
            get { return _gradeCode; }
            set { _gradeCode = value; OnPropertyChanged(nameof(GradeCode)); }
        }

        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; OnPropertyChanged(nameof(UpdateDate)); }
        }

        public string UpdateBy { 
            get { return _updateBy; } 
            set { _updateBy = value; OnPropertyChanged(nameof(UpdateBy)); }
        }

        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; OnPropertyChanged(nameof(Pattern)); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        // Boilerplate INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
