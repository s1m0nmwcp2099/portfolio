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
            string sqlCt = "SELECT COUNT(*) FROM football_data_complete WHERE FTHG IS NOT NULL AND FTAG IS NOT NULL AND FTR IS NOT NULL AND HTHG IS NOT NULL AND HTAG IS NOT NULL AND HTR IS NOT NULL AND HS IS NOT NULL AND AwS IS NOT NULL AND HST IS NOT NULL AND AwST IS NOT NULL AND HC IS NOT NULL AND AC IS NOT NULL";
            int ct = 0;
            using (MySqlConnection conn = new MySqlConnection(connStr)){
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlCt, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read()){
                    ct = rdr.GetInt32(0);
                }
                conn.Close();
            }
            Console.WriteLine($"{ct} lines");
            
            //ThisDiv
            //List<string> _ThisDiv = new List<string>(); //will I need all these lists or go direct to dfColumns?
            StringDataFrameColumn thisDiv = new StringDataFrameColumn("ThisDiv", ct);

            //Date
            //List<DateTime> _Date = new List<DateTime>();
            PrimitiveDataFrameColumn<DateTime> date = new PrimitiveDataFrameColumn<DateTime>("Date", ct);

            //HomeTeam
            //List<string> _HomeTeam = new List<string>();
            StringDataFrameColumn homeTeam = new StringDataFrameColumn("HomeTeam", ct);

            //AwayTeam
            //List<string> _AwayTeam = new List<string>();
            StringDataFrameColumn awayTeam = new StringDataFrameColumn("AwayTeam", ct);

            //FTHG full time home goals
            //List<int> _FTHG = new List<int>();
            PrimitiveDataFrameColumn<int> fthg = new PrimitiveDataFrameColumn<int>("FTHG", ct);

            //FTAG ...away
            //List<int> _FTAG = new List<int>();
            PrimitiveDataFrameColumn<int> ftag = new PrimitiveDataFrameColumn<int>("FTAG", ct);

            //FTR ...result
            //List<string> _FTR = new List<string>();
            StringDataFrameColumn ftr = new StringDataFrameColumn("FTR", ct);

            //HTHG half time home goals
            //List<int> _HTHG = new List<int>();
            PrimitiveDataFrameColumn<int> hthg = new PrimitiveDataFrameColumn<int>("HTHG", ct);

            //HTAG
            //List<int> _HTAG = new List<int>();
            PrimitiveDataFrameColumn<int> htag = new PrimitiveDataFrameColumn<int>("ATAG", ct);

            //HTR
            //List<string> _HTR = new List<string>();
            StringDataFrameColumn htr = new StringDataFrameColumn("HTR", ct);

            //HS shots
            //List<int> _HS = new List<int>();
            PrimitiveDataFrameColumn<int> hs = new PrimitiveDataFrameColumn<int>("HS", ct);

            //AwS
            //List<int> _AwS = new List<int>();
            PrimitiveDataFrameColumn<int> aws = new PrimitiveDataFrameColumn<int>("AwS", ct);

            //HST shots on target
            //List<int> _HST = new List<int>();
            PrimitiveDataFrameColumn<int> hst = new PrimitiveDataFrameColumn<int>("HST", ct);

            //AwST
            //List<int> _AwST = new List<int>();
            PrimitiveDataFrameColumn<int> awst = new PrimitiveDataFrameColumn<int>("AWST", ct);

            //HC corners
            //List<int> _HC = new List<int>();
            PrimitiveDataFrameColumn<int> hc = new PrimitiveDataFrameColumn<int>("HC", ct);

            //AC
            //List<int> _AC = new List<int>();
            PrimitiveDataFrameColumn<int> ac = new PrimitiveDataFrameColumn<int>("AC", ct);

            string sql = "SELECT * FROM football_data_complete WHERE FTHG IS NOT NULL AND FTAG IS NOT NULL AND FTR IS NOT NULL AND HTHG IS NOT NULL AND HTAG IS NOT NULL AND HTR IS NOT NULL AND HS IS NOT NULL AND AwS IS NOT NULL AND HST IS NOT NULL AND AwST IS NOT NULL AND HC IS NOT NULL AND AC IS NOT NULL";
            using (MySqlConnection conn = new MySqlConnection(connStr)){
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read()){

                    int i = rdr.GetOrdinal("ThisDiv");
                    thisDiv.Append(rdr.GetString(i));

                    i = rdr.GetOrdinal("Date");
                    date.Append(rdr.GetDateTime(i));

                    i = rdr.GetOrdinal("HomeTeam");
                    homeTeam.Append(rdr.GetString(i));

                    i = rdr.GetOrdinal("AwayTeam");
                    awayTeam.Append(rdr.GetString(i));

                    i = rdr.GetOrdinal("FTHG");
                    fthg.Append(rdr.GetInt32(i));

                    i = rdr.GetOrdinal("FTAG");
                    ftag.Append(rdr.GetInt32(i));

                    i = rdr.GetOrdinal("FTR");
                    ftr.Append(rdr.GetString(i));

                    i = rdr.GetOrdinal("HTHG");
                    hthg.Append(rdr.GetInt32(i));

                    i = rdr.GetOrdinal("HTAG");
                    htag.Append(rdr.GetInt32(i));

                    i = rdr.GetOrdinal("HTR");
                    htr.Append(rdr.GetString(i));

                    i = rdr.GetOrdinal("HS");
                    hs.Append(rdr.GetInt32(i));

                    i = rdr.GetOrdinal("AwS");
                    aws.Append(rdr.GetInt32(i));

                    i = rdr.GetOrdinal("HST");
                    hst.Append(rdr.GetInt32(i));

                    i = rdr.GetOrdinal("AwST");
                    awst.Append(rdr.GetInt32(i));

                    i = rdr.GetOrdinal("HC");
                    hc.Append(rdr.GetInt32(i));

                    i = rdr.GetOrdinal("AC");
                    ac.Append(rdr.GetInt32(i));
                }
                conn.Close();
            }
        }
    }
}
