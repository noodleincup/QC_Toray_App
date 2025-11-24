using HandleDatabase;
using QC_Toray_App_v3.library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace QC_Toray_App_v3.Windows
{
    /// <summary>
    /// Interaction logic for MasterSampleLimit.xaml
    /// </summary>
    public partial class MasterSampleLimit_Window : Window
    {
        // which automatically notifies the DataGrid of additions/deletions.
        public ObservableCollection<ItemData> ItemList { get; set; }


        private DatabaseHandler databaseHandler;
        private int groupId;
        private bool isEditMode = true;
        private const int ROW_ORDER_COLUMN_INDEX = 1;
        private const int ITEM_NAME_COLUMN_INDEX = 2;
        private const int SAMPLE_ID_COULMN_INDEX = 4;

        private string[] items = new string[]
        {
            "Misscut_DoubleLen", "Misscut_TripleLen", "Misscut_Fourfold",                   // 3
            "Linkage_Double", "Linkage_Triple", "Linkage_Fourfold", "Linkage_Fivefold",     // 4
            "CuttingPlane_L1", "CuttingPlane_L2", "CuttingPlane_L3",                        // 3
            "GFNotDist_L1", "GFNotDist_L2", "GFNotDist_L3",                                 // 3
            "Meyani", "Meyani_Color",                                                       // 2
            "ForeignMaterials",                                                             // 1
            "BlackSpot_SS", "BlackSpot_S","BlackSpot_M", "BlackSpot_L",                     // 4
            "ColorAbnormal",                                                                // 1
            "Macaroni_SS", "Macaroni_S", "Macaroni_M", "Macaroni_L"                         // 4
        };

        Dictionary<string, object> itemDataDict = new Dictionary<string, object> {
            { "@user", "admin" },
            { "@groupId", 0 },
            { "@typeId", 0 },
            { "@qty", 0 },
            { "@rangeMin", 0 },
            { "@rangeMax", 0 }
        };

        public MasterSampleLimit_Window(DatabaseHandler handler, bool isEdit = true, int id = -1)
        {
            InitializeComponent();

            databaseHandler = handler;
            groupId = id;
            isEditMode = isEdit;

            // Initialize the collection
            ItemList = new ObservableCollection<ItemData>();
            MyDataGrid.ItemsSource = ItemList;

            this.Loaded += (s, e) => {
                MasterSampleLimit_Window_Loaded(s, e);
            };
        }


        #region Load DataGrid functions and helpers

        private async void MasterSampleLimit_Window_Loaded(object? sender, RoutedEventArgs e)
        {
            this.Loaded -= MasterSampleLimit_Window_Loaded; // detach if run only once

            try
            {
                await LoadDataGrid();
            }
            catch (Exception ex)
            {
                // Surface any exceptions so you can see what went wrong
                MessageBox.Show(ex.ToString(), "Initialization error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadDataGrid()
        {
            
            await LoadSampleItemName();
            DataTable masterSampleItemNameDataTable = databaseHandler.GetTableDatabaseAsDataTable(DatabaseConfig.MasterSampleItemTableName);
            DataTable masterSampleLimitDataTable;

            if (!isEditMode) {
                await LoadNullToDataGrid(masterSampleItemNameDataTable);
                return;
            }


            masterSampleLimitDataTable = LoadMasterSampleLimitFromDatabase();
            LoadRowDataToItemList(masterSampleLimitDataTable, masterSampleItemNameDataTable);

            databaseHandler.DisplayHeader(masterSampleLimitDataTable);
            databaseHandler.ShowDataTable(masterSampleLimitDataTable);


            if (masterSampleLimitDataTable.Rows.Count == 0)
            {
                MessageBox.Show("No data found for the specified group ID.", 
                                "Get Database error", 
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
            else
            {
                Console.WriteLine($"Rows found: {masterSampleLimitDataTable.Rows.Count}");
            }


            if (masterSampleLimitDataTable.Rows.Count != 25)
            {
                MessageBox.Show("Data count mismatch. Expected 25 rows.",
                                "Get Database error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
        }

        private async Task LoadNullToDataGrid(DataTable sampleItemNameTbl)
        {
            foreach (DataRow row in sampleItemNameTbl.Rows)
            {
                ItemList.Add(new ItemData
                {
                    Items = row[ITEM_NAME_COLUMN_INDEX].ToString(),
                    MaxOKQty = "-",
                    RangeMin = "-",
                    RangeMax = "-",
                    SampleId = row[ROW_ORDER_COLUMN_INDEX] != DBNull.Value ? Convert.ToInt32(row[ROW_ORDER_COLUMN_INDEX]) : -1
                });
            }
        }

        private void LoadRowDataToItemList(DataTable sampleLimitTbl, DataTable sampleItemNameTbl)
        {
            string columnNameToFind = sampleItemNameTbl.Columns[ROW_ORDER_COLUMN_INDEX].ColumnName;
            foreach (DataRow row in sampleLimitTbl.Rows)
            {
                int valueToFind = Convert.ToInt32(row[SAMPLE_ID_COULMN_INDEX]);
                string filter = string.Format("{0} = '{1}'", columnNameToFind, valueToFind);

                DataRow[] foundRows = sampleItemNameTbl.Select(filter);

                if (foundRows.Length == 0)
                {
                    MessageBox.Show($"No matching item found for Sample ID: {valueToFind}",
                                    "Get Database error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                ItemList.Add(new ItemData
                {
                    Items = foundRows[0][ITEM_NAME_COLUMN_INDEX].ToString(),
                    MaxOKQty = (row[5].ToString() != "") ? row[5].ToString() : "-",
                    RangeMin = (row[6].ToString() != "") ? row[6].ToString() : "-",
                    RangeMax = (row[7].ToString() != "") ? row[7].ToString() : "-",
                    SampleId = Convert.ToInt32(row[SAMPLE_ID_COULMN_INDEX])
                });
            }
        }

        private async Task LoadSampleItemName()
        {
            DataTable dt = databaseHandler.GetTableDatabaseAsDataTable(DatabaseConfig.MasterSampleItemTableName);

            foreach (DataRow row in dt.Rows)
            {
                int rowOrder = Convert.ToInt16(row[ROW_ORDER_COLUMN_INDEX]) - 1;
                items[rowOrder] = row[ITEM_NAME_COLUMN_INDEX].ToString();  // dt.Rows[i][ITEM_NAME_COLUMN_INDEX].ToString();
            }

        }

        private DataTable LoadMasterSampleLimitFromDatabase()
        {

            string condition = $"groupID = {groupId}";
            DataTable dt = databaseHandler.GetTableDatabaseAsDataTableWithCondition(DatabaseConfig.MasterSampleLimitTableName, condition);
            
            return dt;
        }
        #endregion



        #region button event handlers
        
        public void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveAllRowToDatabase();

            this.DialogResult = true;
            this.Close();
        }

        #endregion


        #region Save DataGrid to database functions and helpers

        private async Task SaveAllRowToDatabase()
        {
            foreach (var item in ItemList)
            {
                await SaveEachRowDataToDatabase(groupId, item.SampleId, item.MaxOKQty, item.RangeMin, item.RangeMax);
            }
        }

        private async Task SaveEachRowDataToDatabase(int groupId, int sampleId, string qty, string rangeMin, string rangeMax)
        {
            string userInput = GlobalState.Instance.UserName;
            int? groupIdInput = (groupId == -1)? null : groupId;
            int sampleIdInput = sampleId;
            int? qtyInput = (qty == "-") ? null : Convert.ToInt32(qty);
            double? rangeMinInput = (rangeMin == "-") ? null : Convert.ToDouble(rangeMin);
            double? rangeMaxInput = (rangeMax == "-") ? null : Convert.ToDouble(rangeMax);


            Dictionary<string, object> paramDict = GetParameterInputDictionary(userInput, groupIdInput ?? -1, sampleIdInput, qtyInput, rangeMinInput, rangeMaxInput);

            if (groupId == -1)
            {
                MessageBox.Show("Group ID is invalid.", "Save Database error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataSet outputDs = databaseHandler.ExecuteStoredProcedure(DatabaseConfig.UpdateOrInsertMasterSampleItemProcedure, paramDict);


            //MessageBox.Show(outputDs.Tables[0].Rows[0][0].ToString(), outputDs.Tables[0].Columns[0].ColumnName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private Dictionary<string, object> GetParameterInputDictionary(string user, int groupId, int sampleId, int? qty, double? rangeMin, double? rangeMax)
        {
            Dictionary<string, object> itemDataDict = new Dictionary<string, object> { };
            itemDataDict["@user"] = user;
            itemDataDict["@groupId"] = groupId;
            itemDataDict["@typeId"] = sampleId;
            itemDataDict["@qty"] = (qty==null)? DBNull.Value : qty;
            itemDataDict["@rangeMin"] = (rangeMin==null)? DBNull.Value : rangeMin;
            itemDataDict["@rangeMax"] = (rangeMax == null) ? DBNull.Value : rangeMax;

            return itemDataDict;
        }



        #endregion
    }

    public class ItemData : INotifyPropertyChanged
    {
        private string _items;
        private string _maxOKQty;
        private string _rangeMin;
        private string _rangeMax;
        private int _sampleId;

        // Items column is read-only, so no setter is strictly needed for the DataGrid
        // but we include it for initial data population.
        public string Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged(nameof(Items)); }
        }

        public string MaxOKQty
        {
            get { return _maxOKQty; }
            set { _maxOKQty = value; OnPropertyChanged(nameof(MaxOKQty)); }
        }

        public string RangeMin
        {
            get { return _rangeMin; }
            set { _rangeMin = value; OnPropertyChanged(nameof(RangeMin)); }
        }

        public string RangeMax
        {
            get { return _rangeMax; }
            set { _rangeMax = value; OnPropertyChanged(nameof(RangeMax)); }
        }

        public int SampleId
        {
            get { return _sampleId; }
            set { _sampleId = value; OnPropertyChanged(nameof(SampleId)); }
        }

        // Boilerplate INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
