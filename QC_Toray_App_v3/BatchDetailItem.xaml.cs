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

namespace QC_Toray_App_v3
{
    /// <summary>
    /// Interaction logic for BatchDetailItem.xaml
    /// </summary>
    public partial class BatchDetailItem : System.Windows.Controls.UserControl
    {
        public event EventHandler<string> ChangePageRequested;

        public BatchDetailItem()
        {
            InitializeComponent();
        }
        // Public Property to get/set the value from the TextBox
        public string ItemValue
        {
            get { return ValueTextBox.Text; }
            set { ValueTextBox.Text = value; }
        }

        // Public Property to get/set the status from the Status TextBox
        public string Status
        {
            get { return StatusTextBox.Text; }
            set { StatusTextBox.Text = value; }
        }

        // Public Property to get/set the CheckBox state
        public bool? IsItemSelected
        {
            get { return ItemCheckBox.IsChecked; }
            set { ItemCheckBox.IsChecked = value; }
        }

        private void txbBatchDetail_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show($"Batch Detail {ItemValue}");
            MainWindow.Instance.BatchNum = ItemValue;
            //MessageBox.Show($"From the second");
            // ChangePageRequested?.Invoke(this, "Operating");

            OperationUserControl operationControl = new OperationUserControl();

            bool result = operationControl.ShowDialog() ?? false;

            this.Status = result ? "OK" : "NG";
            this.ItemCheckBox.IsChecked = true;
        }
    }
}
