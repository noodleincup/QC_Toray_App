using HandleDatabase;
using QC_Toray_App_v3.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace QC_Toray_App_v3.Element
{
    /// <summary>
    /// Interaction logic for SampleTypeElement.xaml
    /// </summary>
    public partial class SampleTypeElement : System.Windows.Controls.UserControl
    {
        private DatabaseHandler databaseHandler;

        private int _sampleId;
        public int SampleId
        {
            get { return _sampleId; }
            set { _sampleId = value; }
        }



        public SampleTypeElement(DatabaseHandler handler)
        {
            InitializeComponent();

            databaseHandler = handler;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Edit Sample Type clicked");
            MasterSampleLimit_Window masterSampleLimitWindow = new MasterSampleLimit_Window(databaseHandler, SampleId);

            masterSampleLimitWindow.txbSampleName.Text = txtSampleName.Text;

            bool result = masterSampleLimitWindow.ShowDialog() ?? false;
        }
    }
}
