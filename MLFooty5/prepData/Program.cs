using System;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace prepData
{
    class Program
    {
        static string ModifiedLine(string rawLine){
            //IN # league,match_date,home_team,away_team,fthg,ftag,ftr,hthg,htag,htr,home_shots,away_shots,home_shots_target,away_shots_target,home_corners,away_corners,home_yellow,away_yellow,home_red,away_red,home_win_odds,draw_odds,away_win_odds
            //     0     ,1         ,2        ,3        ,4   ,5   ,6  ,7   ,8   ,9  ,10        ,11        ,12               ,13               ,14          ,15          ,16         ,17         ,18      ,19      ,20           ,21       ,22
            string[] cells = rawLine.Split(',');
        }
        static double[] TeamData(string team, bool atHome, string strDate){
            string sqlDate = strDate.ToString("yyyy-MM-dd");
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
            double av_pld = 0;  

            string connStr="server=localhost;user=simon;database=football;port=3306;password=chainsaw";
            string sql = "SELECT * FROM wide_results WHERE {whichTeam} = '{team}' AND match_date < '{sqlDate}' ORDER BY match_date DESC LIMIT 10";
            using MySqlConnection con = new MySqlConnection(connStr);
            con.Open();
            using MySqlCommand cmd = new MySqlCommand(sql, con);
            using MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read()){
                TimeSpan span = (Convert.ToDateTime(strDate) - Convert.ToDateTime(rdr.GetValue(1)));
                int daySpan = span.Days;
                double myExp = Math.Exp(-0.007 * daySpan);
                av_ftG_for += (myExp * rdr.GetValue(4 + adj));
                av_ftG_con += (myExp * rdr.GetValue(5 - adj));
                av_htG_for += (myExp * rdr.GetValue(7 + adj));
                av_htG_con += (myExp * rdr.GetValue(8 - adj));
                av_shots_for += (myExp * rdr.GetValue(10 + adj));
                av_shots_con += (myExp * rdr.GetValue(11 - adj));
                av_shots_target_for += (myExp * rdr.GetValue(12 + adj));
                av_shots_target_con += (myExp * rdr.GetValue(13 - adj));
                av_corners_for += (myExp * rdr.GetValue(14 + adj));
                av_corners_con += (myExp * rdr.GetValue(15 - adj));
                av_yellow_for += (myExp * rdr.GetValue(16 + adj));
                av_yellow_con += (myExp * rdr.GetValue(17 -adj));
                av_red_for += (myExp * rdr.GetValue(18 + adj));
                av_red_con += (myExp * rdr.GetValue(19 - adj));
                av_pld += myExp;
            }
            con.Close();
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
            
            double[] thisTeamData = new double[14];
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

            return thisTeamData;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
