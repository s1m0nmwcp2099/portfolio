using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace BigFootySql
{
    class Program
    {
        static string connStr = "server = localhost; user = simon; database = football; port = 3306; password = chainsaw";
        static bool URLExists(string url)
        {
            bool result = true;
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Timeout = 6000; // miliseconds
            webRequest.Method = "HEAD";
            try
            {
                webRequest.GetResponse();
            }
            catch
            {
                result = false;
            }
            return result;
        }
        static void DownloadAndWriteData(string webAddr, string fileName){
            if (URLExists(webAddr) == true){
                if (File.Exists(fileName)){
                    File.Delete(fileName);
                }
                using (var client = new WebClient()){
                    client.DownloadFile(webAddr, fileName);
                }
            }
        }
        static void Main(string[] args)
        {
            List<string> Seasons=new List<string>();
            Seasons.Add("9394");
            Seasons.Add("9495");
            Seasons.Add("9596");
            Seasons.Add("9697");
            Seasons.Add("9798");
            Seasons.Add("9899");
            Seasons.Add("9900");
            Seasons.Add("0001");
            Seasons.Add("0102");
            Seasons.Add("0203");
            Seasons.Add("0304");
            Seasons.Add("0405");
            Seasons.Add("0506");
            Seasons.Add("0607");
            Seasons.Add("0708");
            Seasons.Add("0809");
            Seasons.Add("0910");
            Seasons.Add("1011");
            Seasons.Add("1112");
            Seasons.Add("1213");
            Seasons.Add("1314");
            Seasons.Add("1415");
            Seasons.Add("1516");
            Seasons.Add("1617");
            Seasons.Add("1718"); 
            Seasons.Add("1819");
            Seasons.Add("1920");
            //Seasons.Add("2021"); //WHEN NEW SEASON KICKS OFF

            List<string> Leagues = new List<string>();
            Leagues.Add("B1"); Leagues.Add("D1"); Leagues.Add("D2"); Leagues.Add("E0"); Leagues.Add("E1"); Leagues.Add("E2"); 
            Leagues.Add("E3"); Leagues.Add("EC"); Leagues.Add("F1"); Leagues.Add("F2"); Leagues.Add("G1"); Leagues.Add("I1");
            Leagues.Add("I2"); Leagues.Add("N1"); Leagues.Add("P1"); Leagues.Add("SC0"); Leagues.Add("SC1"); Leagues.Add("SC2");
            Leagues.Add("SC3"); Leagues.Add("SP1"); Leagues.Add("SP2"); Leagues.Add("T1");
            List<string> ExtraLeagues = new List<string>();
            ExtraLeagues.Add("AUT"); ExtraLeagues.Add("BRA"); ExtraLeagues.Add("CHN");
            ExtraLeagues.Add("DNK"); ExtraLeagues.Add("FIN"); ExtraLeagues.Add("IRL"); ExtraLeagues.Add("JPN");
            ExtraLeagues.Add("MEX"); ExtraLeagues.Add("NOR"); ExtraLeagues.Add("POL"); ExtraLeagues.Add("ROU");
            ExtraLeagues.Add("RUS"); ExtraLeagues.Add("SWE"); ExtraLeagues.Add("SWZ"); ExtraLeagues.Add("USA");

            bool[,] isDataMainLgs = new bool[Leagues.Count, Seasons.Count];
            for (int i = 0; i < Leagues.Count; i++){
                for (int j = 0; j < Seasons.Count; j++){
                    isDataMainLgs[i, j] = false;
                }
            }
            
            for (int j = 0; j < Seasons.Count; j++){
                if (j >= 2){
                    isDataMainLgs[Leagues.IndexOf("B1"), j] = true;
                }
                isDataMainLgs[Leagues.IndexOf("D1"), j] = true;
                isDataMainLgs[Leagues.IndexOf("D2"), j] = true;
                isDataMainLgs[Leagues.IndexOf("E0"), j] = true;
                isDataMainLgs[Leagues.IndexOf("E1"), j] = true;
                isDataMainLgs[Leagues.IndexOf("E2"), j] = true;
                isDataMainLgs[Leagues.IndexOf("E3"), j] = true;
                if (j >= 12){
                    isDataMainLgs[Leagues.IndexOf("EC"), j] = true;
                }
                isDataMainLgs[Leagues.IndexOf("F1"), j] = true;
                if (j >= 3){
                    isDataMainLgs[Leagues.IndexOf("F2"), j] = true;
                    isDataMainLgs[Leagues.IndexOf("SP2"), j] = true;
                }
                if (j >= 1){
                    isDataMainLgs[Leagues.IndexOf("G1"), j] = true;
                    isDataMainLgs[Leagues.IndexOf("P1"), j] = true;
                    isDataMainLgs[Leagues.IndexOf("SC0"), j] = true;
                    isDataMainLgs[Leagues.IndexOf("SC1"), j] = true;
                    isDataMainLgs[Leagues.IndexOf("T1"), j] = true;
                }
                isDataMainLgs[Leagues.IndexOf("I1"), j] = true;
                if (j >= 4){
                    isDataMainLgs[Leagues.IndexOf("I2"), j] = true;
                    isDataMainLgs[Leagues.IndexOf("SC2"), j] = true;
                    isDataMainLgs[Leagues.IndexOf("SC3"), j] = true;
                }
                isDataMainLgs[Leagues.IndexOf("N1"), j] = true;
                isDataMainLgs[Leagues.IndexOf("SP1"), j] = true;
            }
            
            //download files
            List<string> LeagueFileNames = new List<string>();
            for (int i = 0; i < Leagues.Count; i++){
                for (int j = 0; j < Seasons.Count; j++){
                    if (isDataMainLgs[i, j] == true){
                        string leagueUrl = $"https://www.football-data.co.uk/mmz4281/{Seasons[j]}/{Leagues[i]}.csv";
                        string fName = $"Data/Previous/{Seasons[j]}-{Leagues[i]}.csv";
                        Console.WriteLine($"Downloading: {Seasons[j]}-{Leagues[i]}");
                        //DownloadAndWriteData(leagueUrl, fName);
                        LeagueFileNames.Add(fName);
                    }
                }
            }
            for (int i = 0; i < ExtraLeagues.Count; i ++){
                string leagueUrl = $"https://www.football-data.co.uk/new/{ExtraLeagues[i]}.csv";
                string fName = $"Data/Previous/{ExtraLeagues[i]}";
                Console.WriteLine($"Downloading: {ExtraLeagues[i]}");
                //DownloadAndWriteData(leagueUrl, fName);
                LeagueFileNames.Add(fName);
            }

            //Get headers for sql table
            List<string> SqlHeaders = new List<string>();
            foreach (string fName in LeagueFileNames){
                if (File.Exists(fName)){
                    List<string> _headers = new List<string>();
                    string hdrLine = "";
                    using (StreamReader sr = new StreamReader(fName)){
                        hdrLine = sr.ReadLine();
                    }
                    string[] hdrs = hdrLine.Split(',');
                    foreach (string hdr in hdrs){
                        if (hdr != ""){
                            string _hdr = hdr;
                            if (hdr == "Country"){
                                _hdr = "Div";
                            }else if (hdr == "Home"){
                                _hdr = "HomeTeam";
                            }else if (hdr == "Away"){
                                _hdr = "AwayTeam";
                            }else if (hdr == "HG"){
                                _hdr = "FTHG";
                            }else if (hdr == "AG"){
                                _hdr = "FTAG";
                            }else if (hdr == "PH"){
                                _hdr = "PSH";
                            }else if (hdr == "PD"){
                                _hdr = "PSD";
                            }else if (hdr == "PA"){
                                _hdr = "PSA";
                            }else if (hdr == "Res"){
                                _hdr = "FTR";
                            }
                        
                            if (!SqlHeaders.Contains(_hdr)){
                                SqlHeaders.Add(_hdr);
                            }
                        }
                    }
                }
            }
            string x = string.Join(',', SqlHeaders);
            string hdrFName = "Data/Prog/sqlHeaders.csv";
            if (File.Exists(hdrFName)){
                File.Delete(hdrFName);
            }
            using (StreamWriter sw = new StreamWriter(hdrFName)){
                sw.WriteLine(x);
            }

            //create sql table
            //start sql command
            string sql = "CREATE TABLE football_data_complete (";

            //get headers 
            string allHeaders = "";
            using (StreamReader sr = new StreamReader("Data/Prog/sqlHeaders.csv")){
                allHeaders = sr.ReadLine();
            }
            allHeaders = allHeaders.Replace("Div", "ThisDiv");
            allHeaders = allHeaders.Replace(">", "over");
            allHeaders = allHeaders.Replace("<", "under");
            allHeaders = allHeaders.Replace("2.5", "TwoPtFive");
            allHeaders = allHeaders.Replace("365", "Stk");
            allHeaders = allHeaders.Replace("1X2", "ResTot");
            allHeaders = allHeaders.Replace("AS", "AwS");
            string[] headers = allHeaders.Split(',');
            Console.WriteLine("headers = " + headers.Length);
            
            //get header sql types 
            string allHdrTypes = "";
            using (StreamReader sr = new StreamReader("Data/Prog/bigDataSqlColumnTypes.csv")){
                allHdrTypes = sr.ReadLine();
            }
            allHdrTypes = allHdrTypes.Replace("\"", String.Empty);
            allHdrTypes = allHdrTypes.Replace("INT,", "INT-");
            allHdrTypes = allHdrTypes.Replace("TIME,", "TIME-");
            allHdrTypes = allHdrTypes.Replace("DATE,", "DATE-");
            allHdrTypes = allHdrTypes.Replace("CHAR,", "CHAR-");
            allHdrTypes = allHdrTypes.Replace("),", ")-");
            string[] hdrTypes = allHdrTypes.Split('-');
            Console.WriteLine("header types = " + hdrTypes.Length);

            foreach (string s in hdrTypes){
                Console.WriteLine(s);
            }

            //add columns and types to sql string
            for (int i = 0; i < headers.Length; i++){
                if (i > 0){
                    sql += ", ";
                }
                sql += $"{headers[i]} {hdrTypes[i]}";
            }

            //end sql string
            sql += ");";

            Console.WriteLine(sql);
            using (MySqlConnection conn = new MySqlConnection(connStr)){
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            /*
            //write to sql
            foreach (string fName in LeagueFileNames){
                if (File.Exists(fName)){

                }
            }
            */
        }
    }
}