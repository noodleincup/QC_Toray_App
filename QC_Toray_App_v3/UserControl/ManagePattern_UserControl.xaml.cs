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
    public partial class ManagePattern_UserControl : System.Windows.Controls.UserControl
    {
        public event EventHandler<string> ChangePageRequested;
        public ManagePattern_UserControl()
        {
            InitializeComponent();
        }
    }
}
