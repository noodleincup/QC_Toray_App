using HandleDatabase;
using System.Data;
using System.IO;
using System.Data.SqlClient;

namespace dummy_Test_DB_console_app
{


    internal class Program
    {
        private static string conn1 = @"Server=192.168.0.123\SQLEXPRESS; Database=test_app_db; User Id=sa; Password=1234;TrustServerCertificate=True;";
        private static string conn2 = "Server=192.168.0.123\\SQLEXPRESS; Database=camera_inspection; User Id=sa; Password=1234;TrustServerCertificate=True;";


        private const string LOT_OVERVIEW_TABLE = "Lot_Overview_Table"; 
        private const string MASTER_GRADE_PATTERN_TABLE = "tb_master_grade_pattern";
        private const string MASTER_PATTERN_TABLE = "tb_master_pattern";
        private const string SAMPLE_GROUP_TABLE = "tb_master_SamepleGroupName";

        private static DataTable lotOverviewDataTable;
        private static DataTable gradePatternDataTable;
        private static DataTable masterPatternDataTable;

        private static DatabaseHandler testAppHandler;
        private static DatabaseHandler camInspectHandler;


        static void Main(string[] args)
        {
            Console.WriteLine("Start Program");

            testAppHandler = new DatabaseHandler(conn1);
            camInspectHandler = new DatabaseHandler(conn2);

            // Load DataTables
            //lotOverviewDataTable = testAppHandler.GetTableDatabaseAsDataTable(LOT_OVERVIEW_TABLE);
            //gradePatternDataTable = camInspectHandler.GetTableDatabaseAsDataTable(MASTER_GRADE_PATTERN_TABLE);
            masterPatternDataTable = camInspectHandler.GetTableDatabaseAsDataTable(SAMPLE_GROUP_TABLE);

            //Console.WriteLine((lotOverviewDataTable is null && gradePatternDataTable is null && masterPatternDataTable is null)? "DataTables loaded fail." : "DataTables loaded successfully.");

            //testAppHandler.DisplayHeader(lotOverviewDataTable);
            //camInspectHandler.DisplayHeader(gradePatternDataTable);
            camInspectHandler.DisplayHeader(masterPatternDataTable);
            camInspectHandler.ShowDataTable(masterPatternDataTable);
        }
    }
}
