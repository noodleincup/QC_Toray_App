using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Data.SqlClient;
using ControlzEx.Standard;
using QC_Toray_App_v3.library;

namespace QC_Toray_App_v3
{
    // Define an event
    //public event EventHandler<string> DataSent;
    public partial class LoginUserControl : System.Windows.Controls.UserControl
    {
        public event EventHandler<string> ChangePageRequested;
        public event EventHandler<string> ChangeUserName;

        private const string AUTHENTICATION_SP = "usp_authenticate_user";

        // Create DatabaseHandler
        DatabaseHandler databaseHandler = new DatabaseHandler(DatabaseConfig.ConnectionString1);

        public LoginUserControl()
        {
            InitializeComponent();
        }

        private void Login_Button_Clicked(object sender, RoutedEventArgs e)
        {
            string user = txbUser.Text;
            string pass = txbPassword.Password;
            // Chenging for debuggin
            try
            {
                bool result = true; // VerifyLogin(user, pass);
                string display = result ? "Correct" : "Username or Password is failure";
                if (result)
                {
                    GlobalState.Instance.IsFeatureEnabled = true;
                    GlobalState.Instance.UserName = user;
                    // Raise the event and pass the desired ListViewItem name
                    ChangePageRequested?.Invoke(this, "MainTable"); // Example: Navigate to Operating page
                }
                else { MessageBox.Show(display); }
            }
            catch (Exception ex) { 
                MessageBox.Show(ex.ToString());
            }
        }
        private void Enter_Pressed(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    string user = txbUser.Text;
                    string pass = txbPassword.Password;
                    LoginProcess(user, pass);
                }
            }
            catch (Exception ex){ 
                MessageBox.Show(ex.ToString());
            }
        }

        private void LoginProcess(string username, string password)
        {
            bool result = VerifyLogin(username, password);
            string display = result ? "Correct" : "Username or Password is failure";

            if (result)
            {
                GlobalState.Instance.IsFeatureEnabled = true;
                GlobalState.Instance.UserName = username;
                // Raise the event and pass the desired ListViewItem name
                ChangeUserName?.Invoke(this, username);
                ChangePageRequested?.Invoke(this, "MainTable"); // Example: Navigate to Operating page
            }
            else { MessageBox.Show(display); }
        }

        private bool VerifyLogin(string user, string pass)
        {
            // Get Connection String for connecting database
            string conn = DatabaseConfig.ConnectionString1;

            //Debugging
            //MessageBox.Show(conn);

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@username", user },
                { "@password", pass  }
            };

            DataSet result = databaseHandler.ExecuteStoredProcedure(AUTHENTICATION_SP, parameters, true, "@IsValidUser");


            // Check index is null
            if (result.Tables.Count == 0) {
                return false;
            }



            return true;

        }

        // Generate a hashed password with salt
        public string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        // Verify password
        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // hashBytes = salt byte [0:15] + passwordHash Byte [16:36] from verify password
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            // get salt byte
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // generate hash from input password
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            // get byte input password
            byte[] hash = pbkdf2.GetBytes(20);

            // Verify byte per byte between Input password bytes and Stored password bytes
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }
    }
}
