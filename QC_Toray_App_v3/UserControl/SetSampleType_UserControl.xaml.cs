using HandleDatabase;
using QC_Toray_App_v3.Element;
using QC_Toray_App_v3.library;
using QC_Toray_App_v3.Windows;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ManagePattern_UserControl.xaml
    /// </summary>
    public partial class SetSampleType_UserControl : System.Windows.Controls.UserControl
    {

        private DatabaseHandler databaseHandler = new DatabaseHandler(DatabaseConfig.ConnectionString1);

        public SetSampleType_UserControl()
        {
            InitializeComponent();
            //LoadSampleType();

            this.Loaded += SetSampleType_UserControl_Loaded;
        }

        private async void SetSampleType_UserControl_Loaded(object? sender, RoutedEventArgs e)
        {
            //this.Loaded -= SetSampleType_UserControl_Loaded; // detach if run only once
            try
            {
                await LoadSampleType();
            }
            catch (Exception ex)
            {
                // Surface any exceptions so you can see what went wrong
                MessageBox.Show(ex.ToString(), "Initialization error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadSampleType()
        {
            DataTable dt = databaseHandler.GetTableDatabaseAsDataTable(DatabaseConfig.SampleGroupTableName);

            List<string> headerNameList = databaseHandler.HeaderList(dt);

            databaseHandler.DisplayHeader(dt);

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    SampleTypeElement sampleTypeElement = new SampleTypeElement(databaseHandler);
                    sampleTypeElement.txtSampleName.Text = row[headerNameList[3]].ToString();
                    sampleTypeElement.txtUpdateBy.Text = row[headerNameList[2]].ToString();
                    sampleTypeElement.txtUpdateDate.Text = Convert.ToDateTime(row[headerNameList[1]]).ToString("yyyy-MM-dd HH:mm:ss");
                    sampleTypeElement.SampleId = Convert.ToInt32(row[headerNameList[0]]);

                    wrpSampleTypeList.Children.Add(sampleTypeElement);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sample types: " + ex.Message);
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            MasterSampleLimit_Window masterSampleLimitWindow = new MasterSampleLimit_Window(databaseHandler, false);
            bool result = masterSampleLimitWindow.ShowDialog() ?? false;
        }
    }
}
