using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QC_Toray_App_v3
{
    // Class for get connection string from program setting
    public static class DatabaseConfig
    {
        public static string ConnectionString
        {
            get => Properties.Settings.Default.DatabaseConnectionString;
            set
            {
                Properties.Settings.Default.DatabaseConnectionString = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
