using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Microsoft.Data.Analysis;

namespace Cs
{
    class Program
    {
        static string GetCsvLine(DataFrame df, int i){
            string line = "";
            for (int j = 0; j < df.Columns.Count; j++){
                if (j != 0){
                    line += ",";
                }
                line += Convert.ToString(df[i, j]);
            }
            return line;
        }
        static string[] TeamData(string team, DataFrame df, int matchInd, bool atHome){
            DateTime currentMatchDt = Convert.ToDateTime(df[matchInd, 1]);
            //string ha = "home_team";
            int adj = 0;
            /*if (atHome == false){
                ha = "away_team";
                adj = 1;
            }*/
            //declare and fetch averages fo last x matches
            int x = 10;
            int prevCt = 0;
            //averages goals scored and conceded, av wins, draws and losses
            double gs = 0.0; double gc = 0.0; double wn = 0.0; double dr = 0.0; double ls = 0.0; double avPld = 0.0;
            //Streaks: win; winless; draw; drawless; losing; lossless
            bool wStreak = true; bool wxStreak = true; bool dStreak = true; bool dxStreak = true; bool lStreak = true; bool lxStreak = true;
            double wStreakCt = 0.0; double wxStreakCt = 0.0; double dStreakCt =0.0; double dxStreakCt = 0.0; double lStreakCt = 0.0; double lxStreakCt = 0.0;
            for (int i = matchInd - 1; i >= 0; i--){
                if (prevCt < 10 && Convert.ToString(df[i, 2 + adj]) == team){
                    DateTime prevMatchDt = Convert.ToDateTime(df[i, 1]);
                    int delta = (currentMatchDt - prevMatchDt).Days;
                    double myExp = Math.Exp(delta * -0.007);
                    int glsFor = Convert.ToInt32(df[i, 4 + adj]);
                    int glsAg = Convert.ToInt32(df[i, 5 - adj]);
                    gs += (glsFor * myExp);
                    gc += (glsAg * myExp);
                    //WIN
                    if (glsFor > glsAg){ 
                        wn += myExp;
                        if (wStreak == true){
                            wStreakCt += 1;
                        }
                        if (dxStreak == true){
                            dxStreakCt += 1;
                        }
                        if (lxStreak == true){
                            lxStreakCt += 1;
                        }
                        wxStreak = false;
                        dStreak = false;
                        lStreak = false;
                    }else if (glsFor == glsAg){  //DRAW
                        dr += myExp;
                        if (wxStreak == true){
                            wxStreakCt += 1;
                        }
                        if (dStreak == true){
                            dStreakCt += 1;
                        }
                        if (lxStreak == true){
                            lxStreakCt += 1;
                        }
                        wStreak = false;
                        dxStreak = false;
                        lStreak = false;
                    }else{ 
                        ls += myExp;
                        if (wxStreak == true){
                            wxStreakCt += 1;
                        }
                        if (dxStreak == true){
                            dxStreakCt += 1;
                        }
                        if (lStreak == true){
                            lStreakCt += 1;
                        }
                        wStreak = false;
                        dStreak = false;
                        lxStreak = false;
                    }
                    avPld += myExp;
                    prevCt++;
                }
                if (prevCt >= 10){
                    break;
                }
            }
            if (prevCt == x){
                gs /= avPld; gc /= avPld; wn /= avPld; dr /= avPld; ls /=avPld;
                wStreakCt /= x; wxStreakCt /= x;
                dStreakCt /= x; dxStreakCt /= x;
                lStreakCt /= x; lxStreakCt /= x;
                string[] stats = {gs.ToString(), gc.ToString(), wn.ToString(), dr.ToString(), ls.ToString(), wStreakCt.ToString(), wxStreakCt.ToString(), dStreakCt.ToString(), dxStreakCt.ToString(), lStreakCt.ToString(), lxStreakCt.ToString()};
                return stats;
            }else{
                string[] stats = new string[1];
                stats[0] = "-1.0";
                return stats;
            }
        }
        static void Main(string[] args)
        {
           //get row count from sql
            string connStr = "server = localhost; user = simon; database = football; port = 3306; password = chainsaw";
            string sql = "SELECT COUNT(*) FROM results_and_odds;";
            MySqlConnection con = new MySqlConnection(connStr);
            con.Open();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            int rowCt = 0;
            while (rdr.Read()){
                rowCt = Convert.ToInt32(rdr.GetValue(0));
            }
            con.Close();
            Console.WriteLine(rowCt);

            //set up dataframe
            StringDataFrameColumn league = new StringDataFrameColumn("League", rowCt);
            PrimitiveDataFrameColumn<DateTime> date = new PrimitiveDataFrameColumn<DateTime> ("Date", rowCt);
            StringDataFrameColumn homeTeam = new StringDataFrameColumn("HomeTeam", rowCt);
            StringDataFrameColumn awayTeam = new StringDataFrameColumn("AwayTeam", rowCt);
            PrimitiveDataFrameColumn<int> fthg = new PrimitiveDataFrameColumn<int>("Fthg", rowCt);
            PrimitiveDataFrameColumn<int> ftag = new PrimitiveDataFrameColumn<int>("Ftag", rowCt);
            PrimitiveDataFrameColumn<double> homeOdds = new PrimitiveDataFrameColumn<double>("HomeOdds", rowCt);
            PrimitiveDataFrameColumn<double> drawOdds = new PrimitiveDataFrameColumn<double>("DrawOdds", rowCt);
            PrimitiveDataFrameColumn<double> awayOdds = new PrimitiveDataFrameColumn<double>("AwayOdds", rowCt);

            DataFrame allMatches = new DataFrame(league, date, homeTeam, awayTeam, fthg, ftag, homeOdds, drawOdds, awayOdds);

            //put sql values into dataframe
            int thisRow = 0;
            sql = "SELECT * FROM results_and_odds;";
            con.Open();
            cmd = new MySqlCommand(sql, con);
            rdr = cmd.ExecuteReader();
            while (rdr.Read()){
               league[thisRow] = rdr.GetString(0); //Convert.ToString(rdr.GetValue(0));
               date[thisRow] = rdr.GetDateTime(1); //Convert.ToDateTime(rdr.GetValue(1));
               homeTeam[thisRow] = rdr.GetString(2); //Convert.ToString(rdr.GetValue(2));
               awayTeam[thisRow] = rdr.GetString(3); //Convert.ToString(rdr.GetValue(3));
               fthg[thisRow] = rdr.GetInt32(4); //Convert.ToInt32(rdr.GetValue(4));
               ftag[thisRow] = rdr.GetInt32(5); //Convert.ToInt32(rdr.GetValue(5));
               homeOdds[thisRow] = rdr.GetDouble(6); //Convert.ToDouble(rdr.GetValue(6));
               drawOdds[thisRow] = rdr.GetDouble(7); //Convert.ToDouble(rdr.GetValue(7));
               awayOdds[thisRow] = rdr.GetDouble(8); //Convert.ToDouble(rdr.GetValue(8));
               thisRow++;
            }

            //TEST EXAMPLE
            string myTeam = "Nottm Forest";
            string[] myData = TeamData(myTeam, allMatches, 100000, true);
            Console.WriteLine(myTeam);
            for (int i = 0; i < myData.Length; i++){
                Console.WriteLine(myData[i]);
            }

            List<string> AllProcessedMatches = new List<string>();
            for (int i = 0; i < rowCt; i++){
                string hmTm = Convert.ToString(allMatches[i, 2]);
                string awTm = Convert.ToString(allMatches[i, 3]);
                string[] homeTeamData = TeamData(hmTm, allMatches, i, true);
                if (homeTeamData.Length > 1){ //if home team have played more than x matches (10 initially)
                    string[] awayTeamData = TeamData(awTm, allMatches, i, false);
                    if (awayTeamData.Length > 1){
                        //other project
                        string dataLine = $"{allMatches[i, 0]},{allMatches[i, 1]},{hmTm},{awTm},";
                        dataLine += (string.Join(",", homeTeamData)) + ",";
                        dataLine += (string.Join(",", awayTeamData)) + ",";
                        int hmGls = Convert.ToInt32(allMatches[i, 4]);
                        int awGls = Convert.ToInt32(allMatches[i, 5]);
                        if (hmGls > awGls){
                            dataLine += "H";
                        }else if (hmGls == awGls){
                            dataLine += "D";
                        }else{
                            dataLine += "A";
                        }
                        AllProcessedMatches.Add(dataLine);
                    }
                }
            }

            //write to file
            string fName = "../Data/matchData.csv";
            if (File.Exists(fName)){
                File.Delete(fName);
            }
            using (StreamWriter sw = new StreamWriter(fName)){

            }
        }
    }
}
