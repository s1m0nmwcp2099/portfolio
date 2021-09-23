using System;
using MySql.Data.MySqlClient;
using Microsoft.Data.Analysis;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;

namespace GenerateFixtureData
{
    class Program
    {
        static float[] AveragesPerGame(bool atHome, DataFrame df, string team, DateTime fixtureDt){
            float[] rpg = new float[3]; //result per game: 0=win; 1=draw; 2=loss
            float[] gpg = new float[2]; //goals per game: 0=scored per game; 1=conceded per game
            float[] gpst = new float[2]; //goals per shot on target: 0=for; 1= against
            float[] gps = new float[2]; //goals per shot: 0=for; 1=against
            float[] stpg = new float[2]; //shots on target per game
            float[] stps = new float[2]; //shots on target per shot
            float[] spg = new float[2]; //shots per game
            
            for (int i = 0; i < 3; i++){
                rpg[i] = 0f;
                if (i < 2){
                    gpg[i] = 0f;
                    gps[i] = 0f;
                    gpst[i] = 0f;
                    stpg[i] = 0f;
                    stps[i] = 0f;
                    spg[i] = 0f;
                }
            }
            int adj = 0; //this is to adjust dataframe column index depending on whether team is playing home or away
            if (atHome == false){
                adj = 1;
            }
            float played = 0F;
            int prevMatchCt = 0;
            for (int i = 0; i < df.Rows.Count; i++){
                if ((string)df[i, 2 + adj] == team){ //check when team played previous games
                    //Console.WriteLine(df.Rows[i]);
                    float myDeltaExp = MathF.Exp((fixtureDt - Convert.ToDateTime(df[i, 1])).Days * -0.007F); //time delay exponential
                    
                    //win draw or loss
             string[] whichTeam = { "HomeTeam", "AwayTeam" };       if ((string)df[i, 6] == "D"){ //check previous result draw
                        rpg[1] += myDeltaExp;
                    }else if ((string)df[i, 6] == "H"){
                        if (atHome == true){
                            rpg[0] += myDeltaExp;
                        }else{
                            rpg[2] += myDeltaExp;
                        }
                    }else if ((string)df[i, 6] == "A"){
                        if (atHome == true){
                            rpg[2] += myDeltaExp;
                        }else{
                            rpg[0] += myDeltaExp;
                        }
                    }

                    //goals per game
                    gpg[0] += ((int)df[i, 4 + adj] * myDeltaExp); //add time weighted home goals
                    gpg[1] += ((int)df[i, 5 - adj] * myDeltaExp); // -"- away goals

                    //goals per shot on target
                    gpst[0] += XperYcorrectZero((int)df[i, 4 + adj], (int)df[i, 9 + adj], myDeltaExp);
                    gpst[1] += XperYcorrectZero((int)df[i, 5 - adj], (int)df[i, 10 - adj], myDeltaExp);

                    //goals per shot
                    gps[0] += XperYcorrectZero((int)df[i, 4 + adj], (int)df[i, 7 + adj], myDeltaExp);
                    gps[1] += XperYcorrectZero((int)df[i, 5 - adj], (int)df[i, 8 - adj], myDeltaExp);

                    //shots on target per game
                    stpg[0] += ((int)df[i, 9 + adj] * myDeltaExp);
                    stpg[1] += ((int)df[i, 10 - adj] * myDeltaExp);

                    //shots on target per shot
                    stps[0] += XperYcorrectZero((int)df[i, 9 + adj], (int)df[i, 7 + adj], myDeltaExp);
                    stps[1] += XperYcorrectZero((int)df[i, 10 - adj], (int)df[i, 8 - adj], myDeltaExp);

                    //shots per game
                    spg[0] += ((int)df[i, 7 + adj] * myDeltaExp);
                    spg[1] += ((int)df[i, 8 - adj] * myDeltaExp);

                    played += myDeltaExp;

                    prevMatchCt++;
                }
            }
            
            //averages
            for (int i = 0; i < 3; i++){
                rpg[i] /= played;
                if (i < 2){
                    gpg[i] /= played;
                    gpst[i] /= played;
                    gps[i] /= played;
                    stpg[i] /= played;
                    stps[i] /= played;
                    spg[i] /= played;
                }
            }

            if (prevMatchCt >= 10){
                //join arrays
                return rpg.Concat(gpg).Concat(gpst).Concat(gps).Concat(stpg).Concat(stps).Concat(spg).ToArray();
                //     0,1,2,     3,4,        5,6,          7,8,         9,10,      11,12,       13,14
                //Console.ReadLine();
            }else{
                float[] dud = new float[15];
                for (int i = 0; i < 15; i++){
                    dud[i] = -1f;
                }
                return dud;
            }
            //return rpg.Concat(gpg).Concat(gpst).Concat(gps).Concat(stpg).Concat(stps).Concat(spg).ToArray();
        }
        static float XperYcorrectZero (int x, int y, float delta){
            if (y != 0f){
                return (Convert.ToSingle(x) / Convert.ToSingle(y) * delta);
            }else{
                return delta;
            }
        }
        static string connStr = "server = localhost; user = simon; database = football; port = 3306; password = chainsaw";

