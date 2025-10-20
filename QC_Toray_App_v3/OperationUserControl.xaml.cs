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
using HandleDatabase;
using System.Globalization;
using System.Windows.Data;
using System.Data;
using System.Text.RegularExpressions;

namespace QC_Toray_App_v3
{
    /// <summary>
    /// Interação lógica para UserControlCreate.xam
    /// </summary>
    public partial class OperationUserControl : System.Windows.Controls.UserControl
    {
        private string PIC = "Mr. Donald";
        private int batch_no = 15;
        public OperationUserControl()
        {
            InitializeComponent();

            List<Batch> batchs = new List<Batch>();
            batchs.Add(new Batch() { Batch_Number = 1, Judgement = 1 });
            batchs.Add(new Batch() { Batch_Number = 2, Judgement = 1 });
            batchs.Add(new Batch() { Batch_Number = 3, Judgement = 0 });
            batchs.Add(new Batch() { Batch_Number = 4, Judgement = 1 });

            dgBatchHis.ItemsSource = batchs;

            //Display_Data();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                MessageBox.Show("Spacebar Pressed!");
                e.Handled = true; // Prevents further propagation of the event

                


            }
        }

        // Keyboard Click
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                MessageBox.Show("Spacebar Pressed in PreviewKeyDown!");

                e.Handled = true; // Stops further event bubbling
            }
            else if (e.Key == Key.Enter) {
                MessageBox.Show("Enter Pressed in PreviewKeyDown!");

                // Get data and OK Trick
                Batch batch = PrepareData(true);

                // Insert data
                bool success = true; // Insert_Data(batch);
                if (success) { MessageBox.Show("Insert Success"); } else { MessageBox.Show("Insert Success"); }

                e.Handled = true; // Stops further event bubbling
            }
        }

        // Get the data from SQL to datagrid on interface
        private void Display_Data()
        {
            // Get Connection String for connecting database
            string conn = DatabaseConfig.ConnectionString;

            // Define Users Database Object
            ItemsHandle itemsDb = new ItemsHandle(conn);

            // Get Item Data
            DataTable items = itemsDb.GetItems();

            // Prepare List for displaying
            List<Batch> lists = new List<Batch>();

            // Insert data to list
            for(int i = 0; i < items.Rows.Count; i++)
            {
                Batch batch = new Batch();
                batch.GetFromDataTable(items.Rows[i]);
                lists.Add(batch);
            }

            // Add data to DataGrid
            dgBatchHis.ItemsSource = lists;

        }


        // Insert from data table to sql server 
        private bool Insert_Data(Batch batch)
        {
            // Get Connection String for connecting database
            string conn = DatabaseConfig.ConnectionString;

            // Define Users Database Object
            ItemsHandle itemsDb = new ItemsHandle(conn);

            // Prepare data table for inserting
            DataTable dt = batch.ConvertBatchToDataTable();

            try
            {
                // Insert data
                bool result = itemsDb.InsertItems(dt);
                return result;
            }
            catch (Exception ex) { 
                MessageBox.Show(ex.ToString());
                return false;
            }
        }


        // Insert data on text box to batch object
        private Batch PrepareData(bool judge)
        {
            Batch insertBatch = new Batch();
            insertBatch.Lot = txbLot.Text;
            insertBatch.Batch_Number = batch_no;
            insertBatch.Pallet_Size = tgPalletSize.IsChecked == true ? 1 : 0;
            insertBatch.MissDB = Convert.ToInt32(txbMissDb.Text);
            insertBatch.MissTP = Convert.ToInt32(txbMissTp.Text);
            insertBatch.MissFf = Convert.ToInt32(txbMissFf.Text);
            insertBatch.LinkDB = Convert.ToInt32(txbLinkDb.Text);
            insertBatch.LinkTP = Convert.ToInt32(txbLinkTp.Text);
            insertBatch.LinkFf = Convert.ToInt32(txbLinkFf.Text);
            insertBatch.LinkFF = Convert.ToInt32(txbLinkFF.Text);
            insertBatch.Defect = Convert.ToInt32(txbDefect.Text);
            insertBatch.GF1 = Convert.ToInt32(txbGF1.Text);
            insertBatch.GF2 = Convert.ToInt32(txbGF2.Text);
            insertBatch.GF3 = Convert.ToInt32(txbGF3.Text);
            insertBatch.Meya = Convert.ToInt32(txbMeya.Text);
            insertBatch.Meya_NoChg = Convert.ToInt32(txbMeya_NoChg.Text);
            insertBatch.ForeignMat = Convert.ToInt32(txbForeignMat.Text);
            insertBatch.BlackSpot_SS = Convert.ToInt32(txbBlackSpot_SS.Text);
            insertBatch.BlackSpot_S = Convert.ToInt32(txbBlackSpot_S.Text);
            insertBatch.BlackSpot_M = Convert.ToInt32(txbBlackSpot_M.Text);
            insertBatch.BlackSpot_L = Convert.ToInt32(txbBlackSpot_L.Text);
            insertBatch.ColorAbnomal = Convert.ToInt32(txbColorAbnormal.Text);
            insertBatch.Macaroni_SS = Convert.ToInt32(txbMacaroni_SS.Text);
            insertBatch.Macaroni_S = Convert.ToInt32(txbMacaroni_S.Text);
            insertBatch.Macaroni_M = Convert.ToInt32(txbMacaroni_M.Text);
            insertBatch.Macaroni_L = Convert.ToInt32(txbMacaroni_L.Text);
            insertBatch.Remark = txbRemark.Text;
            insertBatch.Judgement = judge ? 1:0;
            insertBatch.PIC = PIC;
            
            return insertBatch;
        } 
    }

    // Bliding Data to Interface
    public class JudgementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int judgement)
            {
                return judgement == 1 ? "OK" : "NG";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string judgementStr)
            {
                return judgementStr == "OK" ? 1 : 0;
            }
            return 0;
        }
    }
}
