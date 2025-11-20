using QC_Toray_App_v3.Element;
using System;
using System.Collections.Generic;
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
        public SetSampleType_UserControl()
        {
            InitializeComponent();
            LoadSampleType();
        }

        private void LoadSampleType()
        {
            for (int i = 0; i < 3; i++)
            {
                SampleTypeElement sampleType = new SampleTypeElement();
                sampleType.txtSampleName.Text = "Sample Type " + (i + 1);
                sampleType.txtUpdateDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                sampleType.txtUpdateBy.Text = "Admin";

                wrpSampleTypeList.Children.Add(sampleType);
            }
        }
    }
}
