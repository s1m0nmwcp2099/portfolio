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
            List<string> _ThisDiv = new List<string>();
            //Date
            List<DateTime> _Date = new List<DateTime>();
            //HomeTeam
            List<string> _HomeTeam = new List<string>();
            //AwayTeam
            List<string> _AwayTeam = new List<string>();
            //FTHG full time home goals
            List<int> _FTHG = new List<int>();
            //FTAG ...away
            List<int> _FTAG = new List<int>();
            //FTR ...result
            List<string> _FTR = new List<string>();
            //HTHG half time home goals
            List<int> _HTHG = new List<int>();
            //HTAG
            List<int> _HTAG = new List<int>();
            //HTR
            List<string> _HTR = new List<string>();
            //HS shots
            List<int> _HS = new List<int>();
            //AwS
            List<int> _AwS = new List<int>();
            //HST shots on target
            List<int> _HST = new List<int>();
            //AwST
            List<int> _AwST = new List<int>();
            //HC corners
            List<int> _HC = new List<int>();
            //AC
            List<int> _AC = new List<int>();
            string sql = "SELECT * FROM football_data_complete WHERE FTHG IS NOT NULL AND FTAG IS NOT NULL AND FTR IS NOT NULL AND HTHG IS NOT NULL AND HTAG IS NOT NULL AND HTR IS NOT NULL AND HS IS NOT NULL AND AwS IS NOT NULL AND HST IS NOT NULL AND AwST IS NOT NULL AND HC IS NOT NULL AND AC IS NOT NULL";
            using (MySqlConnection conn = new MySqlConnection(connStr)){
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read()){
                    //rdr.GetOrdinal("Date");
                }
            }
        }
    }
}
