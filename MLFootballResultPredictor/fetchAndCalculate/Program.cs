using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace fetchAndCalculate
{
    class Program
    {
        static string ModMatchLine(string rawMatchLine, bool alreadyPld){
            string[] rawMatchPts = rawMatchLine.Split(',');
            string lg = rawMatchPts[0]; DateTime thisMatchDt = Convert.ToDateTime(rawMatchPts[1]);
            string hmTm = rawMatchPts[2]; string awTm = rawMatchPts[3];

            //Get home team recent matches
            List<string> RecentHomeTeamMatches = GetRecentMatches(hmTm, true, thisMatchDt);
            double[] homeTeamData = TeamData2(RecentHomeTeamMatches, true, thisMatchDt);
            List<string> RecentAwayTeamMatches = GetRecentMatches(awTm, false, thisMatchDt);
            double[] awayTeamData = TeamData2(RecentAwayTeamMatches, false, thisMatchDt);
            if (RecentHomeTeamMatches.Count<10 && RecentAwayTeamMatches.Count<10){
                hmTm = "void"; awTm = "void";
            }
            double hmOdds = Convert.ToDouble(rawMatchPts[6]);
            double drOdds = Convert.ToDouble(rawMatchPts[7]);
            double awOdds = Convert.ToDouble(rawMatchPts[9]);
            string thisLine = lg+","+rawMatchPts[1]+","+hmTm+","+awTm+","+rawMatchPts[6]+","+rawMatchPts[7]+","+rawMatchPts[8];
            for (int i=0;i<homeTeamData.Length;i++){
                thisLine+=(","+Convert.ToString(homeTeamData));
            }
            for (int i=0;i<awayTeamData.Length;i++){
                thisLine+=(","+Convert.ToString(awayTeamData));
            }
            if (alreadyPld == true){
                int hmGls = Convert.ToInt32(rawMatchPts[4]); int awGls = Convert.ToInt32(rawMatchPts[5]);
                if (hmGls>awGls){
                    thisLine+=(",H");
                }else if (hmGls==awGls){
                    thisLine+=(",D");
                }else{
                    thisLine+=(",A");
                }
            }
            return thisLine;
        }
        static List<string> GetRecentMatches(string thisTeam, bool atHome, DateTime dt){
            string sqlDt = dt.ToString("yyyy-MM-dd");
            List<string> PrevMatches = new List<string>();
            string tm = "";
            if (atHome ==true){
                tm = "home_team";
            }else{
                tm = "away_team";
            }
            string connStr="server=localhost;user=simon;database=football;port=3306;password=chainsaw";
            string sql="SELECT * FROM results_and_odds WHERE "  + tm + " = '" + thisTeam + "' AND date < '" + sqlDt +"' ORDER BY date DESC LIMIT 10";
            using MySqlConnection con=new MySqlConnection(connStr);
            con.Open();
            using MySqlCommand cmd=new MySqlCommand(sql,con);
            using MySqlDataReader rdr=cmd.ExecuteReader();
            while (rdr.Read()){
                string thisPrevMatch = "";
                for (int i = 0; i < 9; i++){
                    if (i > 0){
                        thisPrevMatch+=",";
                    }
                    thisPrevMatch+=rdr.GetValue(i);
                }
            }
            con.Close();
            return PrevMatches;
        }
        static bool URLExists(string url)
        {
            //THIS IS TO CHECK THAT THE URL IS ACCESSIBLE AND AVOID ERROR IF WE CAN'T CONNECT
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
        //ORIGINAL DATA INC SHOTS,OT,C
        static double[] TeamData(List<string> _AllRawMatches, List<int> _Indices, int _min,bool atHome, DateTime _currentDate){
            //0=glsF,1=glsAg,2=shF,3=shA,4=shTf,5=shTag,6=cF,7=cAg,8=wins,9=draws,10=losses
            double[] thisTeamData=new double[11];
            double pld=0;
            double glsF=0; double glsAg=0; double shF=0; double shAg=0; double shTf=0; double shTag=0; double cF=0; double cAg=0;
            double wins=0; double draws=0; double losses=0;
            
            int adj=0;
            if (atHome==false){
                adj=1;
            }
            for (int i=1;i<=_min;i++){
                string thisMatch=_AllRawMatches[_Indices[_Indices.Count-i]];
                string[] cells=thisMatch.Split(',');
                DateTime prevDate=Convert.ToDateTime(cells[1]);
                TimeSpan gap=_currentDate-prevDate;
                int daysAgo=gap.Days;
                double thisExp=Math.Exp(-0.007*daysAgo);
                int goalsFor=Convert.ToInt32(cells[4+adj]); int goalsAg=Convert.ToInt32(cells[5-adj]);
                double thisGf=Convert.ToDouble(cells[4+adj]); double thisGag=Convert.ToDouble(cells[5-adj]);
                double thisShF=Convert.ToDouble(cells[10+adj]); double thisShAg=Convert.ToDouble(cells[11-adj]);
                double thisShTf=Convert.ToDouble(cells[12+adj]); double thisShTAg=Convert.ToDouble(cells[13-adj]);
                double thisCf=Convert.ToDouble(cells[14+adj]); double thisCag=Convert.ToDouble(cells[15-adj]);
                //Apply data
                pld+=thisExp;
                if (goalsFor>goalsAg){
                    wins+=thisExp;
                }else if (goalsFor==goalsAg){
                    draws+=thisExp;
                }else{
                    losses+=thisExp;
                }
                glsF+=(thisGf*thisExp); glsAg+=(thisGag*thisExp);
                shF+=(thisShF*thisExp); shAg+=(thisShAg*thisExp);
                shTf+=(thisShTf*thisExp); shTag+=(thisShTAg*thisExp);
                cF+=(thisCf*thisExp); cAg+=(thisCag*thisExp);
            }
            thisTeamData[0]=(glsF/pld); thisTeamData[1]=(glsAg/pld);
            thisTeamData[2]=(shF/pld); thisTeamData[3]=(shAg/pld);
            thisTeamData[4]=(shTf/pld); thisTeamData[5]=(shTag/pld);
            thisTeamData[6]=(cF/pld); thisTeamData[7]=(cAg/pld);
            thisTeamData[8]=(wins/pld); thisTeamData[9]=(draws/pld); thisTeamData[10]=(losses/pld);
            return thisTeamData;
        }

        //SAME AS PREVIOUS BUT WITHOUT SHOTS, OT, CORNERS
        static double[] TeamData2(List<string> RawMatches, bool atHome, DateTime thisMatchDate){
            //0=glsF,1=glsAg,2=wins,3=draws,4=losses
            double[] thisTeamData=new double[5];
            double pld=0;
            double glsF=0; double glsAg=0; double wins=0; double draws=0; double losses=0;
            
            int adj=0;
            if (atHome==false){
                adj=1;
            }
            for (int i=0;i<9;i++){
                string thisMatch=RawMatches[i];
                string[] cells=thisMatch.Split(',');
                DateTime prevDate=Convert.ToDateTime(cells[1]);
                TimeSpan gap=thisMatchDate-prevDate;
                int daysAgo=gap.Days;
                double thisExp=Math.Exp(-0.007*daysAgo);
                int goalsFor=Convert.ToInt32(cells[4+adj]); int goalsAg=Convert.ToInt32(cells[5-adj]);
                double thisGf=Convert.ToDouble(cells[4+adj]); double thisGag=Convert.ToDouble(cells[5-adj]);
                
                //Apply data
                pld+=thisExp;
                if (goalsFor>goalsAg){
                    wins+=thisExp;
                }else if (goalsFor==goalsAg){
                    draws+=thisExp;
                }else{
                    losses+=thisExp;
                }
                glsF+=(thisGf*thisExp); glsAg+=(thisGag*thisExp);
            }
            thisTeamData[0]=(glsF/pld); thisTeamData[1]=(glsAg/pld);
            thisTeamData[2]=(wins/pld); thisTeamData[3]=(draws/pld); thisTeamData[4]=(losses/pld);
            return thisTeamData;
        }
        //TO GO WITH TEAMDATA2
        static double[] GetAvOppStats(List<string> _AllProcessedMatches, List<int> _TmsPrevMatches, int _min, bool _forHmTm){
            double[] thisStats=new double[5];
            double pld=0;
            double glsF=0; double glsAg=0; double wins=0; double draws=0; double losses=0;
            int adj=0;
            if (_forHmTm==true){
                adj=5;
            }
            for (int i=1;i<=_min;i++){
                string thisMatch=_AllProcessedMatches[_TmsPrevMatches[_TmsPrevMatches.Count-i]];
                string[] cells=thisMatch.Split(',');
                DateTime _currentDate=Convert.ToDateTime(cells[1]);
                DateTime prevDate=Convert.ToDateTime(cells[1]);
                TimeSpan gap=_currentDate-prevDate;
                int daysAgo=gap.Days;
                double thisExp=Math.Exp(-0.007*daysAgo);
                glsF+=(Convert.ToDouble(cells[7+adj])*thisExp);
                glsAg+=(Convert.ToDouble(cells[8+adj])*thisExp);
                wins+=(Convert.ToDouble(cells[9+adj])*thisExp);
                draws+=(Convert.ToDouble(cells[10+adj])*thisExp);
                losses+=(Convert.ToDouble(cells[11+adj])*thisExp);
                pld+=thisExp;
            }
            thisStats[0]=glsF/=pld;
            thisStats[1]=glsAg/=pld;
            thisStats[2]=wins/=pld;
            thisStats[3]=draws/=pld;
            thisStats[4]=losses/=pld;
            return thisStats;
        }
        
        static int[] TeamStreaks(List<string> _AllRawMatches,List<int> _Indices,string _team){
            int[] streakLength=new int[6]; //0=winning, 1=no winning, 2=drawing, 3=not drawing, 4=losing, 5=unbeaten
            bool[] streakOn=new bool[6];
            for (int i=0;i<6;i++){
                streakLength[i]=0;
                streakOn[i]=true;
            }
            for (int i=1;i<=_Indices.Count;i++){
                string thisMatch=_AllRawMatches[_Indices[_Indices.Count-i]];
                string[] cells=thisMatch.Split(',');

                //get score
                int adj=0;
                if (cells[3]==_team){//check if team is home or away
                    adj=1;
                }
                int goalsFor=Convert.ToInt32(cells[4+adj]);
                int goalsAg=Convert.ToInt32(cells[5-adj]);

                //amend streaks
                if (goalsFor>goalsAg){//WIN
                    //make indices 1,2,4 false
                    streakOn[1]=false; streakOn[2]=false; streakOn[4]=false;
                    //add to indices 0,3,5
                    if (streakOn[0]==true){
                        streakLength[0]+=1;
                    }
                    if (streakOn[3]==true){
                        streakLength[3]+=1;
                    }
                    if (streakOn[5]==true){
                        streakLength[5]+=1;
                    }
                    //0=winning, 1=no winning, 2=drawing, 3=not drawing, 4=losing, 5=unbeaten
                }else if (goalsFor==goalsAg){//DRAW
                    //make indices 0,3,4 false
                    streakOn[0]=false; streakOn[3]=false; streakOn[4]=false;
                    //add to indices 1,2,5
                    if (streakOn[1]==true){
                        streakLength[1]+=1;
                    }
                    if (streakOn[2]==true){
                        streakLength[2]+=1;
                    }
                    if (streakOn[5]==true){
                        streakLength[5]+=1;
                    }
                }else if (goalsFor<goalsAg){//LOSS
                    //make indices 0,2,5 false
                    streakOn[0]=false; streakOn[2]=false; streakOn[5]=false;
                    //add to indices 1,3,4
                    if (streakOn[1]==true){
                        streakLength[1]+=1;
                    }
                    if (streakOn[3]==true){
                        streakLength[3]+=1;
                    }
                    if (streakOn[4]==true){
                        streakLength[4]+=1;
                    }
                }
                bool stillAnyStreaks=false;
                for (int j=0;j<6;j++){
                    if (streakOn[j]==true){
                        stillAnyStreaks=true;
                    }
                }
                if (stillAnyStreaks==false){
                    break;
                }
            }
            return streakLength;
        }

        //takes team and opponent data (Team2 method) and combines hF to oppA, hW to oppD and oppL, etc
        static string[] CombinedStats(string matchStr, bool atHome){
            string[] output=new string[5];
            string[] cells=matchStr.Split(',');
            int adj=0;
            if (atHome==false){
                adj=5;
            }
            output[0]=Convert.ToString(Convert.ToDouble(cells[7+adj])-Convert.ToDouble(cells[18+adj]));
            output[1]=Convert.ToString(Convert.ToDouble(cells[8+adj])-Convert.ToDouble(cells[17+adj]));
            output[2]=Convert.ToString(Convert.ToDouble(cells[9+adj])-(Convert.ToDouble(cells[20+adj])+Convert.ToDouble(cells[21+adj]))/2);
            output[3]=Convert.ToString(Convert.ToDouble(cells[10+adj])-(Convert.ToDouble(cells[19+adj])+Convert.ToDouble(cells[21+adj]))/2);
            output[4]=Convert.ToString(Convert.ToDouble(cells[11+adj])-(Convert.ToDouble(cells[19+adj])+Convert.ToDouble(cells[20+adj]))/2);
            return output;
        }
        static void Main(string[] args)
        {string filename="../Data/fixtures.csv";
            string url="https://www.football-data.co.uk/fixtures.csv";
            Console.WriteLine("Do you want to download fixtures? y or n");
            string ans=Console.ReadLine();
            if ((ans=="y"||ans=="Y") && URLExists(url)==true){
                if (File.Exists(filename)){
                    File.Delete(filename);
                }
                using (var myClient=new WebClient()){
                    myClient.DownloadFile(url,filename);
                }
            }
            //FIRST UPDATE MYSQL TABLE OF RESULTS AND BOOKMAKER ODDS
            /*
            string fname="updateDt.txt"; //most recent update datetime in txt file
            Console.WriteLine("Checking date");
            DateTime prevUpDt=Convert.ToDateTime("01/01/1970");
            using (StreamReader sr = new StreamReader(fname)){
                prevUpDt=Convert.ToDateTime(sr.ReadLine());
            }
            List<string> Leagues = new List<string>(); //major European leagues
            Leagues.Add("B1"); Leagues.Add("D1"); Leagues.Add("D2"); Leagues.Add("E0"); Leagues.Add("E1"); Leagues.Add("E2"); 
            Leagues.Add("E3"); Leagues.Add("EC"); Leagues.Add("F1"); Leagues.Add("F2"); Leagues.Add("G1"); Leagues.Add("I1");
            Leagues.Add("I2"); Leagues.Add("N1"); Leagues.Add("P1"); Leagues.Add("SC0"); Leagues.Add("SC1"); Leagues.Add("SC2");
            Leagues.Add("SC3"); Leagues.Add("SP1"); Leagues.Add("SP2"); Leagues.Add("T1");

            List<string> ExtraLeagues=new List<string>(); //extra leagues
            ExtraLeagues.Add("ARG"); ExtraLeagues.Add("AUT"); ExtraLeagues.Add("BRA"); ExtraLeagues.Add("CHN");
            ExtraLeagues.Add("DNK"); ExtraLeagues.Add("FIN"); ExtraLeagues.Add("IRL"); ExtraLeagues.Add("JPN");
            ExtraLeagues.Add("MEX"); ExtraLeagues.Add("NOR"); ExtraLeagues.Add("POL"); ExtraLeagues.Add("ROU");
            ExtraLeagues.Add("RUS"); ExtraLeagues.Add("SWE"); ExtraLeagues.Add("SWZ"); ExtraLeagues.Add("USA");

            List<string> Seasons=new List<string>();
            //1920 (2019-2020) is the current season. All previous seasons are already in the database and therefore commented out
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

            //download main European leagues
            foreach (string lg in Leagues){
                foreach (string sn in Seasons){
                    string webAddr = "https://www.football-data.co.uk/mmz4281/" + sn + "/" + lg + ".csv";
                    string fileName2 = "../Data/" + lg + "-" + sn + ".csv";
                    if (URLExists(webAddr) ==true){
                        if (File.Exists(fileName2)){
                            File.Delete(fileName2);
                        }
                        Console.WriteLine("Downloading results for {0}, {1}", lg, sn);
                        using (var client = new WebClient()){
                            client.DownloadFile(webAddr, fileName2);
                        }
                        Console.WriteLine("Downloaded");
                    }
                }
            }
            
            //download extra leagues
            foreach (string lg in ExtraLeagues){
                string webAddr = "https://www.football-data.co.uk/new/"  + lg + ".csv";
                string fileName2 = "../Data/" + lg + ".csv";
                if (URLExists(webAddr) ==true){
                    if (File.Exists(fileName2)){
                        File.Delete(fileName2);
                    }
                    Console.WriteLine("Downloading results for {0}", lg);
                    using (var client = new WebClient()){
                        client.DownloadFile(webAddr, fileName2);
                    }
                    Console.WriteLine("Downloaded");
                }
                
            }
            
            List<string> AllMatches=new List<string>();
            string rqdHeadersAsOne="Div,Date,HomeTeam,AwayTeam,FTHG,FTAG,B365H,B365D,B365A";
            string[] rqdHeadersSplit=rqdHeadersAsOne.Split(',');
            foreach (string lg in Leagues){
                foreach (string season in Seasons){
                    string fileName2 = "../Data/" + lg + "-" + season + ".csv";
                    List<string> AllHeaders=new List<string>();
                    using (StreamReader sr=new StreamReader(fileName2)){
                        while(sr.Peek()>0){
                            string thisMatch="";
                            string thisLine=sr.ReadLine().Replace("'",string.Empty);
                            //Console.WriteLine(thisLine);
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
                                    if (i==0||i==2||i==3){
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
            
            //ADDITIONAL LEAGUES TO LIST
            //This is done separately to main leagues due to different header titles
            rqdHeadersAsOne="Country,Date,Home,Away,HG,AG,PH,PD,PA";
            rqdHeadersSplit=rqdHeadersAsOne.Split(',');
            foreach (string lg in ExtraLeagues){
                string fileName2 = "../Data/" + lg + ".csv";
                List<string> AllHeaders=new List<string>();
                using (StreamReader sr=new StreamReader(fileName2)){
                    while(sr.Peek()>0){
                        string thisMatch="";
                        string thisLine=sr.ReadLine().Replace("'",string.Empty);
                        //Console.WriteLine(thisLine);
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
                                if (i==0||i==2||i==3){
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
            string connStr="server=localhost;user=simon;database=football;port=3306;password=chainsaw";
            if (AllMatches.Count>0){
                connStr="server=localhost;user=simon;database=football;port=3306;password=chainsaw";
                Console.WriteLine("Writing to MySql database");
                //DateTime lastUpdate=Convert.ToDateTime("14/05/2020");
                using (MySqlConnection conn=new MySqlConnection(connStr)){
                    conn.Open();
                    foreach (string lgMatch in AllMatches){
                        string[] cells=lgMatch.Split(',');
                        string[] cDatePts=cells[1].Split("-");
                        string newDate=(cDatePts[2]+"/"+cDatePts[1]+"/"+cDatePts[0]).Replace("'",string.Empty);
                        DateTime thisFxDt=Convert.ToDateTime(newDate);
                        //if (thisFxDt.CompareTo(lastUpdate)>=0){
                            Console.WriteLine("Checking/writing: ");
                            Console.WriteLine(lgMatch);
                            string sql2="REPLACE INTO results_and_odds VALUES ("+lgMatch+")";
                            MySqlCommand cmd=new MySqlCommand(sql2,conn);
                            cmd.ExecuteNonQuery();
                        //}
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
            }*/
            //**************************************************************************

            
            //**************************************************************************

            
            /*string oldLine = "E1,28/05/2020,Nottm Forest,Brentford,,,2.2,3.4,2.3";
            string newLine = ModMatchLine(oldLine, false);
            Console.WriteLine(oldLine);
            Console.WriteLine(newLine);*/

        }
    }
}