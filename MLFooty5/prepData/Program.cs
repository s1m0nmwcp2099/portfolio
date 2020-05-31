using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;

namespace prepData
{
    class Program
    {
        static string ModifiedLine(string rawLine){
            //IN # league,match_date,home_team,away_team,fthg,ftag,ftr,hthg,htag,htr,home_shots,away_shots,home_shots_target,away_shots_target,home_corners,away_corners,home_yellow,away_yellow,home_red,away_red,home_win_odds,draw_odds,away_win_odds
            //     0     ,1         ,2        ,3        ,4   ,5   ,6  ,7   ,8   ,9  ,10        ,11        ,12               ,13               ,14          ,15          ,16         ,17         ,18      ,19      ,20           ,21       ,22       
            string[] cells = rawLine.Split(',');
            string modLine = "";
            for (int i=0;i<=3;i++){
                if (i != 0){
                    modLine += ",";
                }
                modLine += cells[i];
            }
            double[] home_data = TeamData(cells[2].Replace("'",string.Empty), true, cells[1]);
            for (int i=0;i<home_data.Length;i++){
                modLine += ",";
                modLine += Convert.ToString(home_data[i]);
            }
            if (home_data[0] == -1){
                cells[0] = "nullMatch";
            }
            double[] away_data = TeamData(cells[3].Replace("'",string.Empty), false, cells[1]);
            for (int i=0;i<away_data.Length;i++){
                modLine += ",";
                modLine += Convert.ToString(away_data[i]);
            }
            if (away_data[0] == -1){
                cells[0] = "nullMatch";
            }
            if (cells[0] == "nullMatch"){
                return "Null match";
            }else{
                modLine += ($",{cells[20]},{cells[21]},{cells[22]},{cells[6]}");
                return modLine;
            }
        }
        static double[] TeamData(string team, bool atHome, string strDate){
            DateTime match_date = Convert.ToDateTime(strDate);
            string sqlDate = match_date.ToString("yyyy-MM-dd");
            List<string> PrevMatches = new List<string>();
            int adj = 0; //for home or away. Set to home
            string whichTeam = "";
            if (atHome == true){
                whichTeam = "home_team";
            }else{
                whichTeam = "away_team";
                adj = 1; //Set to away
            }

            double av_ftG_for = 0;      double av_ftG_con = 0;      double av_htG_for = 0;     double av_htG_con = 0;
            double av_shots_for = 0;    double av_shots_con = 0;    double av_shots_target_for = 0; double av_shots_target_con = 0;
            double av_corners_for = 0;  double av_corners_con = 0;
            double av_yellow_for = 0;   double av_yellow_con = 0;   double av_red_for = 0;     double av_red_con = 0;
            double av_ft_wins = 0;      double av_ft_draws = 0;     double av_ft_losses = 0;
            double av_ht_wins = 0;      double av_ht_draws = 0;     double av_ht_losses = 0;
            double av_pld = 0;
            double[] streak_lengths = new double[6]; //0=win,1=winless,2=draw,3=drawless,4=loss,5=lossless
            bool[] streak = new bool[6];
            for (int i=0;i<6;i++){
                streak_lengths[i] = 0;
                streak[i] = true;
            }

            string connStr="server=localhost;user=simon;database=football;port=3306;password=chainsaw";
            string sql = $"SELECT * FROM wide_results WHERE {whichTeam} = '{team}' AND match_date < '{sqlDate}' ORDER BY match_date DESC LIMIT 10";
            using MySqlConnection con = new MySqlConnection(connStr);
            con.Open();
            using MySqlCommand cmd = new MySqlCommand(sql, con);
            using MySqlDataReader rdr = cmd.ExecuteReader();
            int actualPld = 0;
            while (rdr.Read()){
                actualPld ++;
                TimeSpan span = (Convert.ToDateTime(strDate) - Convert.ToDateTime(rdr.GetValue(1)));
                int daySpan = span.Days;
                double myExp = Math.Exp(-0.007 * daySpan);
                av_ftG_for += (myExp * Convert.ToDouble(rdr.GetValue(4 + adj)));    //average fulltime goals per game
                av_ftG_con += (myExp * Convert.ToDouble(rdr.GetValue(5 - adj)));    
                av_htG_for += (myExp * Convert.ToDouble(rdr.GetValue(7 + adj)));    //average halftime goals pg
                av_htG_con += (myExp * Convert.ToDouble(rdr.GetValue(8 - adj)));
                av_shots_for += (myExp * Convert.ToDouble(rdr.GetValue(10 + adj))); //av shots pg
                av_shots_con += (myExp * Convert.ToDouble(rdr.GetValue(11 - adj)));
                av_shots_target_for += (myExp * Convert.ToDouble(rdr.GetValue(12 + adj)));  //av shots on target pg
                av_shots_target_con += (myExp * Convert.ToDouble(rdr.GetValue(13 - adj)));
                av_corners_for += (myExp * Convert.ToDouble(rdr.GetValue(14 + adj)));   //av corners pg
                av_corners_con += (myExp * Convert.ToDouble(rdr.GetValue(15 - adj)));   
                av_yellow_for += (myExp * Convert.ToDouble(rdr.GetValue(16 + adj)));    //av yellow cards pg
                av_yellow_con += (myExp * Convert.ToDouble(rdr.GetValue(17 -adj)));
                av_red_for += (myExp * Convert.ToDouble(rdr.GetValue(18 + adj)));       //av red cards pg
                av_red_con += (myExp * Convert.ToDouble(rdr.GetValue(19 - adj)));
                //Full time wins, draws, losses and streaks
                if (Convert.ToString(rdr.GetValue(6)) == "H"){
                    if (atHome == true){
                        av_ft_wins += myExp;
                        if (streak[0] == true){
                            streak_lengths[0] += 1;
                        }
                        if (streak[3] == true){
                            streak_lengths[3] += 1;
                        }
                        if (streak[5] == true){
                            streak_lengths[5] += 1;
                        }
                        streak[1] = false;
                        streak[2] = false;
                        streak[4] = false;
                    }else{
                        av_ft_losses += myExp;
                        if (streak[1] == true){
                            streak_lengths[1] += 1;
                        }
                        if (streak[3] == true){
                            streak_lengths[3] += 1;
                        }
                        if (streak[4] == true){
                            streak_lengths[4] += 1;
                        }
                        streak[0] = false;
                        streak[2] = false;
                        streak[5] = false;
                    }
                }else if (Convert.ToString(rdr.GetValue(6)) == "D"){
                    av_ft_draws += myExp;
                        if (streak[1] == true){
                            streak_lengths[1] += 1;
                        }
                        if (streak[2] == true){
                            streak_lengths[2] += 1;
                        }
                        if (streak[5] == true){
                            streak_lengths[5] += 1;
                        }
                        streak[0] = false;
                        streak[3] = false;
                        streak[4] = false;
                }else if (Convert.ToString(rdr.GetValue(6)) == "A"){
                    if (atHome == false){
                        av_ft_wins += myExp;
                        if (streak[0] == true){
                            streak_lengths[0] += 1;
                        }
                        if (streak[3] == true){
                            streak_lengths[3] += 1;
                        }
                        if (streak[5] == true){
                            streak_lengths[5] += 1;
                        }
                        streak[1] = false;
                        streak[2] = false;
                        streak[4] = false;
                    }else{
                        av_ft_losses += myExp;
                        if (streak[1] == true){
                            streak_lengths[1] += 1;
                        }
                        if (streak[3] == true){
                            streak_lengths[3] += 1;
                        }
                        if (streak[4] == true){
                            streak_lengths[4] += 1;
                        }
                        streak[0] = false;
                        streak[2] = false;
                        streak[5] = false;
                    }
                }
                //Half time time wins, draws, losses and streaks
                if (Convert.ToString(rdr.GetValue(9)) == "H"){
                    if (atHome == true){
                        av_ht_wins += myExp;
                    }else{
                        av_ht_losses += myExp;
                    }
                }else if (Convert.ToString(rdr.GetValue(9)) == "D"){
                    av_ht_draws += myExp;
                }else if (Convert.ToString(rdr.GetValue(9)) == "A"){
                    if (atHome == false){
                        av_ht_wins += myExp;
                    }else{
                        av_ht_losses += myExp;
                    }
                }
                
                av_pld += myExp;
            }
            con.Close();
            double[] thisTeamData = new double[27];
            if (actualPld < 5){
                for (int i=0;i<thisTeamData.Length;i++){
                    thisTeamData[i] = -1;
                }
            }else{
                av_ftG_for /= av_pld;
                av_ftG_con /= av_pld;
                av_htG_for /= av_pld;
                av_htG_con /= av_pld;
                av_shots_for /= av_pld;
                av_shots_con /= av_pld;
                av_shots_target_for /= av_pld;
                av_shots_target_con /= av_pld;
                av_corners_for /= av_pld;
                av_corners_con /= av_pld;
                av_yellow_for /= av_pld;
                av_yellow_con /= av_pld;
                av_red_for /= av_pld;
                av_red_con /= av_pld;
                av_ft_wins /= av_pld;
                av_ft_draws /= av_pld;
                av_ft_losses /= av_pld;
                av_ht_wins /= av_pld;
                av_ht_draws /= av_pld;
                av_ht_losses /= av_pld;
                
                
                thisTeamData[0] = av_ftG_for;
                thisTeamData[1] = av_ftG_con;
                thisTeamData[2] = av_htG_for;
                thisTeamData[3] = av_htG_con;
                thisTeamData[4] = av_shots_for;
                thisTeamData[5] = av_shots_con;
                thisTeamData[6] = av_shots_target_for;
                thisTeamData[7] = av_shots_target_con;
                thisTeamData[8] = av_corners_for;
                thisTeamData[9] = av_corners_con;
                thisTeamData[10] = av_yellow_for;
                thisTeamData[11] = av_yellow_con;
                thisTeamData[12] = av_red_for;
                thisTeamData[13] = av_red_con; 
                thisTeamData[14] = av_ft_wins;
                thisTeamData[15] = av_ft_draws;
                thisTeamData[16] = av_ft_losses;
                for (int i=17; i<=22;i++){
                    thisTeamData[i] = streak_lengths[i - 17];
                }
                thisTeamData[23] = av_ht_wins;
                thisTeamData[24] = av_ht_draws;
                thisTeamData[25] = av_ht_losses;
                thisTeamData[26] = Convert.ToDouble(actualPld);
            }
            return thisTeamData;
        }
        static string ModifiedFixtureLine(string rawLine){
            string[] cells = rawLine.Split(',');
            string modLine = "";
            for (int i=0;i<=4;i++){
                if (i != 2){
                    if (i != 0){
                        modLine += ",";
                    }
                    modLine += cells[i];
                }
            }
            double[] home_data = TeamData(cells[3].Replace("'",string.Empty), true, cells[1]);
            for (int i=0;i<home_data.Length;i++){
                modLine += ",";
                modLine += Convert.ToString(home_data[i]);
            }
            if (home_data[0] == -1){
                cells[0] = "nullMatch";
            }
            double[] away_data = TeamData(cells[4].Replace("'",string.Empty), false, cells[1]);
            for (int i=0;i<away_data.Length;i++){
                modLine += ",";
                modLine += Convert.ToString(away_data[i]);
            }
            if (away_data[0] == -1){
                cells[0] = "nullMatch";
            }
            if (cells[0] == "nullMatch"){
                return "Null match";
            }else{
                modLine += ($",{cells[11]},{cells[12]},{cells[13]}");
                return modLine;
            }
        }
        static void Main(string[] args)
        {
            /*
            //EXTRACT ALL RESULTS FROM SQL AND PUT INTO LIST
            List<string> AllPreviousMatches = new List<string>();
            string connStr="server=localhost;user=simon;database=football;port=3306;password=chainsaw";
            string sql = $"SELECT * FROM wide_results";
            Console.WriteLine("Extracting data from sql");
            using MySqlConnection con = new MySqlConnection(connStr);
            con.Open();
            using MySqlCommand cmd = new MySqlCommand(sql, con);
            using MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read()){
                string rawLine = "";
                for (int i=0;i<23;i++){
                    if (i != 0){
                        rawLine += ",";
                    }
                    rawLine += Convert.ToString(rdr.GetValue(i));
                }
                AllPreviousMatches.Add(rawLine);
            }
            con.Close();

            //PROCESS DATA
            List<string> ModifiedMatches = new List<string>();
            foreach (string x in AllPreviousMatches){
                //Console.WriteLine(x);
                string newLine = ModifiedLine(x);
                //string[] cells = newLine.Split(',');
                if (newLine != "Null match"){
                    ModifiedMatches.Add(newLine);
                    //Console.WriteLine(newLine);
                    if (ModifiedMatches.Count % 1000 == 0){
                        Console.WriteLine($"{ModifiedMatches.Count} processed");
                    }
                }
            }
            string fname = "../Data/processedData1.csv";
            if (File.Exists(fname)){
                File.Delete(fname);
            }
            using (StreamWriter sw = new StreamWriter(fname)){
                sw.WriteLine("league,match_date,home_team,away_team,av_hm_ftG_for,av__hm_ftG_con,av_hm_htG_for,av_hm_htG_con,av_hm_shots_for,av_hm_shots_con,av_hm_shots_target_for,av_hm_shots_target_con,av_hm_corners_for,av_hm_corners_con,av_hm_yellow_for,av_hm_yellow_con,av_hm_red_for,av_hm_red_con,av_hm_ft_wins,av_hm_ft_draws,av_hm_ft_losses,av_hm_win_streak,av_hm_winless_streak,av_hm_draw_streak,av_hm_drawless_streak,av_hm_loss_streak,av_hm_lossless_streak,av_hm_ht_wins,av_hm_ht_draws,av_hm_ht_losses,av_hm_pld,av_aw_ftG_for,av__aw_ftG_con,av_aw_htG_for,av_aw_htG_con,av_aw_shots_for,av_aw_shots_con,av_aw_shots_target_for,av_aw_shots_target_con,av_aw_corners_for,av_aw_corners_con,av_aw_yellow_for,av_aw_yellow_con,av_aw_red_for,av_aw_red_con,av_aw_ft_wins,av_aw_ft_draws,av_aw_ft_losses,av_aw_win_streak,av_aw_winless_streak,av_aw_draw_streak,av_aw_drawless_streak,av_aw_loss_streak,av_aw_lossless_streak,av_aw_ht_wins,av_aw_ht_draws,av_aw_ht_losses,av_aw_pld,home_odds,draw_odds,_away_odds,ftr");
                foreach (string modMatch in ModifiedMatches){
                    sw.WriteLine(modMatch);
                }
            }
            */

            //NORMALISE DATA
            //0=lg, 1=dt, 2=hm, 3=aw,
            //FOR HOME TEAM
            //4=avftfor, 5=avftag, 6=avhtf, 7=avhtag 8=avshf,9=avshag, 10=avshTf, 11=avshTag, 12=avcf, 13=avcag, 14=avyf, 15=avyag
            //       16=avrf, 17=avrag, 18=win, 19=draw, 20=loss
            //       21=wstrk, 22=noWstrk, 23=dstrk, 24=noDstrk, 25=lstrk, 26=nolstrk
            //       27=htw, 28=htd, 29=htl, 
            //       30=pld
            //AWAY TEAM
            //31=avftfor, 32=avftfag, 33=avhtf, 34=avhtag 35=avshf, 36=avshag, 37=avshTf, 38=avshTag, 39=avcf, 40=avcag, 41=avyf, 42=avyag
            //       43=avrf, 44=avrag, 45=win, 46=draw, 47=loss
            //       48=wstrk, 49=noWstrk, 50=dstrk, 51=noDstrk, 52=lstrk, 53=nolstrk
            //       54=htw, 55=htd, 56=htl, 
            //       57=pld
            //58=hmodds, 59=drodds, 60=awodds
            //61=ftr

            //0=league, 1=match_date, 2=home_team, 3=away_team, 4=av_hm_ftG_for, 5=av__hm_ftG_con, 6=av_hm_htG_for, 7=av_hm_htG_con,
                //8=av_hm_shots_for, 9=av_hm_shots_con, 10=av_hm_shots_target_for, 11=av_hm_shots_target_con, 12=av_hm_corners_for,
                //13=av_hm_corners_con, 14=av_hm_yellow_for, 15=av_hm_yellow_con, 16=av_hm_red_for, 17=av_hm_red_con, 18=av_hm_ft_wins,
                //19=av_hm_ft_draws, 20=av_hm_ft_losses, 21=av_hm_win_streak, 22=av_hm_winless_streak, 23=av_hm_draw_streak,
                //24=av_hm_drawless_streak, 25=av_hm_loss_streak, 26=av_hm_lossless_streak, 27=av_hm_ht_wins, 28=av_hm_ht_draws,
                //29=av_hm_ht_losses, 30=av_hm_pld, 31=av_aw_ftG_for, 32=av__aw_ftG_con, 33=av_aw_htG_for, 34=av_aw_htG_con,
                //35=av_aw_shots_for, 36=av_aw_shots_con, 37=av_aw_shots_target_for, 38=av_aw_shots_target_con, 39=av_aw_corners_for,
                //40av_aw_corners_con, 41=av_aw_yellow_for, 42=av_aw_yellow_con, 43=av_aw_red_for, 44=av_aw_red_con, 45=av_aw_ft_wins,
                //46=av_aw_ft_draws, 47=av_aw_ft_losses, 48=av_aw_win_streak, 49=av_aw_winless_streak, 50=av_aw_draw_streak,
                //51=av_aw_drawless_streak, 52=av_aw_loss_streak, 53=av_aw_lossless_streak, 54=av_aw_ht_wins, 55=av_aw_ht_draws, 
                //56=av_aw_ht_losses, 57=av_aw_pld, 58=home_odds, 59=draw_odds, 60=away_odds, 61=ftr

            //TEMPORARY TO AVOID REPROCESSING
            string fname = "../Data/processedData1.csv";
            List<string> ModifiedMatches = new List<string>();
            using (StreamReader sr = new StreamReader(fname)){
                while (sr.Peek() > 0){
                    ModifiedMatches.Add(sr.ReadLine());
                }
            }
            //****************************

            int[] noNorm = {0,1,2,3,18,19,20,27,28,29,30,45,46,47,54,55,56,57,61};
            double[] maximums = new double[62];
            for (int i=0;i<62;i++){
                maximums[i] = 0;
            }
            

            //get maximums
            foreach (string match in ModifiedMatches){
                string[] cells = match.Split(',');
                if (cells[0] != "league"){
                    for (int i=0;i<62;i++){
                        if (!noNorm.Contains(i)){
                            if (Convert.ToDouble(cells[i]) > maximums[i]){
                                maximums[i] = Convert.ToDouble(cells[i]);
                            }
                        }
                    }
                }
            }

            //normalise
            List<string> NormalisedModifiedMatches = new List<string>();
            foreach (string match in ModifiedMatches){
                string[] cells = match.Split(',');
                string newLine = "";
                for (int i=0;i<62;i++){
                    if (i != 0){
                        newLine += ",";
                    }
                    if (noNorm.Contains(i)){
                        newLine += cells[i];
                    }else{
                        newLine += Convert.ToString(Convert.ToDouble(cells[i]) / Convert.ToDouble(maximums[i]));
                    }
                }
                NormalisedModifiedMatches.Add(newLine);
            }
            //write normalised data to file
            fname = "../Data/normalisedProcessedData1.csv";
            using (StreamWriter sw = new StreamWriter(fname)){
                sw.WriteLine("league,match_date,home_team,away_team,av_hm_ftG_for,av__hm_ftG_con,av_hm_htG_for,av_hm_htG_con,av_hm_shots_for,av_hm_shots_con,av_hm_shots_target_for,av_hm_shots_target_con,av_hm_corners_for,av_hm_corners_con,av_hm_yellow_for,av_hm_yellow_con,av_hm_red_for,av_hm_red_con,av_hm_ft_wins,av_hm_ft_draws,av_hm_ft_losses,av_hm_win_streak,av_hm_winless_streak,av_hm_draw_streak,av_hm_drawless_streak,av_hm_loss_streak,av_hm_lossless_streak,av_hm_ht_wins,av_hm_ht_draws,av_hm_ht_losses,av_hm_pld,av_aw_ftG_for,av__aw_ftG_con,av_aw_htG_for,av_aw_htG_con,av_aw_shots_for,av_aw_shots_con,av_aw_shots_target_for,av_aw_shots_target_con,av_aw_corners_for,av_aw_corners_con,av_aw_yellow_for,av_aw_yellow_con,av_aw_red_for,av_aw_red_con,av_aw_ft_wins,av_aw_ft_draws,av_aw_ft_losses,av_aw_win_streak,av_aw_winless_streak,av_aw_draw_streak,av_aw_drawless_streak,av_aw_loss_streak,av_aw_lossless_streak,av_aw_ht_wins,av_aw_ht_draws,av_aw_ht_losses,av_aw_pld,home_odds,draw_odds,away_odds,ftr");
                foreach (string match in NormalisedModifiedMatches){
                    sw.WriteLine(match);
                }
            }
            
            //*********************************
            //DOWNLOAD FIXTURES
            string filename="../Data/fixtures.csv";
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
            //FIXTURES TO LIST
            List<string> RawFixtures = new List<string>();
            using (StreamReader sr = new StreamReader(filename)){
                while (sr.Peek() > 0){
                    RawFixtures.Add(sr.ReadLine());
                }
            }
            //PROCESS FIXTURES
            List<string> ModifiedFixtures = new List<string>();
            foreach (string fx in RawFixtures){
                string[] cells = fx.Split(',');
                if (cells[0] != "Div"){
                    string newLine = ModifiedFixtureLine(fx);
                    if (newLine != "Null match"){
                        ModifiedFixtures.Add(newLine);
                    }
                }
            }
            //NORMALISE PROCESSED FIXTURES
            for (int i=0;i<62;i++){
                maximums[i] = 0;
            }
            

            //get maximums
            foreach (string match in ModifiedFixtures){
                string[] cells = match.Split(',');
                if (cells[0] != "league"){
                    for (int i=0;i<62;i++){
                        if (!noNorm.Contains(i)){
                            if (Convert.ToDouble(cells[i]) > maximums[i]){
                                maximums[i] = Convert.ToDouble(cells[i]);
                            }
                        }
                    }
                }
            }
            //normalise
            List<string> NormalisedModifiedFixtures = new List<string>();
            foreach (string match in ModifiedFixtures){
                string[] cells = match.Split(',');
                string newLine = "";
                for (int i=0;i<62;i++){
                    if (i != 0){
                        newLine += ",";
                    }
                    if (noNorm.Contains(i)){
                        newLine += cells[i];
                    }else{
                        newLine += Convert.ToString(Convert.ToDouble(cells[i]) / Convert.ToDouble(maximums[i]));
                    }
                }
                NormalisedModifiedFixtures.Add(newLine);
            }
            //write normalised processed fixtures to file
            fname = "../Data/normalisedProcessedFixtures.csv";
            using (StreamWriter sw = new StreamWriter(fname)){
                sw.WriteLine("league,match_date,home_team,away_team,av_hm_ftG_for,av__hm_ftG_con,av_hm_htG_for,av_hm_htG_con,av_hm_shots_for,av_hm_shots_con,av_hm_shots_target_for,av_hm_shots_target_con,av_hm_corners_for,av_hm_corners_con,av_hm_yellow_for,av_hm_yellow_con,av_hm_red_for,av_hm_red_con,av_hm_ft_wins,av_hm_ft_draws,av_hm_ft_losses,av_hm_win_streak,av_hm_winless_streak,av_hm_draw_streak,av_hm_drawless_streak,av_hm_loss_streak,av_hm_lossless_streak,av_hm_ht_wins,av_hm_ht_draws,av_hm_ht_losses,av_hm_pld,av_aw_ftG_for,av__aw_ftG_con,av_aw_htG_for,av_aw_htG_con,av_aw_shots_for,av_aw_shots_con,av_aw_shots_target_for,av_aw_shots_target_con,av_aw_corners_for,av_aw_corners_con,av_aw_yellow_for,av_aw_yellow_con,av_aw_red_for,av_aw_red_con,av_aw_ft_wins,av_aw_ft_draws,av_aw_ft_losses,av_aw_win_streak,av_aw_winless_streak,av_aw_draw_streak,av_aw_drawless_streak,av_aw_loss_streak,av_aw_lossless_streak,av_aw_ht_wins,av_aw_ht_draws,av_aw_ht_losses,av_aw_pld,home_odds,draw_odds,away_odds,ftr");
                foreach (string match in NormalisedModifiedFixtures){
                    sw.WriteLine(match);
                }
            }
        }
    }
}
