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

        public static string AddLotStoredProcedure
        {
            get => Properties.Settings.Default.AddLotStoredProcedure;
            set
            {
                Properties.Settings.Default.AddLotStoredProcedure = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string SampleGroupTableName
        {
            get => Properties.Settings.Default.SampleGroupTable;
            set
            {
                Properties.Settings.Default.SampleGroupTable = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string MasterSampleLimitTableName
        {
            get => Properties.Settings.Default.MasterSampleLimitTable;
            set
            {
                Properties.Settings.Default.MasterSampleLimitTable = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
