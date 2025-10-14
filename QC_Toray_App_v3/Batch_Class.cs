using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QC_Toray_App_v3
{
    // Class for Mangaing Batch Item
    // Batch Object
    public class Batch
    {
        public string Lot { get; set; }
        public int Pallet_Size { get; set; }
        public int Batch_Number { get; set; }
        public int MissDB { get; set; }
        public int MissTP { get; set; }
        public int MissFf { get; set; }
        public int LinkDB { get; set; }
        public int LinkTP { get; set; }
        public int LinkFf { get; set; }
        public int LinkFF { get; set; }
        public int Defect { get; set; }
        public int GF1 { get; set; }
        public int GF2 { get; set; }
        public int GF3 { get; set; }
        public int Meya { get; set; }
        public int Meya_NoChg { get; set; }
        public int ForeignMat { get; set; }
        public int BlackSpot_SS { get; set; }
        public int BlackSpot_S { get; set; }
        public int BlackSpot_M { get; set; }
        public int BlackSpot_L { get; set; }
        public int ColorAbnomal { get; set; }
        public int Macaroni_SS { get; set; }
        public int Macaroni_S { get; set; }
        public int Macaroni_M { get; set; }
        public int Macaroni_L { get; set; }
        public string Remark { get; set; }
        public int Judgement { get; set; }
        public string PIC { get; set; }

        public DataTable ConvertBatchToDataTable()
        {
            DataTable dt = new DataTable();

            // Add columns (same as Batch properties)
            dt.Columns.Add("Lot", typeof(string));
            dt.Columns.Add("Batch_Num", typeof(int));
            dt.Columns.Add("Pallet_Size", typeof(int));
            dt.Columns.Add("MissDB", typeof(int));
            dt.Columns.Add("MissTp", typeof(int));
            dt.Columns.Add("MissFf", typeof(int));
            dt.Columns.Add("LinkDb", typeof(int));
            dt.Columns.Add("LinkTp", typeof(int));
            dt.Columns.Add("LinkFf", typeof(int));
            dt.Columns.Add("LinkFif", typeof(int));
            dt.Columns.Add("Defect", typeof(int));
            dt.Columns.Add("GF1", typeof(int));
            dt.Columns.Add("GF2", typeof(int));
            dt.Columns.Add("GF3", typeof(int));
            dt.Columns.Add("Meya", typeof(int));
            dt.Columns.Add("Meya_NoChg", typeof(int));
            dt.Columns.Add("ForeignMat", typeof(int));
            dt.Columns.Add("BlackSpot_SS", typeof(int));
            dt.Columns.Add("BlackSpot_S", typeof(int));
            dt.Columns.Add("BlackSpot_M", typeof(int));
            dt.Columns.Add("BlackSpot_L", typeof(int));
            dt.Columns.Add("ColorAbnomal", typeof(int));
            dt.Columns.Add("Macaroni_SS", typeof(int));
            dt.Columns.Add("Macaroni_S", typeof(int));
            dt.Columns.Add("Macaroni_M", typeof(int));
            dt.Columns.Add("Macaroni_L", typeof(int));
            dt.Columns.Add("Judgment", typeof(int));
            dt.Columns.Add("Remark", typeof(string));
            dt.Columns.Add("PIC", typeof(string));

            // Add row with values from Batch object
            dt.Rows.Add(
               Lot,
               Batch_Number,
               Pallet_Size,
               MissDB,
               MissTP,
               MissFf,
               LinkDB,
               LinkTP,
               LinkFf,
               LinkFF,
               Defect,
               GF1,
               GF2,
               GF3,
               Meya,
               Meya_NoChg,
               ForeignMat,
               BlackSpot_SS,
               BlackSpot_S,
               BlackSpot_M,
               BlackSpot_L,
               ColorAbnomal,
               Macaroni_SS,
               Macaroni_S,
               Macaroni_M,
               Macaroni_L,
               Judgement,
               Remark,
               PIC
            );

            return dt;
        }

        // Get data from DataTable throught SQL server
        public void GetFromDataTable(DataRow row)
        {
            Lot = row["Lot"].ToString();
            Batch_Number = Convert.ToInt32(row["Batch_Num"]);
            Pallet_Size = Convert.ToInt32(row["Pallet_Size"]);
            MissDB = Convert.ToInt32(row["MissDb"]);
            MissTP = Convert.ToInt32(row["MissTp"]);
            MissFf = Convert.ToInt32(row["MissFf"]);
            LinkDB = Convert.ToInt32(row["LinkDb"]);
            LinkTP = Convert.ToInt32(row["LinkTp"]);
            LinkFf = Convert.ToInt32(row["LinkFf"]);
            LinkFF = Convert.ToInt32(row["LinkFF"]);
            Defect = Convert.ToInt32(row["Defect"]);
            GF1 = Convert.ToInt32(row["GF1"]);
            GF2 = Convert.ToInt32(row["GF2"]);
            GF3 = Convert.ToInt32(row["GF3"]);
            Meya = Convert.ToInt32(row["Meya"]);
            Meya_NoChg = Convert.ToInt32(row["Meya"]);
            ForeignMat = Convert.ToInt32(row["ForeignMat"]);
            BlackSpot_SS = Convert.ToInt32(row["BlackSpot_SS"]);
            BlackSpot_S = Convert.ToInt32(row["BlackSpot_S"]);
            BlackSpot_M = Convert.ToInt32(row["BlackSpot_M"]);
            BlackSpot_L = Convert.ToInt32(row["BlackSpot_L"]);
            ColorAbnomal = Convert.ToInt32(row["ColorAbnomal"]);
            Macaroni_SS = Convert.ToInt32(row["Macaroni_SS"]);
            Macaroni_S = Convert.ToInt32(row["Macaroni_S"]);
            Macaroni_M = Convert.ToInt32(row["Macaroni_M"]);
            Macaroni_L = Convert.ToInt32(row["Macaroni_L"]);
            Remark = row["Remark"].ToString();
            Judgement = Convert.ToInt32(row["Judgement"]);
            PIC = row["PIC"].ToString();

        }
    }
}
