//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace SampleMulticlassClassification.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("league"), LoadColumn(0)]
        public string League { get; set; }


        [ColumnName("date"), LoadColumn(1)]
        public string Date { get; set; }


        [ColumnName("home_team"), LoadColumn(2)]
        public string Home_team { get; set; }


        [ColumnName("away_team"), LoadColumn(3)]
        public string Away_team { get; set; }


        [ColumnName("hm_odds"), LoadColumn(4)]
        public float Hm_odds { get; set; }


        [ColumnName("dr_odds"), LoadColumn(5)]
        public float Dr_odds { get; set; }


        [ColumnName("aw_odds"), LoadColumn(6)]
        public float Aw_odds { get; set; }


        [ColumnName("hmGfAtHm"), LoadColumn(7)]
        public float HmGfAtHm { get; set; }


        [ColumnName("hmGagAtHm"), LoadColumn(8)]
        public float HmGagAtHm { get; set; }


        [ColumnName("hmWatHm"), LoadColumn(9)]
        public float HmWatHm { get; set; }


        [ColumnName("hmDrAtHm"), LoadColumn(10)]
        public float HmDrAtHm { get; set; }


        [ColumnName("hmLosAtHm"), LoadColumn(11)]
        public float HmLosAtHm { get; set; }


        [ColumnName("awGfAtAw"), LoadColumn(12)]
        public float AwGfAtAw { get; set; }


        [ColumnName("awGagAtAw"), LoadColumn(13)]
        public float AwGagAtAw { get; set; }


        [ColumnName("awWatAw"), LoadColumn(14)]
        public float AwWatAw { get; set; }


        [ColumnName("awDrAtAw"), LoadColumn(15)]
        public float AwDrAtAw { get; set; }


        [ColumnName("awLosAtAw"), LoadColumn(16)]
        public float AwLosAtAw { get; set; }


        [ColumnName("ftr"), LoadColumn(17)]
        public string Ftr { get; set; }


    }
}
