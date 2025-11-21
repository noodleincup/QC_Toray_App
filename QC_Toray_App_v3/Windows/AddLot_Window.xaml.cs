using HandleDatabase;
using QC_Toray_App_v3.library;
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
using System.Windows.Shapes;

namespace QC_Toray_App_v3.Windows
{
    /// <summary>
    /// Interaction logic for AddLot_Window.xaml
    /// </summary>
    public partial class AddLot_Window : Window
    {

        private DatabaseHandler dbHandler;

        private string _result;
        public string Result
        {
            get { return _result; }
            private set { _result = value; }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            private set { _message = value; }
        }

        public AddLot_Window(DatabaseHandler handler)
        {
            InitializeComponent();

            dbHandler = handler;
        }

        private void btnAddLot_Click(object sender, RoutedEventArgs e)
        {

            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "@Lot", txtLotNumber.Text },
                { "@user", GlobalState.Instance.UserName }
            };

            DataSet ds = dbHandler.ExecuteStoredProcedure(DatabaseConfig.AddLotStoredProcedure, parameters);
            DataTable dataTableResult = ds.Tables[0];

            Console.WriteLine($"Amount table : {ds.Tables.Count}");
            Console.WriteLine($"Table result:");
            dbHandler.DisplayHeader(dataTableResult);
            dbHandler.ShowDataTable(dataTableResult);
            Console.WriteLine("-----");

            Result = dataTableResult.Rows[0]["result"].ToString();
            Message = dataTableResult.Rows[0]["message"].ToString();

            if (Result != "success")
            {
                MessageBox.Show(Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
