//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace SampleClassification.Model
{
    public class ModelInput
    {
        [ColumnName("ThisDiv"), LoadColumn(0)]
        public string ThisDiv { get; set; }


        [ColumnName("Date"), LoadColumn(1)]
        public string Date { get; set; }


        [ColumnName("HomeTeam"), LoadColumn(2)]
        public string HomeTeam { get; set; }


        [ColumnName("Hwpg"), LoadColumn(3)]
        public float Hwpg { get; set; }


        [ColumnName("Hdpg"), LoadColumn(4)]
        public float Hdpg { get; set; }


        [ColumnName("Hlpg"), LoadColumn(5)]
        public float Hlpg { get; set; }


        [ColumnName("Hgspg"), LoadColumn(6)]
        public float Hgspg { get; set; }


        [ColumnName("Hgcpg"), LoadColumn(7)]
        public float Hgcpg { get; set; }


        [ColumnName("Hsfpg"), LoadColumn(8)]
        public float Hsfpg { get; set; }


        [ColumnName("Hsapg"), LoadColumn(9)]
        public float Hsapg { get; set; }


        [ColumnName("Hstfpg"), LoadColumn(10)]
        public float Hstfpg { get; set; }


        [ColumnName("Hstapg"), LoadColumn(11)]
        public float Hstapg { get; set; }


        [ColumnName("AwayTeam"), LoadColumn(12)]
        public string AwayTeam { get; set; }


        [ColumnName("Awpg"), LoadColumn(13)]
        public float Awpg { get; set; }


        [ColumnName("Adpg"), LoadColumn(14)]
        public float Adpg { get; set; }


        [ColumnName("Alpg"), LoadColumn(15)]
        public float Alpg { get; set; }


        [ColumnName("Agspg"), LoadColumn(16)]
        public float Agspg { get; set; }


        [ColumnName("Agcpg"), LoadColumn(17)]
        public float Agcpg { get; set; }


        [ColumnName("Asfpg"), LoadColumn(18)]
        public float Asfpg { get; set; }


        [ColumnName("Asapg"), LoadColumn(19)]
        public float Asapg { get; set; }


        [ColumnName("Astfpg"), LoadColumn(20)]
        public float Astfpg { get; set; }


        [ColumnName("Astapg"), LoadColumn(21)]
        public float Astapg { get; set; }


        [ColumnName("RowValid"), LoadColumn(22)]
        public bool RowValid { get; set; }


        [ColumnName("FTR"), LoadColumn(23)]
        public string FTR { get; set; }


        [ColumnName("Over"), LoadColumn(24)]
        public bool Over { get; set; }


    }
}