        static void Main(string[] args)
        {
            //open fixture file
            string fName = "../Data/fixtures.csv";
            if (File.Exists(fName)){
                List<string> Fixtures = new List<string>();
                using (StreamReader sr = new StreamReader(fName)){
                    while (sr.Peek() > 0){
                        Console.WriteLine(sr.ReadLine());
                        Fixtures.Add(sr.ReadLine());
                    }
                }
                //Console.ReadLine();

                /*
                //get last 10 matches from mysql and put into dataframe
                //COLUMNS FOR FIRST DATAFRAME
                StringDataFrameColumn thisDiv = new StringDataFrameColumn("ThisDiv", 0);
                PrimitiveDataFrameColumn<DateTime> date = new PrimitiveDataFrameColumn<DateTime>("Date", 0);
                StringDataFrameColumn homeTeam = new StringDataFrameColumn("HomeTeam", 0);
                StringDataFrameColumn awayTeam = new StringDataFrameColumn("AwayTeam", 0);
                PrimitiveDataFrameColumn<int> fthg = new PrimitiveDataFrameColumn<int>("FTHG", 0);
                PrimitiveDataFrameColumn<int> ftag = new PrimitiveDataFrameColumn<int>("FTAG", 0);
                StringDataFrameColumn ftr = new StringDataFrameColumn("FTR", 0);
                PrimitiveDataFrameColumn<int> hs = new PrimitiveDataFrameColumn<int>("HS", 0);
                PrimitiveDataFrameColumn<int> aws = new PrimitiveDataFrameColumn<int>("AwS", 0);
                PrimitiveDataFrameColumn<int> hst = new PrimitiveDataFrameColumn<int>("HST", 0);
                PrimitiveDataFrameColumn<int> awst = new PrimitiveDataFrameColumn<int>("AwST", 0);
                
                using (MySqlConnection conn = new MySqlConnection(connStr)){
                    //conn.Open();
                    for (int i = 1; i < Fixtures.Count; i++){
                        string[] cells = Fixtures[i].Split(',');
                        if (cells[0] != string.Empty){
                            string[] whichTeam = { "HomeTeam", "AwayTeam" };
                            for (int j = 0; j < 2; j++){
                                conn.Open();
                                string sql = $"SELECT ThisDiv, Date, HomeTeam, AwayTeam, FTHG, FTAG, FTR, HS, AwS, HST, AwST FROM football_data_complete WHERE ({whichTeam[j]} = '{cells[3 + j].Replace("'", string.Empty)}' AND FTHG > -1 AND FTAG > -1 AND HS > -1 AND AwS > -1 AND HST > -1 AND AwST > -1) ORDER BY Date LIMIT 10;";
                                Console.WriteLine(sql);
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
                                    hs.Append(rdr.GetInt32(7));
                                    aws.Append(rdr.GetInt32(8));
                                    hst.Append(rdr.GetInt32(9));
                                    awst.Append(rdr.GetInt32(10));
                                }
                                conn.Close();
                            }
                        }
                    }
                    //conn.Close();
                }
                //Console.ReadLine();
                DataFrame df = new DataFrame(thisDiv, date, homeTeam, awayTeam, fthg, ftag, ftr, hs, aws, hst, awst);
                Console.WriteLine(df.Info());
                Console.WriteLine(df.Sample(20));
                */
                
                
                StringDataFrameColumn thisDivFix = new StringDataFrameColumn("ThisDivFix", 0);
                PrimitiveDataFrameColumn<DateTime> dateFix = new PrimitiveDataFrameColumn<DateTime>("DateFix", 0);
                StringDataFrameColumn homeTeamFix = new StringDataFrameColumn("HomeTeamFix", 0);
                StringDataFrameColumn awayTeamFix = new StringDataFrameColumn("AwayTeamFix", 0);

                //CALCULATE
                PrimitiveDataFrameColumn<float> hwpg = new PrimitiveDataFrameColumn<float>("Hwpg", 0);    //home wins per game
                PrimitiveDataFrameColumn<float> hdpg = new PrimitiveDataFrameColumn<float>("Hdpg", 0);    //home draws pg
                PrimitiveDataFrameColumn<float> hlpg = new PrimitiveDataFrameColumn<float>("Hlpg", 0);    //home losses pg
                PrimitiveDataFrameColumn<float> awpg = new PrimitiveDataFrameColumn<float>("Awpg", 0);    //away wins per game
                PrimitiveDataFrameColumn<float> adpg = new PrimitiveDataFrameColumn<float>("Adpg", 0);    //away draws pg
                PrimitiveDataFrameColumn<float> alpg = new PrimitiveDataFrameColumn<float>("Alpg", 0);    //away losses pg
                
                PrimitiveDataFrameColumn<float> hgspg = new PrimitiveDataFrameColumn<float>("Hgspg", 0);  //home goals scored per game
                PrimitiveDataFrameColumn<float> hgcpg = new PrimitiveDataFrameColumn<float>("Hgcpg", 0);  //home goals conceded pg
                PrimitiveDataFrameColumn<float> agspg = new PrimitiveDataFrameColumn<float>("Agspg", 0);  //away goals scored pg
                PrimitiveDataFrameColumn<float> agcpg = new PrimitiveDataFrameColumn<float>("Agcpg", 0);  //away goals conceded pg
                
                PrimitiveDataFrameColumn<float> hgspst = new PrimitiveDataFrameColumn<float>("Hgspst", 0);  //home goals scored per shot on target
                PrimitiveDataFrameColumn<float> hgcpst = new PrimitiveDataFrameColumn<float>("Hgcpst", 0);  //home goals conceded per shot on target
                PrimitiveDataFrameColumn<float> agspst = new PrimitiveDataFrameColumn<float>("Agspst", 0);  //away goals scored per shot on target
                PrimitiveDataFrameColumn<float> agcpst = new PrimitiveDataFrameColumn<float>("Agcpst", 0);  //away goals conceded per shot on target

                PrimitiveDataFrameColumn<float> hgsps = new PrimitiveDataFrameColumn<float>("Hgsps", 0);  //home goals scored per shot
                PrimitiveDataFrameColumn<float> hgcps = new PrimitiveDataFrameColumn<float>("Hgcps", 0);  //home goals conceded per shot
                PrimitiveDataFrameColumn<float> agsps = new PrimitiveDataFrameColumn<float>("Agsps", 0);  //away goals scored per shot
                PrimitiveDataFrameColumn<float> agcps = new PrimitiveDataFrameColumn<float>("Agcps", 0);  //away goals conceded per shot
                
                PrimitiveDataFrameColumn<float> hsfpg = new PrimitiveDataFrameColumn<float>("Hsfpg", 0);  //home shots for per game
                PrimitiveDataFrameColumn<float> hsapg = new PrimitiveDataFrameColumn<float>("Hsapg", 0);  //home shots against per game
                PrimitiveDataFrameColumn<float> asfpg = new PrimitiveDataFrameColumn<float>("Asfpg", 0);  //away shots for pg
                PrimitiveDataFrameColumn<float> asapg = new PrimitiveDataFrameColumn<float>("Asapg", 0);  //away shots against pg

                PrimitiveDataFrameColumn<float> hstfpg = new PrimitiveDataFrameColumn<float>("Hstfpg", 0);  //home shots on target for per game
                PrimitiveDataFrameColumn<float> hstapg = new PrimitiveDataFrameColumn<float>("Hstapg", 0);  //home shots on target against per game
                PrimitiveDataFrameColumn<float> astfpg = new PrimitiveDataFrameColumn<float>("Astfpg", 0);  //away shots on target for pg
                PrimitiveDataFrameColumn<float> astapg = new PrimitiveDataFrameColumn<float>("Astapg", 0);  //away shots on target against pg

                PrimitiveDataFrameColumn<float> hstfps = new PrimitiveDataFrameColumn<float>("Hstfps", 0);  //home shots on target for per shot
                PrimitiveDataFrameColumn<float> hstaps = new PrimitiveDataFrameColumn<float>("Hstaps", 0);  //home shots on target against per shot
                PrimitiveDataFrameColumn<float> astfps = new PrimitiveDataFrameColumn<float>("Astfps", 0);  //away shots on target for ps
                PrimitiveDataFrameColumn<float> astaps = new PrimitiveDataFrameColumn<float>("Astaps", 0);  //away shots on target against ps

                PrimitiveDataFrameColumn<bool> rowValid = new PrimitiveDataFrameColumn<bool>("RowValid", 0);

                StringDataFrameColumn over = new StringDataFrameColumn("Over", 0);

                for (int x=0; x<Fixtures.Count(); x++){
                    Console.WriteLine(Fixtures[x]);
                }
                Console.ReadLine();

                for (int gm = 1; gm < Fixtures.Count; gm++){
                    Console.WriteLine(Fixtures[gm]);
                    try{
                        string[] cells = Fixtures[gm].Split(',');
                        //get last 10 matches from mysql and put into dataframe
                        //COLUMNS FOR FIRST DATAFRAME
                        
                        if (cells[0] != string.Empty){
                            StringDataFrameColumn thisDiv = new StringDataFrameColumn("ThisDiv", 0);
                            PrimitiveDataFrameColumn<DateTime> date = new PrimitiveDataFrameColumn<DateTime>("Date", 0);
                            StringDataFrameColumn homeTeam = new StringDataFrameColumn("HomeTeam", 0);
                            StringDataFrameColumn awayTeam = new StringDataFrameColumn("AwayTeam", 0);
                            PrimitiveDataFrameColumn<int> fthg = new PrimitiveDataFrameColumn<int>("FTHG", 0);
                            PrimitiveDataFrameColumn<int> ftag = new PrimitiveDataFrameColumn<int>("FTAG", 0);
                            StringDataFrameColumn ftr = new StringDataFrameColumn("FTR", 0);
                            PrimitiveDataFrameColumn<int> hs = new PrimitiveDataFrameColumn<int>("HS", 0);
                            PrimitiveDataFrameColumn<int> aws = new PrimitiveDataFrameColumn<int>("AwS", 0);
                            PrimitiveDataFrameColumn<int> hst = new PrimitiveDataFrameColumn<int>("HST", 0);
                            PrimitiveDataFrameColumn<int> awst = new PrimitiveDataFrameColumn<int>("AwST", 0);
                            string[] whichTeam = { "HomeTeam", "AwayTeam" };
                            for (int j = 0; j < 2; j++){
                                //using (MySqlConnection conn = new MySqlConnection(connStr)){
                                    MySqlConnection conn = new MySqlConnection(connStr);
                                    conn.Open();
                                    //string sql = $"SELECT DISTINCT ThisDiv, Date, HomeTeam, AwayTeam, FTHG, FTAG, FTR, HS, AwS, HST, AwST FROM football_data_complete WHERE ({whichTeam[j]} = '{cells[3 + j].Replace("'", string.Empty)}' AND FTHG > -1 AND FTAG > -1 AND HS > -1 AND AwS > -1 AND HST > -1 AND AwST > -1) ORDER BY Date DESC LIMIT 10;";
                                    string sql = $"SELECT DISTINCT ThisDiv, Date, HomeTeam, AwayTeam, FTHG, FTAG, FTR, HS, AwS, HST, AwST FROM football_data_complete WHERE ({whichTeam[j]} = '{cells[3 + j].Replace("'", string.Empty)}' AND FTHG > -1 AND FTAG > -1 AND HS > -1 AND AwS > -1 AND HST > -1 AND AwST > -1) ORDER BY Date DESC LIMIT 10;";
                                    Console.WriteLine(sql);
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
                                        hs.Append(rdr.GetInt32(7));
                                        aws.Append(rdr.GetInt32(8));
                                        hst.Append(rdr.GetInt32(9));
                                        awst.Append(rdr.GetInt32(10));
                                    }
                                    conn.Close();
                                //}
                            }
                            DataFrame df = new DataFrame(thisDiv, date, homeTeam, awayTeam, fthg, ftag, ftr, hs, aws, hst, awst);
                            Console.WriteLine(df.Info());
                            Console.WriteLine(df);
                            //string[] cells = Fixtures[gm].Split(',');
                            thisDivFix.Append(cells[0]);
                            DateTime newDt = Convert.ToDateTime(cells[1]);
                            dateFix.Append(newDt);
                            homeTeamFix.Append(cells[3].Replace("'", string.Empty));
                            awayTeamFix.Append(cells[4].Replace("'", string.Empty));
                            
                            //home team stats
                            float[] homeStats = AveragesPerGame(true, df, cells[3].Replace("'", string.Empty), newDt);
                            hwpg.Append(homeStats[0]);
                            hdpg.Append(homeStats[1]);
                            hlpg.Append(homeStats[2]);
                            hgspg.Append(homeStats[3]);
                            hgcpg.Append(homeStats[4]);
                            hgspst.Append(homeStats[5]);
                            hgcpst.Append(homeStats[6]);
                            hgsps.Append(homeStats[7]);
                            hgcps.Append(homeStats[8]);
                            hstfpg.Append(homeStats[9]);
                            hstapg.Append(homeStats[10]);
                            hstfps.Append(homeStats[11]);
                            hstaps.Append(homeStats[12]);
                            hsfpg.Append(homeStats[13]);
                            hsapg.Append(homeStats[14]);

                            //away team stats
                            float[] awayStats = AveragesPerGame(false, df, cells[4].Replace("'", string.Empty), newDt);
                            awpg.Append(awayStats[0]);
                            adpg.Append(awayStats[1]);
                            alpg.Append(awayStats[2]);
                            agspg.Append(awayStats[3]);
                            agcpg.Append(awayStats[4]);
                            agspst.Append(awayStats[5]);
                            agcpst.Append(awayStats[6]);
                            agsps.Append(awayStats[7]);
                            agcps.Append(awayStats[8]);
                            astfpg.Append(awayStats[9]);
                            astapg.Append(awayStats[10]);
                            astfps.Append(awayStats[11]);
                            astaps.Append(awayStats[12]);
                            asfpg.Append(awayStats[13]);
                            asapg.Append(awayStats[14]);

                            //define whether each row is valid
                            if (homeStats.Contains(-1) || awayStats.Contains(-1)){
                                rowValid.Append(false);
                            }else{
                                rowValid.Append(true);
                            }
                        }
                    }catch (Exception ex){
                        Console.WriteLine("Problem!");
                    }
                }

