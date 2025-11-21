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

        DatabaseHandler databaseHandler;
        private int sampleId;

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

        public MasterSampleLimit_Window(DatabaseHandler handler, int id)
        {
            InitializeComponent();

            databaseHandler = handler;
            sampleId = id;

            // Initialize the collection
            ItemList = new ObservableCollection<ItemData>();
            MyDataGrid.ItemsSource = ItemList;

            LoadDataGrid();
        }

        private async Task LoadDataGrid()
        {

            DataTable dt = LoadMasterSampleLimitFromDatabase();

            databaseHandler.DisplayHeader(dt);
            databaseHandler.ShowDataTable(dt);

            if (dt.Rows.Count != 25) { 
                MessageBox.Show("Data count mismatch. Expected 25 rows.", 
                                "Get Database error", 
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return; 
            }

            // Example: Add a few rows
            for (int i = 0; i < 25; i++) 
            {
                if(Convert.ToInt32(dt.Rows[i][4]) != i + 1) 
                {
                    MessageBox.Show("Sequence sample id data is invalid", 
                                    "Get Database error", 
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }


                ItemList.Add(new ItemData { Items = items[i], 
                                            MaxOKQty = (dt.Rows[i][5].ToString() != "")? dt.Rows[i][5].ToString() : "-", 
                                            RangeMin = (dt.Rows[i][6].ToString() != "") ? dt.Rows[i][6].ToString() : "-", 
                                            RangeMax = (dt.Rows[i][7].ToString() != "") ? dt.Rows[i][7].ToString() : "-"
                }
                );
            }
        }

        private DataTable LoadMasterSampleLimitFromDatabase()
        {

            string condition = $"groupID = {sampleId}";
            DataTable dt = databaseHandler.GetTableDatabaseAsDataTableWithCondition(DatabaseConfig.MasterSampleLimitTableName, condition);
            
            return dt;
        }

    }

    public class ItemData : INotifyPropertyChanged
    {
        private string _items;
        private string _maxOKQty;
        private string _rangeMin;
        private string _rangeMax;

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

        // Boilerplate INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
