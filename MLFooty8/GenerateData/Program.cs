using System;
using MySql.Data.MySqlClient;
using Microsoft.Data.Analysis;
using System.Collections.Generic;
using System.Linq;

namespace GenerateData
{
    class Program
    {
        static float[] AveragesPerGame(bool atHome, int matchInd, Dataframe df){
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
            for (int prevMatchInd = matchInd - 1; prevMatchInd >= 0 && prevMatchCt <= 10; prevMatchInd--){
                if (df[matchInd, 2 + adj] == df[prevMatchInd, 2 + adj]){ //check when team played previous games
                    prevMatchCt++;
                    float myDeltaExp = MathF.Exp((df[matchInd, 1] - df[prevMatchInd - 1]).Days * -0.007F); //time delay exponential
                    
                    //win draw or loss
                    if (df[prevMatchInd, 6] == "D"){ //check previous result draw
                        rpg[1] += myDeltaExp;
                    }else if (df[prevMatchInd, 6] == "H"){
                        if (atHome == true){
                            rpg[0] += myDeltaExp;
                        }else{
                            rpg[2] += myDeltaExp;
                        }
                    }else if (df[prevMatchInd, 6] == "A"){
                        if (atHome == true){
                            rpg[2] += myDeltaExp;
                        }else{
                            rpg[0] += myDeltaExp;
                        }
                    }

                    //goals per game
                    gpg[0] += (df[prevMatchInd, 4 + adj] * myDeltaExp); //add time weighted home goals
                    gpg[1] += (df[prevMatchInd, 5 - adj] * myDeltaExp); // -"- away goals

                    //goals per shot on target
                    //gpst[0] += (df[prevMatchInd, 4 + adj] / df[prevMatchInd, 12 + adj] * myDeltaExp);
                    gpst[0] += XperYcorrectZero(df[prevMatchInd, 4 + adj], df[prevMatchInd, 12 + adj], myDeltaExp);
                    //gpst[1] += (df[prevMatchInd, 5 - adj] / df[prevMatchInd, 13 - adj] * myDeltaExp);
                    gpst[1] += XperYcorrectZero(df[prevMatchInd, 5 - adj], df[prevMatchInd, 13 - adj], myDeltaExp);

                    //goals per shot
                    //gps[0] += (df[prevMatchInd, 4 + adj] / df[prevMatchInd, 10 + adj] * myDeltaExp);
                    gps[0] += XperYcorrectZero(df[prevMatchInd, 4 + adj], df[prevMatchInd, 10 + adj], myDeltaExp);
                    //gps[1] += (df[prevMatchInd, 5 - adj] / df[prevMatchInd, 11 - adj] * myDeltaExp);
                    gps[1] += XperYcorrectZero(df[prevMatchInd, 5 - adj], df[prevMatchInd, 11 - adj], myDeltaExp);

                    //shots on target per game
                    stpg[0] += (df[prevMatchInd, 12 + adj] * myDeltaExp);
                    stpg[1] += (df[prevMatchInd, 13 - adj] * myDeltaExp);

                    //shots on target per shot
                    //stps[0] += (df[prevMatchInd, 12 + adj] / df[prevMatchInd, 10 + adj] * myDeltaExp);
                    stps[0] += XperYcorrectZero(df[prevMatchInd, 12 + adj], df[prevMatchInd, 10 + adj], myDeltaExp);
                    //stps[1] += (df[prevMatchInd, 13 - adj] / df[prevMatchInd, 11 - adj] * myDeltaExp);
                    stps[0] += XperYcorrectZero(df[prevMatchInd, 13 - adj], df[prevMatchInd, 11 - adj], myDeltaExp);

                    //shots per game
                    spg[0] += (df[prevMatchInd, 10 + adj] * myDeltaExp);
                    spg[1] += (df[prevMatchInd, 11 - adj] * myDeltaExp);

                    played += myDeltaExp;
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
                return rpg.Concat(gpg).Concat(gpst).Concat(gps).Concat(stpg).Concat(stps).Concat(spg);
                //     0,1,2,     3,4,        5,6,          7,8,         9,10,      11,12,       13,14
            }else{
                float[] dud = new float[15];
                for (int i = 0; i < 15; i++){
                    dud[i] = -1;
                }
            } 
        }
        static float XperYcorrectZero (float x, float y, float delta){
            if (y != 0f){
                return (x / y * delta);
            }else{
                return delta;
            }
        }
        static string connStr = "server = localhost; user = simon; database = football; port = 3306; password = chainsaw";
        static void Main(string[] args)
        {
            //GATHER HISTORICAL MATCH DATA FROM MYSQL TABLE FOR MATCHES THAT INCLUDE HALF TIME SCORES, SHOTS, SHOTS ON TARGETIS NOT
            string sqlCt = "SELECT COUNT(*) FROM football_data_complete WHERE (FTHG IS NOT NULL AND FTAG IS NOT NULL AND FTR IS NOT NULL AND HTHG IS NOT NULL AND HTAG IS NOT NULL AND HTR IS NOT NULL AND HS IS NOT NULL AND AwS IS NOT NULL AND HST IS NOT NULL AND AwST IS NOT NULL AND HC IS NOT NULL AND AC IS NOT NULL AND FTHG <> 'null' AND FTAG <> 'null' AND FTR <> 'null' AND HTHG <> 'null' AND HTAG <> 'null' AND HTR <> 'null' AND HS <> 'null' AND AwS <> 'null' AND HST <> 'null' AND AwST <> 'null' AND HC <> 'null' AND AC <> 'null');";
            //string sqlCt = "SELECT COUNT(*) FROM football_data_complete WHERE (FTHG AND FTAG AND FTR AND HTHG AND HTAG AND HTR AND HS AND AwS AND HST AND AwST AND HC AND AC) IS NOT NULL";
            int ct = 0;
            using (MySqlConnection conn = new MySqlConnection(connStr)){
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlCt, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read()){
                    ct = rdr.GetInt32(0);
                }
                conn.Close();
            }
            Console.WriteLine($"{ct} lines");
            
            //COLUMNS FOR FIRST DATAFRAME
            StringDataFrameColumn thisDiv = new StringDataFrameColumn("ThisDiv", 0);
            PrimitiveDataFrameColumn<DateTime> date = new PrimitiveDataFrameColumn<DateTime>("Date", 0);
            StringDataFrameColumn homeTeam = new StringDataFrameColumn("HomeTeam", 0);
            StringDataFrameColumn awayTeam = new StringDataFrameColumn("AwayTeam", 0);
            PrimitiveDataFrameColumn<int> fthg = new PrimitiveDataFrameColumn<int>("FTHG", 0);
            PrimitiveDataFrameColumn<int> ftag = new PrimitiveDataFrameColumn<int>("FTAG", 0);
            StringDataFrameColumn ftr = new StringDataFrameColumn("FTR", 0);
            PrimitiveDataFrameColumn<int> hthg = new PrimitiveDataFrameColumn<int>("HTHG", 0);
            PrimitiveDataFrameColumn<int> htag = new PrimitiveDataFrameColumn<int>("HTAG", 0);
            StringDataFrameColumn htr = new StringDataFrameColumn("HTR", 0);
            PrimitiveDataFrameColumn<int> hs = new PrimitiveDataFrameColumn<int>("HS", 0);
            PrimitiveDataFrameColumn<int> aws = new PrimitiveDataFrameColumn<int>("AwS", 0);
            PrimitiveDataFrameColumn<int> hst = new PrimitiveDataFrameColumn<int>("HST", 0);
            PrimitiveDataFrameColumn<int> awst = new PrimitiveDataFrameColumn<int>("AwST", 0);
            PrimitiveDataFrameColumn<int> hc = new PrimitiveDataFrameColumn<int>("HC", 0);
            PrimitiveDataFrameColumn<int> ac = new PrimitiveDataFrameColumn<int>("AC", 0);

            string sql = "SELECT ThisDiv, Date, HomeTeam, AwayTeam, FTHG, FTAG, FTR, HTHG, HTAG, HTR, HS, AwS, HST, AwST, HC, AC  FROM football_data_complete WHERE (FTHG IS NOT NULL AND FTAG IS NOT NULL AND FTR IS NOT NULL AND HTHG IS NOT NULL AND HTAG IS NOT NULL AND HTR IS NOT NULL AND HS IS NOT NULL AND AwS IS NOT NULL AND HST IS NOT NULL AND AwST IS NOT NULL AND HC IS NOT NULL AND AC IS NOT NULL AND FTHG <> 'null' AND FTAG <> 'null' AND FTR <> 'null' AND HTHG <> 'null' AND HTAG <> 'null' AND HTR <> 'null' AND HS <> 'null' AND AwS <> 'null' AND HST <> 'null' AND AwST <> 'null' AND HC <> 'null' AND AC <> 'null');";
            //string sql = "SELECT * FROM football_data_complete WHERE (FTHG AND FTAG AND FTR AND HTHG AND HTAG AND HTR AND HS AND AwS AND HST AND AwST AND HC AND AC) IS NOT NULL";
            using (MySqlConnection conn = new MySqlConnection(connStr)){
                conn.Open();
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
                    hthg.Append(rdr.GetInt32(7));
                    htag.Append(rdr.GetInt32(8));
                    htr.Append(rdr.GetString(9));
                    hs.Append(rdr.GetInt32(10));
                    aws.Append(rdr.GetInt32(11));
                    hst.Append(rdr.GetInt32(12));
                    awst.Append(rdr.GetInt32(13));
                    hc.Append(rdr.GetInt32(14));
                    ac.Append(rdr.GetInt32(15));
                }
                conn.Close();
            }
            //CREATE DATAFRAME
            DataFrame df = new DataFrame(thisDiv, date, homeTeam, awayTeam, fthg, ftag, ftr, hthg, htag, htr, hs, aws, hst, awst, hc, ac);
            //Console.WriteLine(df.Info());
            //Console.WriteLine(df.Sample(20));

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

            PrimitiveDataFrameColumn<float> hstfpg = new PrimitiveDataFrameColumn<float>("Hstfps", 0);  //home shots on target for per shot
            PrimitiveDataFrameColumn<float> hstapg = new PrimitiveDataFrameColumn<float>("Hstaps", 0);  //home shots on target against per shot
            PrimitiveDataFrameColumn<float> astfpg = new PrimitiveDataFrameColumn<float>("Astfps", 0);  //away shots on target for ps
            PrimitiveDataFrameColumn<float> astapg = new PrimitiveDataFrameColumn<float>("Astaps", 0);  //away shots on target against ps

            for (int gm = 0; gm < thisDiv.Length; gm++){
                float[] homeStats = AveragesPerGame(true, gm, df);
                hwpg.Append(homeStats[0]);
                hdpg.Append(homeStats[1]);
                hlpg.Append(homeStats[2]);
                hgspst.Append(homeStats[3]);
                hgcpst.Append(homeStats[4]);
                hgsps.Append(homeStats[5]);
                hgcps.Append(homeStats[6]);
                hsfpg.Append(homeStats[7]);
                hsfpg.Append(homeStats[8]);
            }
        }
    }
}
