using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SqlResultUpdater2
{
    class Program
    {
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
        static string SqlDateConvert (string oldDate){
            string[] parts=oldDate.Split('/');
            string newDate="'" + parts[2] + "-" + parts[1] + "-" + parts[0] + "'";
            return newDate;
        }
        static string SqlStringConvert (string oldString){
            string newString="'"+oldString+"'";
            return newString;
        }
        static void Main(string[] args)
        {
            string fname="updateDt.txt";
            Console.WriteLine("Checking date");
            DateTime prevUpDt=Convert.ToDateTime("01/01/1970");
            using (StreamReader sr = new StreamReader(fname)){
                prevUpDt=Convert.ToDateTime(sr.ReadLine()).AddDays(-7);
            }
            List<string> Leagues = new List<string>();
            Leagues.Add("B1"); Leagues.Add("D1"); Leagues.Add("D2"); Leagues.Add("E0"); Leagues.Add("E1"); Leagues.Add("E2"); 
            Leagues.Add("E3"); Leagues.Add("EC"); Leagues.Add("F1"); Leagues.Add("F2"); Leagues.Add("G1"); Leagues.Add("I1");
            Leagues.Add("I2"); Leagues.Add("N1"); Leagues.Add("P1"); Leagues.Add("SC0"); Leagues.Add("SC1"); Leagues.Add("SC2");
            Leagues.Add("SC3"); Leagues.Add("SP1"); Leagues.Add("SP2"); Leagues.Add("T1");

            List<string> ExtraLeagues=new List<string>();
            /*ExtraLeagues.Add("ARG");*/ ExtraLeagues.Add("AUT"); ExtraLeagues.Add("BRA"); ExtraLeagues.Add("CHN");
            ExtraLeagues.Add("DNK"); ExtraLeagues.Add("FIN"); ExtraLeagues.Add("IRL"); ExtraLeagues.Add("JPN");
            ExtraLeagues.Add("MEX"); ExtraLeagues.Add("NOR"); ExtraLeagues.Add("POL"); ExtraLeagues.Add("ROU");
            ExtraLeagues.Add("RUS"); ExtraLeagues.Add("SWE"); ExtraLeagues.Add("SWZ"); ExtraLeagues.Add("USA");

            List<string> Seasons=new List<string>();
            /*Seasons.Add("0506");
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
            Seasons.Add("1819"); */
            Seasons.Add("1920");
            Seasons.Add("2021");

            //download main leagues
            foreach (string lg in Leagues){
                foreach (string sn in Seasons){
                    string webAddr = "https://www.football-data.co.uk/mmz4281/" + sn + "/" + lg + ".csv";
                    string fileName = "Data/" + lg + "-" + sn + ".csv";
                    if (URLExists(webAddr) ==true){
                        if (File.Exists(fileName)){
                            File.Delete(fileName);
                        }
                        Console.WriteLine("Downloading results for {0}, {1}", lg, sn);
                        using (var client = new WebClient()){
                            client.DownloadFile(webAddr, fileName);
                        }
                        Console.WriteLine("Downloaded");
                    }
                }
            }
            
            //download extra leagues
            foreach (string lg in ExtraLeagues){
                string webAddr = "https://www.football-data.co.uk/new/"  + lg + ".csv";
                string fileName = "Data/" + lg + ".csv";
                if (URLExists(webAddr) ==true){
                    if (File.Exists(fileName)){
                        File.Delete(fileName);
                    }
                    Console.WriteLine("Downloading results for {0}", lg);
                    using (var client = new WebClient()){
                        client.DownloadFile(webAddr, fileName);
                    }
                    Console.WriteLine("Downloaded");
                }
                
            }
            
            List<string> AllMatches=new List<string>();
            string rqdHeadersAsOne="Div,Date,HomeTeam,AwayTeam,FTHG,FTAG,B365H,B365D,B365A";
            string[] rqdHeadersSplit=rqdHeadersAsOne.Split(',');
            foreach (string lg in Leagues){
                foreach (string season in Seasons){
                    string fileName = "Data/" + lg + "-" + season + ".csv";
                    List<string> AllHeaders=new List<string>();
                    if (File.Exists(fileName)){
                        using (StreamReader sr=new StreamReader(fileName)){
                            while(sr.Peek()>0){
                                string thisMatch="";
                                string thisLine=sr.ReadLine().Replace("'",string.Empty);
                                Console.WriteLine(thisLine);
                                string[] cells=thisLine.Split(',');
                                if (cells[0]=="Div"){
                                    foreach (string hdr in cells){
                                        AllHeaders.Add(hdr);
                                    }
                                }else if (cells[0]!=""&&Convert.ToDateTime(cells[1]).CompareTo(prevUpDt)>=0){
                                    for (int i=0;i<rqdHeadersSplit.Length;i++){
                                        string str=rqdHeadersSplit[i];
                                        string thisCell=cells[AllHeaders.IndexOf(str)];
                                        string amendedCell=thisCell;
                                        //make date sql ready
                                        if (i==1){
                                            amendedCell=SqlDateConvert(thisCell);
                                        }
                                        //make strings sql ready (league, teams, ftg, atg)
                                        if (i==0||i==2||i==3/*||i==6||i==9*/){
                                            amendedCell=SqlStringConvert(thisCell);
                                        }
                                        thisMatch+=amendedCell;
                                        //Console.Write(cells[AllHeaders.IndexOf(str)]+",");
                                        if (i!=rqdHeadersSplit.Length-1){
                                            thisMatch+=",";
                                        }
                                    }
                                    bool isValid=true;
                                    string[] thisMatchPts=thisMatch.Split(',');
                                    for (int i=0;i<thisMatchPts.Length;i++){
                                        if (string.IsNullOrEmpty(thisMatchPts[i])){
                                            isValid=false;
                                        }
                                    }
                                    if (isValid==true){
                                        AllMatches.Add(thisMatch);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            //MINOR LEAGUES TO LIST
            rqdHeadersAsOne="Country,Date,Home,Away,HG,AG,PH,PD,PA";
            rqdHeadersSplit=rqdHeadersAsOne.Split(',');
            foreach (string lg in ExtraLeagues){
                string fileName = "Data/" + lg + ".csv";
                List<string> AllHeaders=new List<string>();
                using (StreamReader sr=new StreamReader(fileName)){
                    while(sr.Peek()>0){
                        string thisMatch="";
                        string thisLine=sr.ReadLine().Replace("'",string.Empty);
                        Console.WriteLine(thisLine);
                        string[] cells=thisLine.Split(',');
                        
                        if (cells[0]=="Country"){
                            foreach (string hdr in cells){
                                AllHeaders.Add(hdr);
                            }
                        }else if (cells[0]!=""&&Convert.ToDateTime(cells[3]).CompareTo(prevUpDt)>=0){
                            for (int i=0;i<rqdHeadersSplit.Length;i++){
                                string str=rqdHeadersSplit[i];
                                string thisCell=cells[AllHeaders.IndexOf(str)];
                                string amendedCell=thisCell;
                                //make date sql ready
                                if (i==1){
                                    amendedCell=SqlDateConvert(thisCell);
                                }
                                //make strings sql ready (league, teams, ftg, atg)
                                if (i==0||i==2||i==3/*||i==6||i==9*/){
                                    amendedCell=SqlStringConvert(thisCell);
                                }
                                thisMatch+=amendedCell;
                                //Console.Write(cells[AllHeaders.IndexOf(str)]+",");
                                if (i!=rqdHeadersSplit.Length-1){
                                    thisMatch+=",";
                                }
                            }
                            bool isValid=true;
                            string[] thisMatchPts=thisMatch.Split(',');
                            for (int i=0;i<thisMatchPts.Length;i++){
                                if (string.IsNullOrEmpty(thisMatchPts[i])){
                                    isValid=false;
                                }
                            }
                            if (isValid==true){
                                AllMatches.Add(thisMatch);
                            }
                        }
                    }
                }
                
            }
            //add to sql
            if (AllMatches.Count>0){
                string connStr="server=localhost;user=simon;database=football;port=3306;password=chainsaw";
                Console.WriteLine("Writing to MySql database");
                DateTime lastUpdate=Convert.ToDateTime("14/05/2020");
                using (MySqlConnection conn=new MySqlConnection(connStr)){
                    conn.Open();
                    foreach (string lgMatch in AllMatches){
                        string[] cells=lgMatch.Split(',');
                        string[] cDatePts=cells[1].Split("-");
                        string newDate=(cDatePts[2]+"/"+cDatePts[1]+"/"+cDatePts[0]).Replace("'",string.Empty);
                        DateTime thisFxDt=Convert.ToDateTime(newDate);
                        if (thisFxDt.CompareTo(lastUpdate)>=0){
                            Console.WriteLine("Checking/writing: ");
                            Console.WriteLine(lgMatch);
                            string sql="REPLACE INTO results_and_odds VALUES ("+lgMatch+")";
                            MySqlCommand cmd=new MySqlCommand(sql,conn);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
            }
            DateTime prevUpdate = DateTime.Now;
            if (File.Exists(fname)){
                File.Delete(fname);
            }
            using (StreamWriter sw = new StreamWriter(fname)){
                sw.Write(prevUpdate);
            }
        }
    }
}
