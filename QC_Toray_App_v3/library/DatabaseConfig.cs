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
        #region Database Connection Strings
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
        #endregion

        #region Table Names
        public static string LotOverviewTableName
        {
            get => Properties.Settings.Default.LotOverviewTableName;
            set
            {
                Properties.Settings.Default.LotOverviewTableName = value;
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

        public static string MasterSampleItemTableName
        {
            get => Properties.Settings.Default.MasterSampleItemTable;
            set
            {
                Properties.Settings.Default.MasterSampleItemTable = value;
                Properties.Settings.Default.Save();
            }
        }
        public static string MasterPatternTableName
        {
            get => Properties.Settings.Default.MasterPatternTable;
            set
            {
                Properties.Settings.Default.MasterPatternTable = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string MasterGradePatternTableName
        {
            get => Properties.Settings.Default.MasterGradePatternTable;
            set
            {
                Properties.Settings.Default.MasterGradePatternTable = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string MasterDiameterTableName 
        {
            get => Properties.Settings.Default.MasterDiameterTable;
            set 
            { 
                Properties.Settings.Default.MasterDiameterTable = value;
                Properties.Settings.Default.Save();
            }
        }
        #endregion


        #region Stored Procedures Names
        public static string AddLotStoredProcedure
        {
            get => Properties.Settings.Default.AddLotStoredProcedure;
            set
            {
                Properties.Settings.Default.AddLotStoredProcedure = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string UpdateOrInsertMasterSampleItemProcedure
        {
            get => Properties.Settings.Default.UpdateOrInsertMasterSampleLimitProcedure;
            set
            {
                Properties.Settings.Default.UpdateOrInsertMasterSampleLimitProcedure = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string InserOrUpdateMasterPatternProcedure
        {
            get => Properties.Settings.Default.InserOrUpdateMasterPatternProcedure;
            set
            {
                Properties.Settings.Default.InserOrUpdateMasterPatternProcedure = value;
                Properties.Settings.Default.Save();
            }
        }
        #endregion

        private static int _timeoutMs = 2000;
        public static int TimeoutMs
        {
            get => _timeoutMs;
        }
    }
}
