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
        static string CheckStringChars(string inStr){
            string outStr = "";
            for (int i = 0; i < inStr.Length; i++){
                if ((int)inStr[i] == 65533){
                    outStr += "o";
                }else if (i != inStr.Length - 1 || (int)inStr[i] != 32){
                    outStr += inStr[i];
                }
            }
            return outStr;
        }
        static string ReduceHeader(string originalHeader){
            string newHeader = originalHeader;
            if (originalHeader == "Country" || originalHeader == "Div"){
                newHeader = "Div";
            }else if (originalHeader == "Home"){
                newHeader = "HomeTeam";
            }else if (originalHeader == "Away"){
                newHeader = "AwayTeam";
            }else if (originalHeader == "HG"){
                newHeader = "FTHG";
            }else if (originalHeader == "AG"){
                newHeader = "FTAG";
            }else if (originalHeader == "PH"){
                newHeader = "PSH";
            }else if (originalHeader == "PD"){
                newHeader = "PSD";
            }else if (originalHeader == "PA"){
                newHeader = "PSA";
            }else if (originalHeader == "Res"){
                newHeader = "FTR";
            }
            return newHeader;
        }
        static string SqliseCsvHeaderLine(string line){
            line = line.Replace("Div", "ThisDiv");
            line = line.Replace(">", "over");
            line = line.Replace("<", "under");
            line = line.Replace("2.5", "TwoPtFive");
            line = line.Replace("365", "Stk");
            line = line.Replace("1X2", "ResTot");
            line = line.Replace("AS", "AwS");
            return line;
        }
        static string SqliseCsvVariableLine(string line){
            line = line.Replace("\"", String.Empty);
            line = line.Replace("INT,", "INT-");
            line = line.Replace("TIME,", "TIME-");
            line = line.Replace("DATE,", "DATE-");
            line = line.Replace("CHAR,", "CHAR-");
            line = line.Replace("),", ")-");
            return line;
        }
        static string SqliseDate (string dtStr){
            DateTime dt = Convert.ToDateTime(dtStr);
            return dt.ToString("yyyy-MM-dd");
        }
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
                    foreach (string originalHdr in hdrs){
                        if (originalHdr != ""){
                            string newHdr = ReduceHeader(originalHdr);
                            if (!SqlHeaders.Contains(newHdr)){
                                SqlHeaders.Add(newHdr);
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
            allHeaders = SqliseCsvHeaderLine(allHeaders);
            string[] headers = allHeaders.Split(',');
            Console.WriteLine("headers = " + headers.Length);
            
            //get header sql types 
            string allHdrTypes = "";
            using (StreamReader sr = new StreamReader("Data/Prog/bigDataSqlColumnTypes.csv")){
                allHdrTypes = sr.ReadLine();
            }
            allHdrTypes = SqliseCsvVariableLine(allHdrTypes);
            string[] hdrTypes = allHdrTypes.Split('-');
            Console.WriteLine("header types = " + hdrTypes.Length);

            /*foreach (string s in hdrTypes){
                Console.WriteLine(s);
            }*/

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
            /*using (MySqlConnection conn = new MySqlConnection(connStr)){
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }*/

            
            //write to sql table
            foreach (string fName in LeagueFileNames){
                //fetch from file and write to list
                List<string> ThisPrevCsv = new List<string>();
                using (StreamReader sr = new StreamReader(fName)){
                    while (sr.Peek() > 0){
                        ThisPrevCsv.Add(sr.ReadLine());
                    }
                }
                //get headers
                string[] hdrs = ThisPrevCsv[0].Split(',');
                for (int i = 0; i < hdrs.Length; i++){
                    hdrs[i] = ReduceHeader(hdrs[i]);
                }
                //string hdrLine = string.Join(",", hdrs);
                string hdrLine = "";
                bool started = false;
                for (int i = 0; i < hdrs.Length; i++){
                    if (String.IsNullOrEmpty(hdrs[i]) == false){
                        if (started == true){
                            hdrLine += ",";
                        }
                        hdrLine += hdrs[i];
                    }
                    started = true;
                }
                hdrLine = SqliseCsvHeaderLine(hdrLine);
                hdrs = hdrLine.Split(',');
                foreach (string y in hdrs){
                    Console.WriteLine($"hdr: {y}");
                }
                //get index of referee column to remove diacritics
                //int refereeInd = Array.IndexOf(hdrs, "Referee");
                //int DateInd = Array.IndexOf(hdrs, "Date");

                //go through each line
                for (int match = 1; match < ThisPrevCsv.Count; match++){
                    string thisCsvLine = ThisPrevCsv[match];
                    string[] parts = thisCsvLine.Split(',');
                    if (String.IsNullOrEmpty(parts[0]) == false){
                        //start sql string
                        string sqlReplaceStart = "REPLACE INTO football_data_complete (";
                        string sqlReplaceEnd = " VALUES (";
                        bool valAdded = false;
                        for (int i = 0; i < hdrs.Length && i < parts.Length; i++){//LAST
                            if (hdrs[i] == "Date"){
                                parts[i] = SqliseDate(parts[i]);
                            }
                            if (hdrs[i] == "Referee"){
                                parts[i] = CheckStringChars(parts[i]);
                                //Console.ReadLine();
                            }
                            //check if cell has value
                            if (string.IsNullOrEmpty(parts[i]) == false && string.IsNullOrEmpty(hdrs[i]) == false){
                                if (valAdded == true){
                                    sqlReplaceStart += ", ";
                                    sqlReplaceEnd += ", ";
                                }
                                sqlReplaceStart += hdrs[i];
                                if (hdrTypes[i] == "VARCHAR(11)" || hdrTypes[i] == "DATE" || hdrTypes[i] == "VARCHAR(25)" || hdrTypes[i] == "CHAR" || hdrTypes[i] == "TIME"){
                                    sqlReplaceEnd += ($"'{parts[i]}'");
                                }else{
                                    sqlReplaceEnd += parts[i];
                                }
                                valAdded = true;
                            }
                            if (parts[0] == "D1"){
                                Console.WriteLine($"i = {i} header length = {hdrs.Length}");
                                Console.WriteLine(sqlReplaceStart + sqlReplaceEnd);
                                //Console.ReadLine();
                            }
                        }
                        sqlReplaceStart += ")";
                        sqlReplaceEnd += ");";
                        string sqlReplaceWhole = sqlReplaceStart + sqlReplaceEnd;
                        Console.WriteLine(sqlReplaceWhole);
                        //Console.ReadLine();
                    }
                }
            }
        }
    }
}