                //create processed dataframe
                DataFrame dfp = new DataFrame(thisDivFix, dateFix, homeTeamFix, hwpg, hdpg, hlpg, hgspg, hgcpg, hgspst, hgcpst, hgsps, hgcps, hstfpg, hstapg, hstfps, hstaps, hsfpg, hsapg, awayTeamFix, awpg, adpg, alpg, agspg, agcpg, agspst, agcpst, agsps, agcps, astfpg, astapg, astfps, astaps, asfpg, asapg, rowValid);
                dfp = dfp.Filter(rowValid);
                Console.WriteLine(dfp.Info());
                Console.WriteLine(dfp.Sample(10));
                
                //write dataframe to csv
                fName = "../Data/processedFixtures.csv";
                if (File.Exists(fName)){
                    File.Delete(fName);
                }
                string[] hdrs = new string[dfp.Columns.Count];
                hdrs[0] = "ThisDiv";
                hdrs[1] = "Date";
                for (int i = 2; i < dfp.Columns.Count; i++){
                    hdrs[i] = dfp.Columns[i].Name;
                }
                string hdrLine = string.Join(",", hdrs) + ",ftr,over";
                using (StreamWriter sw = new StreamWriter(fName)){
                    sw.WriteLine(hdrLine);
                    for (int i = 0; i < dfp.Rows.Count; i++){
                        sw.WriteLine(string.Join(",", dfp.Rows[i]));
                    }
                }
            }
        }
    }
}