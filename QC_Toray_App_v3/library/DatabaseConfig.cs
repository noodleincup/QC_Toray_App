using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QC_Toray_App_v3.library
{
    // Class for get connection string from program setting
    public static class DatabaseConfig
    {
        public static string ConnectionString1
        {
            get => Properties.Settings.Default.DatabaseConnectionString1;
            set
            {
                Properties.Settings.Default.DatabaseConnectionString1 = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string ConnectionString2
        {
            get => Properties.Settings.Default.DatabaseConnectionString2;
            set
            {
                Properties.Settings.Default.DatabaseConnectionString1 = value;
                Properties.Settings.Default.Save();
            }
        }



        public static string LotOverviewTableName
        {
            get => Properties.Settings.Default.LotOverviewTableName;
            set
            {
                Properties.Settings.Default.LotOverviewTableName = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
