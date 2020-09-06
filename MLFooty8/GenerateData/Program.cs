using System;
using MySql.Data.MySqlClient;
using Microsoft.Data.Analysis;
using System.Collections.Generic;

namespace GenerateData
{
    class Program
    {
        static string connStr = "server = localhost; user = simon; database = football; port = 3306; password = chainsaw";
        static void Main(string[] args)
        {
            //GATHER HISTORICAL MATCH DATA FROM MYSQL TABLE FOR MATCHES THAT INCLUDE HALF TIME SCORES, SHOTS, SHOTS ON TARGETIS NOT
            string sqlCt = "SELECT COUNT(*) FROM football_data_complete WHERE (FTHG IS NOT NULL AND FTAG IS NOT NULL AND FTR IS NOT NULL AND HTHG IS NOT NULL AND HTAG IS NOT NULL AND HTR IS NOT NULL AND HS IS NOT NULL AND AwS IS NOT NULL AND HST IS NOT NULL AND AwST IS NOT NULL AND HC IS NOT NULL AND AC IS NOT NULL AND FTHG <> 'null' AND FTAG <> 'null' AND FTR <> 'null' AND HTHG <> 'null' AND HTAG <> 'null' AND HTR <> 'null' AND HS <> 'null' AND AwS <> 'null' AND HST <> 'null' AND AwST <> 'null' AND HC <> 'null' AND AC <> 'null');";
            //string sqlCt = "SELECT COUNT(*) FROM football_data_complete WHERE (FTHG AND FTAG AND FTR AND HTHG AND HTAG AND HTR AND HS AND AwS AND HST AND AwST AND HC AND AC) IS NOT NULL";
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
            
            //COLUMNS FOR DATAFRAME
            StringDataFrameColumn thisDiv = new StringDataFrameColumn("ThisDiv", 0);
            PrimitiveDataFrameColumn<DateTime> date = new PrimitiveDataFrameColumn<DateTime>("Date", 0);
            StringDataFrameColumn homeTeam = new StringDataFrameColumn("HomeTeam", 0);
            StringDataFrameColumn awayTeam = new StringDataFrameColumn("AwayTeam", 0);
            PrimitiveDataFrameColumn<int> fthg = new PrimitiveDataFrameColumn<int>("FTHG", 0);
            PrimitiveDataFrameColumn<int> ftag = new PrimitiveDataFrameColumn<int>("FTAG", 0);
            StringDataFrameColumn ftr = new StringDataFrameColumn("FTR", 0);
            PrimitiveDataFrameColumn<int> hthg = new PrimitiveDataFrameColumn<int>("HTHG", 0);
            PrimitiveDataFrameColumn<int> htag = new PrimitiveDataFrameColumn<int>("HTAG", 0);
            StringDataFrameColumn htr = new StringDataFrameColumn("HTR", 0);
            PrimitiveDataFrameColumn<int> hs = new PrimitiveDataFrameColumn<int>("HS", 0);
            PrimitiveDataFrameColumn<int> aws = new PrimitiveDataFrameColumn<int>("AwS", 0);
            PrimitiveDataFrameColumn<int> hst = new PrimitiveDataFrameColumn<int>("HST", 0);
            PrimitiveDataFrameColumn<int> awst = new PrimitiveDataFrameColumn<int>("AwST", 0);
            PrimitiveDataFrameColumn<int> hc = new PrimitiveDataFrameColumn<int>("HC", 0);
            PrimitiveDataFrameColumn<int> ac = new PrimitiveDataFrameColumn<int>("AC", 0);

            string sql = "SELECT ThisDiv, Date, HomeTeam, AwayTeam, FTHG, FTAG, FTR, HTHG, HTAG, HTR, HS, AwS, HST, AwST, HC, AC  FROM football_data_complete WHERE (FTHG IS NOT NULL AND FTAG IS NOT NULL AND FTR IS NOT NULL AND HTHG IS NOT NULL AND HTAG IS NOT NULL AND HTR IS NOT NULL AND HS IS NOT NULL AND AwS IS NOT NULL AND HST IS NOT NULL AND AwST IS NOT NULL AND HC IS NOT NULL AND AC IS NOT NULL AND FTHG <> 'null' AND FTAG <> 'null' AND FTR <> 'null' AND HTHG <> 'null' AND HTAG <> 'null' AND HTR <> 'null' AND HS <> 'null' AND AwS <> 'null' AND HST <> 'null' AND AwST <> 'null' AND HC <> 'null' AND AC <> 'null');";
            //string sql = "SELECT * FROM football_data_complete WHERE (FTHG AND FTAG AND FTR AND HTHG AND HTAG AND HTR AND HS AND AwS AND HST AND AwST AND HC AND AC) IS NOT NULL";
            using (MySqlConnection conn = new MySqlConnection(connStr)){
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read()){
                    thisDiv.Append(rdr.GetString(0));
                    date.Append(rdr.GetDateTime(1));
                    homeTeam.Append(rdr.GetString(2));
                    awayTeam.Append(rdr.GetString(3));
                    fthg.Append(rdr.GetInt32(4));
                    ftag.Append(rdr.GetInt32(5));
                    ftr.Append(rdr.GetString(6));
                    hthg.Append(rdr.GetInt32(7));
                    htag.Append(rdr.GetInt32(8));
                    htr.Append(rdr.GetString(9));
                    hs.Append(rdr.GetInt32(10));
                    aws.Append(rdr.GetInt32(11));
                    hst.Append(rdr.GetInt32(12));
                    awst.Append(rdr.GetInt32(13));
                    hc.Append(rdr.GetInt32(14));
                    ac.Append(rdr.GetInt32(15));
                }
                conn.Close();
            }
            //CREATE DATAFRAME
            DataFrame df = new DataFrame(thisDiv, date, homeTeam, awayTeam, fthg, ftag, ftr, hthg, htag, htr, hs, aws, hst, awst, hc, ac);
            Console.WriteLine(df.Info());
            Console.WriteLine(df.Sample(20));
        }
    }
}
