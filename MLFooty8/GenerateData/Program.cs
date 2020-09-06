using System;
using MySql.Data.MySqlClient;
using Microsoft.Data.Analysis;

namespace GenerateData
{
    class Program
    {
        static string connStr = "server = localhost; user = simon; database = football; port = 3306; password = chainsaw";
        static void Main(string[] args)
        {
            //GATHER HISTORICAL MATCH DATA FROM MYSQL TABLE FOR MATCHES THAT INCLUDE HALF TIME SCORES, SHOTS, SHOTS ON TARGET
            
            //ThisDiv
            List<string> _ThisDiv = new List<string>(); //will I need all these lists or go direct to dfColumns?
            StringDataFrameColumn thisDiv = new StringDataFrameColumn("ThisDiv");

            //Date
            List<DateTime> _Date = new List<DateTime>();
            PrimitiveDataFrameColumn<DateTime> date = new PrimitiveDataFrameColumn<DateTime>("Date");

            //HomeTeam
            List<string> _HomeTeam = new List<string>();
            StringDataFrameColumn homeTeam = new StringDataFrameColumn("HomeTeam");

            //AwayTeam
            List<string> _AwayTeam = new List<string>();
            StringDataFrameColumn awayTeam = new StringDataFrameColumn("AwayTeam");

            //FTHG full time home goals
            List<int> _FTHG = new List<int>();
            PrimitiveDataFrameColumn<int> fthg = new PrimitiveDataFrameColumn<int>("FTHG");

            //FTAG ...away
            List<int> _FTAG = new List<int>();
            PrimitiveDataFrameColumn<int> athg = new PrimitiveDataFrameColumn<int>("ATHG");

            //FTR ...result
            List<string> _FTR = new List<string>();
            StringDataFrameColumn ftr = new StringDataFrameColumn("FTR");

            //HTHG half time home goals
            List<int> _HTHG = new List<int>();
            PrimitiveDataFrameColumn<int> hthg = new PrimitiveDataFrameColumn<int>("HTHG");

            //HTAG
            List<int> _HTAG = new List<int>();
            PrimitiveDataFrameColumn<int> htag = new PrimitiveDataFrameColumn<int>("ATAG");

            //HTR
            List<string> _HTR = new List<string>();
            StringDataFrameColumn htr = new StringDataFrameColumn("HTR");

            //HS shots
            List<int> _HS = new List<int>();
            PrimitiveDataFrameColumn<int> hs = new PrimitiveDataFrameColumn<int>("HS");

            //AwS
            List<int> _AwS = new List<int>();
            PrimitiveDataFrameColumn<int> as = new PrimitiveDataFrameColumn<int>("AS");

            //HST shots on target
            List<int> _HST = new List<int>();
            PrimitiveDataFrameColumn<int> hst = new PrimitiveDataFrameColumn<int>("HST");

            //AwST
            List<int> _AwST = new List<int>();
            PrimitiveDataFrameColumn<int> awst = new PrimitiveDataFrameColumn<int>("AWST");

            //HC corners
            List<int> _HC = new List<int>();
            PrimitiveDataFrameColumn<int> hc = new PrimitiveDataFrameColumn<int>("HC");

            //AC
            List<int> _AC = new List<int>();
            PrimitiveDataFrameColumn<int> ac = new PrimitiveDataFrameColumn<int>("AC");

            string sql = "SELECT * FROM football_data_complete WHERE FTHG IS NOT NULL AND FTAG IS NOT NULL AND FTR IS NOT NULL AND HTHG IS NOT NULL AND HTAG IS NOT NULL AND HTR IS NOT NULL AND HS IS NOT NULL AND AwS IS NOT NULL AND HST IS NOT NULL AND AwST IS NOT NULL AND HC IS NOT NULL AND AC IS NOT NULL";
            using (MySqlConnection conn = new MySqlConnection(connStr)){
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read()){
                    //rdr.GetOrdinal("Date");
                    rdr.GetInt32
                }
            }
        }
    }
}
