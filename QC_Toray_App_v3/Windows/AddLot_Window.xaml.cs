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
using System.Windows.Shapes;

namespace QC_Toray_App_v3.Windows
{
    /// <summary>
    /// Interaction logic for AddLot_Window.xaml
    /// </summary>
    public partial class AddLot_Window : Window
    {
        public AddLot_Window()
        {
            InitializeComponent();
        }

        private void btnAddLot_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
