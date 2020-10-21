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


        [ColumnName("Hgspst"), LoadColumn(8)]
        public float Hgspst { get; set; }


        [ColumnName("Hgcpst"), LoadColumn(9)]
        public float Hgcpst { get; set; }


        [ColumnName("Hgsps"), LoadColumn(10)]
        public float Hgsps { get; set; }


        [ColumnName("Hgcps"), LoadColumn(11)]
        public float Hgcps { get; set; }


        [ColumnName("Hstfpg"), LoadColumn(12)]
        public float Hstfpg { get; set; }


        [ColumnName("Hstapg"), LoadColumn(13)]
        public float Hstapg { get; set; }


        [ColumnName("Hstfps"), LoadColumn(14)]
        public float Hstfps { get; set; }


        [ColumnName("Hstaps"), LoadColumn(15)]
        public float Hstaps { get; set; }


        [ColumnName("Hsfpg"), LoadColumn(16)]
        public float Hsfpg { get; set; }


        [ColumnName("Hsapg"), LoadColumn(17)]
        public float Hsapg { get; set; }


        [ColumnName("AwayTeam"), LoadColumn(18)]
        public string AwayTeam { get; set; }


        [ColumnName("Awpg"), LoadColumn(19)]
        public float Awpg { get; set; }


        [ColumnName("Adpg"), LoadColumn(20)]
        public float Adpg { get; set; }


        [ColumnName("Alpg"), LoadColumn(21)]
        public float Alpg { get; set; }


        [ColumnName("Agspg"), LoadColumn(22)]
        public float Agspg { get; set; }


        [ColumnName("Agcpg"), LoadColumn(23)]
        public float Agcpg { get; set; }


        [ColumnName("Agspst"), LoadColumn(24)]
        public float Agspst { get; set; }


        [ColumnName("Agcpst"), LoadColumn(25)]
        public float Agcpst { get; set; }


        [ColumnName("Agsps"), LoadColumn(26)]
        public float Agsps { get; set; }


        [ColumnName("Agcps"), LoadColumn(27)]
        public float Agcps { get; set; }


        [ColumnName("Astfpg"), LoadColumn(28)]
        public float Astfpg { get; set; }


        [ColumnName("Astapg"), LoadColumn(29)]
        public float Astapg { get; set; }


        [ColumnName("Astfps"), LoadColumn(30)]
        public float Astfps { get; set; }


        [ColumnName("Astaps"), LoadColumn(31)]
        public float Astaps { get; set; }


        [ColumnName("Asfpg"), LoadColumn(32)]
        public float Asfpg { get; set; }


        [ColumnName("Asapg"), LoadColumn(33)]
        public float Asapg { get; set; }


        [ColumnName("RowValid"), LoadColumn(34)]
        public bool RowValid { get; set; }


        [ColumnName("FTR"), LoadColumn(35)]
        public string FTR { get; set; }


        [ColumnName("Over"), LoadColumn(36)]
        public string Over { get; set; }


    }
}
