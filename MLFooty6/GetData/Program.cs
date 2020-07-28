using System;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Math;

namespace GetData
{
    class Program
    {
        static string[] GetTeamData(string team, bool atHm, DateTime matchDt){
            int adj = 0;
            string tm = "";
            if (atHm == True){
                tm = "home_team";
            }else{
                tm = "away_team";
                adj = 1;
            }
            DateTime prevMatchDt = new DateTime(1970, 1, 1);
            List<string> RecentResults = new List<string>();
            string matchSqlDt = matchDt.ToString("yyyy-MM-dd HH:mm:ss.fff")
            string sql = $"SELECT * FROM results_and_odds WHERE {tm} = '{team}' AND date < '{matchSqlDt}' ORDER BY date DESC LIMIT 10;";
            MySqlConnection con = new MySqlConnection(connStr);
            con.Open();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read()){
                string prevMatch = "";
                for (int i = 0; i < 9; i++){
                    prevMatch += (rdr.GetValue(i) + ",";
                }
                prevMatchDt = rdr.GetDateTime(1);
                int delta = (matchDt - prevMatchDt).TotalDays;
                prevMatch += Convert.ToString(delta)
                RecentResults.Add(prevMatch);
            }
            if (RecentResults.Count < 10){
                string[] x = {"x"};
                return x;
            }else{
                double avGlsSco = 0.0;
                double avGlsCon = 0.0;
                double avPld = 0.0;
                double avWins = 0.0;
                double avDraws = 0.0;
                double avLosses = 0.0;

                bool winningStreak = True;
                int winningCt = 0;
                bool winlessStreak = True;
                int winlessCt = 0;
                bool drawingStreak = True;
                int drawingCt = 0;
                bool drawlessStreak = True;
                int drawlessCt = 0;
                bool losingStreak = True;
                int losingCt = 0;
                bool losslessStreak = True;
                int losslessCt = 0;

                foreach (string match in RecentResults){
                    string[] parts = match.Split(',');
                    int delta = Convert.ToInt32(parts[9]);
                    double myExp = Math.Exp(delta * -0.007);
                    int goalsScored = Convert.ToInt32(parts[4 + adj]);
                    int goalsConceded = Convert.ToInt32(parts[5 - adj]);
                    avGlsSco += (goalsScored * myExp);
                    avGlsCon += (goalsConceded * myExp);
                    avPld += myExp;
                    if (goalsScored > goalsConceded){ //win
                        avWins += myExp;
                        if (winningStreak == True){
                            winningCt += 1;
                        }
                        if (drawlessStreak == True){
                            drawlessCt += 1;
                        }
                        if (losslessStreak == True){
                            losslessCt += 1;
                        }
                        winlessStreak = False;
                        drawingStreak = False;
                        losingStreak = False;
                    }
                    else if (goalsScored == goalsConceded){ //draw
                        avDraws += myExp;
                        if (winlessStreak == True){
                            winlessCt += 1;
                        }
                        if (drawingStreak == True){
                            drawingCt += 1;
                        }
                        if (losslessStreak == True){
                            losslessCt += 1;
                        }
                        winningStreak = False;
                        drawlessStreak = False;
                        losingStreak = False;
                    }
                    else{                         //loss
                        avLosses += myExp;
                        if (winlessStreak == True){
                            winlessCt += 1;
                        }
                        if (drawlessStreak == True){
                            drawlessCt += 1;
                        }
                        if (losingStreak == True){
                            losingCt += 1;
                        }
                        winningStreak = False;
                        drawingStreak = False;
                        losslessStreak = False;
                    }
                }
            }
        }

        static string connStr = "server = localhost; user = simon; database = football; port = 3306; password = chainsaw";

        static void Main(string[] args)
        {
            List<string> AllRawMatches = new List<string>();
            string sql = "SELECT * FROM results_and_odds;";
            MySqlConnection con = new MySqlConnection(connStr);
            con.Open();
            MySqlCommand cmd0 = new MySqlCommand(sql, con);
            MySqlDataReader rdr0 = cmd0.ExecuteReader();
            while (rdr0.Read()){
                string thisMatch = "";
                for (int i = 0; i < 9; i++){
                    if (i != 0){
                        thisMatch += ",";
                    }
                    thisMatch += (rdr0.GetValue(i));
                }
                AllRawMatches.Add(thisMatch);
                Console.WriteLine(thisMatch);
            }
            con.Close();

            //
        }
    }
}
