using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

namespace CreateModifyData
{
    class Program
    {
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
                if (daysAgo>60){
                    daysAgo-=45;
                }
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
        static double[] TeamData2(List<string> _AllRawMatches, List<int> _Indices, int _min,bool atHome, DateTime _currentDate){
            //0=glsF,1=glsAg,2=wins,3=draws,4=losses
            double[] thisTeamData=new double[5];
            double pld=0;
            double glsF=0; double glsAg=0; double wins=0; double draws=0; double losses=0;
            
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
                /*if (daysAgo>60){
                    daysAgo-=45;
                }*/
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
                /*if (daysAgo>60){
                    daysAgo-=45;
                }*/
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
        {
            DateTime firstFxDt = DateTime.Now;

            Console.WriteLine("Please select:");
            Console.WriteLine("1 - Create database just for fixtures");
            Console.WriteLine("2 - Create database to create model. This takes longer");
            string option = Console.ReadLine();
            DateTime ourDt = Convert.ToDateTime("01/01/2018");
            if (option == "2"){
                ourDt = Convert.ToDateTime("01/01/1970");
            }
            string sqlDt = ourDt.ToString("yyyy-MM-dd"); 
            //GET ALL MATCH DATA FROM SQL AND WRITE TO MEMORY
            List<string> AllRawMatches=new List<string>();
            List<string> AllProcessedMatches=new List<string>();

            string connStr="server=localhost;user=simon;database=football;port=3306;password=chainsaw";
            //string sql="SELECT * FROM wide_results";
            string sql="SELECT * FROM results_and_odds WHERE date > '" + sqlDt +"'";
            using MySqlConnection con=new MySqlConnection(connStr);
            con.Open();
            using MySqlCommand cmd0=new MySqlCommand(sql,con);
            using MySqlDataReader rdr0=cmd0.ExecuteReader();
            
            while (rdr0.Read()){
                string thisMatch="";
                //for (int i=0;i<23;i++){
                for (int i=0;i<9;i++){
                    thisMatch+=rdr0.GetValue(i);
                    thisMatch+=",";
                }
                string season="";
                DateTime thisDt=Convert.ToDateTime(rdr0.GetValue(1));
                //which season
                DateTime season2start=Convert.ToDateTime("01/07/2018");
                DateTime season3start=Convert.ToDateTime("01/07/2019");
                if (thisDt.CompareTo(season2start)==-1){
                    season="1";
                }else if (thisDt.CompareTo(season3start)==-1){
                    season="2";
                }else{
                    season="3";
                }
                thisMatch+=season;
                AllRawMatches.Add(thisMatch);
            }
            con.Close();
            //Same with fixtures
            //FIXTURES
            //int firstFxtInd = AllRawMatches.Count;
            List<string> RawFixtures=new List<string>();
            string fileName="../Data/fixtures.csv";
            using (StreamReader sr=new StreamReader(fileName)){
                while (sr.Peek()>0){
                    RawFixtures.Add(sr.ReadLine());
                }
            }
            for (int i=0;i<RawFixtures.Count;i++){
                string[] cells=RawFixtures[i].Split(',');
                if (cells[0]!="Div"){
                    if (i==1){
                        firstFxDt = Convert.ToDateTime(cells[1]);
                    }
                    string thisMatch=cells[0]+","+cells[1]+","+cells[3]+","+cells[4]+",0"+",0"+","+cells[11]+","+
                        cells[12]+","+cells[13];
                    AllRawMatches.Add(thisMatch);
                }    
            }
            

            //MAKE TEAM LIST AND REF INDICES FOR EACH TEAM'S PREVIOUS MATCHES
            List<string> Teams=new List<string>();
            List<List<int>> HmTmsPrevMatches=new List<List<int>>();
            List<List<int>> AwTmsPrevMatches=new List<List<int>>();
            List<List<int>> HmTmsPrevOpp=new List<List<int>>();
            List<List<int>> AwTmsPrevOpp=new List<List<int>>();
            foreach (string thisLine in AllRawMatches){
                string[] cells=thisLine.Split(',');
                for (int i=2;i<=3;i++){
                    if (!Teams.Contains(cells[i])){
                        Teams.Add(cells[i]);
                    }
                }
            }
            int[] homePld=new int[Teams.Count]; int[] awayPld=new int[Teams.Count];
            for (int i=0;i<Teams.Count;i++){
                HmTmsPrevMatches.Add(new List<int>());
                AwTmsPrevMatches.Add(new List<int>());

                homePld[i]=0; awayPld[i]=0;
            }

            double[,] allTeamsHmData=new double[Teams.Count,5];
            double[,] allTeamsAwData=new double[Teams.Count,5];
            


            //ITERATE THROUGH EACH GAME
            for (int i=0;i<AllRawMatches.Count;i++){
                string[] cells=AllRawMatches[i].Split(',');
                string homeTeam=cells[2]; int hmInd=Teams.IndexOf(homeTeam);
                string awayTeam=cells[3]; int awInd=Teams.IndexOf(awayTeam);
                //Check if both teams have played x or more games
                int min=10;
                if (homePld[hmInd]>=min && /*awayPld[hmInd]>=min && homePld[awInd]>=min &&*/ awayPld[awInd]>=min){
                    //PROCESS DATA AND PRESENT TO LIST<string>
                    string newLine="";
                    
                    //GENERAL
                    //newLine+=(cells[0]+","+cells[1]+","+homeTeam+","+awayTeam+","+cells[20]+","+cells[21]+","+cells[22]);
                    newLine+=(cells[0]+","+cells[1]+","+homeTeam+","+awayTeam+","+cells[6]+","+cells[7]+","+cells[8]);

                    //HOME TEAM AT HOME
                    Console.WriteLine(cells[1]);
                    double [] homeTeamHmData=new double[0]; double [] awayTeamAwData=new double[0];
                    if (cells[1] != ""){
                        homeTeamHmData=TeamData2(AllRawMatches,HmTmsPrevMatches[hmInd],min,true,Convert.ToDateTime(cells[1]));
                        for (int j=0;j<5;j++){ //j<11 for method inc shots,ot+c
                            newLine+=(","+Convert.ToString(homeTeamHmData[j]));
                        }
                        
                        /*int[] homeTeamHmStreaks=TeamStreaks(AllRawMatches, HmTmsPrevMatches[hmInd], homeTeam);
                        for (int j=0;j<6;j++){
                            newLine+=(","+Convert.ToString(homeTeamHmStreaks[j]));
                        }

                        //HOME TEAM AT AWAY
                        
                        double[] homeTeamAwData=TeamData(AllRawMatches,AwTmsPrevMatches[hmInd],min,false,Convert.ToDateTime(cells[1]));
                        for (int j=0;j<11;j++){
                            newLine+=(","+Convert.ToString(homeTeamAwData[j]));
                        }
                        int[] homeTeamAwStreaks=TeamStreaks(AllRawMatches, AwTmsPrevMatches[hmInd], homeTeam);
                        for (int j=0;j<6;j++){
                            newLine+=(","+Convert.ToString(homeTeamAwStreaks[j]));
                        }

                        //AWAY TEAM AT HOME
                        double[] awayTeamHmData=TeamData(AllRawMatches,HmTmsPrevMatches[awInd],min,true,Convert.ToDateTime(cells[1]));
                        for (int j=0;j<11;j++){
                            newLine+=(","+Convert.ToString(awayTeamHmData[j]));
                        }
                        int[] awayTeamHmStreaks=TeamStreaks(AllRawMatches, HmTmsPrevMatches[awInd], awayTeam);
                        for (int j=0;j<6;j++){
                            newLine+=(","+Convert.ToString(awayTeamHmStreaks[j]));
                        }
                        */

                        //AWAY TEAM AT AWAY
                        awayTeamAwData=TeamData2(AllRawMatches,AwTmsPrevMatches[awInd],min,false,Convert.ToDateTime(cells[1]));
                        for (int j=0;j<5;j++){ //j<11 for method inc shots,ot+c
                            newLine+=(","+Convert.ToString(awayTeamAwData[j]));
                        }
                    }
                    

                    /*int[] awayTeamAwStreaks=TeamStreaks(AllRawMatches, AwTmsPrevMatches[awInd], awayTeam);
                    for (int j=0;j<6;j++){
                        newLine+=(","+Convert.ToString(awayTeamAwStreaks[j]));
                    }

                    //HOME TEAM AT HOME AND AWAY STREAKS
                    List<int> TmsPrevMatchesBoth=new List<int>();
                    foreach (int x in HmTmsPrevMatches[hmInd]){
                        TmsPrevMatchesBoth.Add(x);
                    }
                    foreach (int x in AwTmsPrevMatches[hmInd]){
                        TmsPrevMatchesBoth.Add(x);
                    }
                    TmsPrevMatchesBoth.Sort();
                    int[] homeTeamBothStreaks=TeamStreaks(AllRawMatches, TmsPrevMatchesBoth, homeTeam);
                    for (int j=0;j<6;j++){
                        newLine+=(","+Convert.ToString(homeTeamBothStreaks[j]));
                    }

                    //AWAY TEAM AT HOME AND AWAY
                    TmsPrevMatchesBoth=new List<int>();
                    foreach (int x in HmTmsPrevMatches[awInd]){
                        TmsPrevMatchesBoth.Add(x);
                    }
                    foreach (int x in AwTmsPrevMatches[awInd]){
                        TmsPrevMatchesBoth.Add(x);
                    }
                    TmsPrevMatchesBoth.Sort();
                    int[] awayTeamBothStreaks=TeamStreaks(AllRawMatches, TmsPrevMatchesBoth, awayTeam);
                    for (int j=0;j<6;j++){
                        newLine+=(","+Convert.ToString(awayTeamBothStreaks[j]));
                    }*/

                    //ACTUAL RESULT
                    //newLine+=(","+cells[6]);
                    int homeGoals=Convert.ToInt32(cells[4]);
                    int awayGoals=Convert.ToInt32(cells[5]);
                    string thisResult="";
                    if (homeGoals>awayGoals){
                        thisResult="H";
                    }else if (homeGoals==awayGoals){
                        thisResult="D";
                    }else{
                        thisResult="A";
                    }
                    newLine+=(","+thisResult);
                    AllProcessedMatches.Add(newLine);
                }
                //add data of this match for future ref
                homePld[hmInd]+=1; awayPld[awInd]+=1;
                HmTmsPrevMatches[hmInd].Add(i);
                AwTmsPrevMatches[awInd].Add(i);
                //HmTmsPrevOpp[hmInd].Add(AllProcessedMatches.Count-1);
                //AwTmsPrevOpp[awInd].Add(AllProcessedMatches.Count-1);
            }

            //**************************************************
            

            //********************************************************************************************************
            //THIS ADDS THE OPPONENT DATA

            //reset lists
            HmTmsPrevMatches=new List<List<int>>();
            AwTmsPrevMatches=new List<List<int>>();
            for (int i=0;i<Teams.Count;i++){
                HmTmsPrevMatches.Add(new List<int>()); //indices in AllProcessedMatches list
                AwTmsPrevMatches.Add(new List<int>());

                homePld[i]=0; awayPld[i]=0; //refers only to modified matches list
            }

            List<string> AllProcessedMatches2=new List<string>(); //f,a,w,d,l for home,away and opponents
            List<string> AllProcessedMatches3=new List<string>(); //combine 
            List<string> AllProcessedFixtures2=new List<string>();
            List<string> AllProcessedFixtures3=new List<string>();


            //iterate through matches
            for (int i=0;i<AllProcessedMatches.Count;i++){
                string[] cells=AllProcessedMatches[i].Split(',');
                if (cells[0]!=""){
                    string homeTeam=cells[2]; int hmInd=Teams.IndexOf(homeTeam);
                    string awayTeam=cells[3]; int awInd=Teams.IndexOf(awayTeam);
                    int min=10;
                    if (homePld[hmInd]>=min && awayPld[awInd]>=min){
                        //original data except result
                        string myNewLine="";
                        for (int j=0;j<cells.Length-1;j++){
                            myNewLine+=(cells[j]+",");
                        }
                        Console.WriteLine(myNewLine);

                        double[] oppStats=new double[0];
                        //get home team opponents data
                        oppStats=GetAvOppStats(AllProcessedMatches,HmTmsPrevMatches[hmInd],min,true);
                        for (int j=0;j<oppStats.Length;j++){
                            myNewLine+=(Convert.ToString(oppStats[j])+",");
                        }
                        //get away team opponents data
                        oppStats=GetAvOppStats(AllProcessedMatches,AwTmsPrevMatches[awInd],min,false);
                        for (int j=0;j<oppStats.Length;j++){
                            myNewLine+=(Convert.ToString(oppStats[j])+",");
                        }
                        //add result
                        myNewLine+=cells[cells.Length-1];

                        //add to new list
                        AllProcessedMatches2.Add(myNewLine);
                        //Console.WriteLine(myNewLine);

                        //CREATE ALLPROCESSEDMATCHES3 -- (team stats and opp stats combined()
                        string[] homeTeamData=CombinedStats(myNewLine,true);
                        string[] awayTeamdata=CombinedStats(myNewLine,false);
                        myNewLine="";
                        for (int j=0;j<7;j++){
                            myNewLine+=(cells[j]+",");
                        }
                        for (int j=0;j<5;j++){
                            myNewLine+=(homeTeamData[j]+",");
                        }
                        for (int j=0;j<5;j++){
                            myNewLine+=(awayTeamdata[j]+",");
                        }
                        myNewLine+=cells[cells.Length-1];
                        if (i%5000==0){
                            Console.WriteLine(cells[1]);
                        }
                        AllProcessedMatches3.Add(myNewLine);
                    }
                    HmTmsPrevMatches[hmInd].Add(i) ; AwTmsPrevMatches[awInd].Add(i);
                    homePld[hmInd]++; awayPld[awInd]++;
                }
            }
            //********************************************
            

            //write to new file
            fileName="../Data/modifiedMatches2.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,awGfAtAw,awGagAtAw,awWatAw,awDrAtAw,awLosAtAw,hmOppGf,hmOppGagg,hmOppWin,hmOppDraw,hmOppLoss,awOppGf,awOppGagg,awOppWin,awOppDraw,awOppLoss,ftr");
                foreach (string thisLine in AllProcessedMatches2){
                    sw.WriteLine(thisLine);
                }
            }

            //write to new file 3
            fileName="../Data/modifiedMatches3.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,awGfAtAw,awGagAtAw,awWatAw,awDrAtAw,awLosAtAw,ftr");
                foreach (string thisLine in AllProcessedMatches3){
                    sw.WriteLine(thisLine);
                }
            }
            






            //************************************************************************************************************




            //PUT PROCESSED MATCHES INTO A FILE
            fileName="../Data/modifiedMatches.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                //0=glsF,1=glsAg,2=shF,3=shA,4=shTf,5=shTag,6=cF,7=cAg,8=wins,9=draws,10=losses

                //WITHOUT STREAKS
                //sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmShFatHm,hmShAgAtHm,hmShTfAtHm,hmShTagAtHm,hmCfAtHm,hmCagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,hmGfAtaw,hmGagAtAw,hmShFatAw,hmShAgAtAw,hmShTfAtAw,hmShTagAtAw,hmCfAtAw,hmCagAtAw,hmWatAw,hmDrAtAw,hmLosAtAw,awGfAtHm,awGagAtHm,awShFatHm,awShAgAtHm,awShTfAtHm,awShTagAtHm,awCfAtHm,awCagAtHm,awWatHm,awDrAtHm,awLosAtHm,awGfAtAw,awGagAtAw,awShFatAw,awShAgAtAw,awShTfAtAw,awShTagAtAw,awCfAtAw,awCagAtHAw,awWatAw,awDrAtAw,awLosAtAw,ftr");
                //            *about                          *odds                   *home team at home                                                                                           *home team away                                                                                              *away team at home                                                                                           *away team away                                                                                               *result
                
                //WITH STREAKS
                //sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmShFatHm,hmShAgAtHm,hmShTfAtHm,hmShTagAtHm,hmCfAtHm,hmCagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,hmWstrkAtHm,hmWXstrkAtHm,hmDstrkAtHm,hmDXstrkAtHm,hmLstrkAtHm,hmLXstrkAtHm,hmGfAtaw,hmGagAtAw,hmShFatAw,hmShAgAtAw,hmShTfAtAw,hmShTagAtAw,hmCfAtAw,hmCagAtAw,hmWatAw,hmDrAtAw,hmLosAtAw,hmWstrkAtAw,hmWXstrkAtAw,hmDstrkAtAw,hmDXstrkAtAw,hmLstrkAtAw,hmLXstrkAtAw,awGfAtHm,awGagAtHm,awShFatHm,awShAgAtHm,awShTfAtHm,awShTagAtHm,awCfAtHm,awCagAtHm,awWatHm,awDrAtHm,awLosAtHm,awWstrkAtHm,awWXstrkAtHm,awDstrkAtHm,awDXstrkAtHm,awLstrkAtHm,awLXstrkAtHm,awGfAtAw,awGagAtAw,awShFatAw,awShAgAtAw,awShTfAtAw,awShTagAtAw,awCfAtAw,awCagAtHAw,awWatAw,awDrAtAw,awLosAtAw,awWstrkAtaw,awWXstrkAtaw,awDstrkAtaw,awDXstrkAtaw,awLstrkAtaw,awLXstrkAtaw,hmWstrkAtBoth,hmWXstrkAtBoth,hmDstrkAtBoth,hmDXstrkAtBoth,hmLstrkAtBoth,hmLXstrkAtBoth,awWstrkAtBoth,awWXstrkAtBoth,awDstrkAtBoth,awDXstrkAtBoth,awLstrkAtBoth,awLXstrkAtBoth,ftr");
                //            *about                          *odds                   *home team at home                                                                                                                                                                      *home team away                                                                                                                                                                         *away team at home                                                                                                                                                                      *away team away                                                                                                                                                                          *streaks for home team ALL                                                               *streaks for away team ALL                                                             *result

                //WITH STREAKS, REMOVED ALL HOME AT AWAY AND AWAY AT HOME
                //sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmShFatHm,hmShAgAtHm,hmShTfAtHm,hmShTagAtHm,hmCfAtHm,hmCagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,hmWstrkAtHm,hmWXstrkAtHm,hmDstrkAtHm,hmDXstrkAtHm,hmLstrkAtHm,hmLXstrkAtHm,awGfAtAw,awGagAtAw,awShFatAw,awShAgAtAw,awShTfAtAw,awShTagAtAw,awCfAtAw,awCagAtHAw,awWatAw,awDrAtAw,awLosAtAw,awWstrkAtaw,awWXstrkAtaw,awDstrkAtaw,awDXstrkAtaw,awLstrkAtaw,awLXstrkAtaw,hmWstrkAtBoth,hmWXstrkAtBoth,hmDstrkAtBoth,hmDXstrkAtBoth,hmLstrkAtBoth,hmLXstrkAtBoth,awWstrkAtBoth,awWXstrkAtBoth,awDstrkAtBoth,awDXstrkAtBoth,awLstrkAtBoth,awLXstrkAtBoth,ftr");
                //            *about                          *odds                   *home team at home                                                                                                                                                                      *away team away                                                                                                                                                                          *streaks for home team ALL                                                               *streaks for away team ALL                                                             *result
                
                sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,awGfAtAw,awGagAtAw,awWatAw,awDrAtAw,awLosAtAw,ftr");
                
                
                foreach (string newLine in AllProcessedMatches){
                    sw.WriteLine(newLine);
                }
            }

            //************************************************************

            //SPLIT MOD AND MOD2 FOR TRAINING AND TESTING
            //MOD1
            /*
            List<string> Train=new List<string>();
            List<string> Test=new List<string>();
            foreach (string s in AllProcessedMatches){
                string[] cells=s.Split(',');
                if (Convert.ToDateTime(cells[1]).CompareTo(Convert.ToDateTime("20/07/2019"))==-1){
                    Train.Add(s);
                }else{
                    Test.Add(s);
                }
            }

            fileName="../Data/modifiedMatchesTrain.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,awGfAtAw,awGagAtAw,awWatAw,awDrAtAw,awLosAtAw,ftr");
                foreach (string s in Train){
                    sw.WriteLine(s);
                }
            }

            fileName="../Data/modifiedMatchesTest.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,awGfAtAw,awGagAtAw,awWatAw,awDrAtAw,awLosAtAw,ftr");
                foreach (string s in Test){
                    sw.WriteLine(s);
                }
            }*/

            //*****************************
            //MOD2 (CANNOT RUN ALONGSIDE MOD1 - NEED TO CHANGE VARIABLE NAMES)
            List<string> Train=new List<string>();
            List<string> Test=new List<string>();
            for (int i=0;i<AllProcessedMatches2.Count;i++){
            //foreach (string s in AllProcessedMatches2){
                string s = AllProcessedMatches2[i];
                string[] cells=s.Split(',');
                if (Convert.ToDateTime(cells[1]).CompareTo(firstFxDt)<0){
                    Train.Add(s);
                }else{
                    Test.Add(s);
                }
            }

            fileName="../Data/modifiedMatches2Train.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,awGfAtAw,awGagAtAw,awWatAw,awDrAtAw,awLosAtAw,hmOppGf,hmOppGagg,hmOppWin,hmOppDraw,hmOppLoss,awOppGf,awOppGagg,awOppWin,awOppDraw,awOppLoss,ftr");
                foreach (string s in Train){
                    sw.WriteLine(s);
                }
            }

            fileName="../Data/modifiedMatches2Test.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,awGfAtAw,awGagAtAw,awWatAw,awDrAtAw,awLosAtAw,hmOppGf,hmOppGagg,hmOppWin,hmOppDraw,hmOppLoss,awOppGf,awOppGagg,awOppWin,awOppDraw,awOppLoss,ftr");
                foreach (string s in Test){
                    sw.WriteLine(s);
                }
            }

            //MOD3 split
            List<string> Train3=new List<string>();
            List<string> Test3=new List<string>();
            for (int i=0;i<AllProcessedMatches3.Count;i++){
                string s = AllProcessedMatches3[i];
                string[] cells=s.Split(',');
                if (Convert.ToDateTime(cells[1]).CompareTo(firstFxDt)<0){
                //if (Convert.ToDateTime(cells[1]).CompareTo(Convert.ToDateTime("01/06/2020"))<0){
                    Train3.Add(s);
                }else{
                    Test3.Add(s);
                }
            }

            fileName="../Data/modifiedMatches3Train.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,awGfAtAw,awGagAtAw,awWatAw,awDrAtAw,awLosAtAw,ftr");
                foreach (string s in Train3){
                    sw.WriteLine(s);
                }
            }

            fileName="../Data/modifiedMatches3Test.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                sw.WriteLine("league,date,home_team,away_team,hm_odds,dr_odds,aw_odds,hmGfAtHm,hmGagAtHm,hmWatHm,hmDrAtHm,hmLosAtHm,awGfAtAw,awGagAtAw,awWatAw,awDrAtAw,awLosAtAw,ftr");
                foreach (string s in Test3){
                    sw.WriteLine(s);
                }
            }
        }

    }
}
