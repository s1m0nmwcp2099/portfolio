using System;
using SampleMulticlassClassification.Model.DataModels;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
//using Microsoft.Extensions.ML;

namespace consumeModelApp
{
    class Program
    {
        public static void ConsumeModel(ModelInput _inputData, float[] _probs){
            MLContext mlContext=new MLContext();
            ITransformer mlModel=mlContext.Model.Load("MLModel.zip", out var inputSchema);
            var predEngine=mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
            ModelOutput result=predEngine.Predict(_inputData);
            var _thisMatchProbs=GetScoresWithLabelsSorted(predEngine.OutputSchema, "Score", result.Score);
            
            if (_thisMatchProbs.TryGetValue("H", out float prob0)){
                _probs[0]=prob0;
            }
            if (_thisMatchProbs.TryGetValue("D", out float prob1)){
                _probs[1]=prob1;
            }
            if (_thisMatchProbs.TryGetValue("A", out float prob2)){
                _probs[2]=prob2;
            }
        }
        private static Dictionary<string,float> GetScoresWithLabelsSorted(DataViewSchema schema, string name,float[] scores){
            Dictionary<string,float> result=new Dictionary<string, float>();
            var column=schema.GetColumnOrNull(name);
            var slotNames=new VBuffer<ReadOnlyMemory<char>>();
            column.Value.GetSlotNames(ref slotNames);
            var names=new string[slotNames.Length];
            var num=0;
            foreach (var denseValue in slotNames.DenseValues()){
                result.Add(denseValue.ToString(),scores[num++]);
            }
            return result.OrderByDescending(c => c.Value).ToDictionary(i=>i.Key,i=>i.Value);
        }
        static void Main(string[] args)
        {
            

            //PUT PROCESSED MATCHES INTO LIST
            List<string> NormalisedModifiedFixtures = new List<string>();
            using (StreamReader sr=new StreamReader("../Data/normalisedProcessedFixtures.csv")){
                while (sr.Peek()>0){
                    NormalisedModifiedFixtures.Add(sr.ReadLine());
                }
            }
            //GET SUGGESTED BETS
            //get odds from fixture file because odds in allProbabilities.csv have been normalised
            List<string> Odds = new List<string>();
            using (StreamReader sr = new StreamReader("../Data/fixtures.csv")){
                while (sr.Peek() > 0){
                    string[] fixtureCells = sr.ReadLine().Split(',');
                    if (fixtureCells[0] != "Div"){
                        for (int i=0;i<3;i++){
                            Odds.Add(fixtureCells[11 + i]);
                        }
                    }
                }
            }

            List<string> AllProbabilities=new List<string>();
            AllProbabilities.Add("league,date,home,away,prediction,odds,probability,real");
            for (int k=0;k<NormalisedModifiedFixtures.Count;k++){
            //foreach (string thisMatch in NormalisedModifiedFixtures){
                //string[] cells=thisMatch.Split(',');
                string[] cells = NormalisedModifiedFixtures[k].Split(',');
                if (cells[0]!="league"){
                    //Console.WriteLine(thisMatch);
                    Console.WriteLine(NormalisedModifiedFixtures[k]);
                    ModelInput inputData=new ModelInput(){
                        League =  cells[0],
                        Match_date = cells[1],
                        Home_team=  cells[2],
                        Away_team = cells[3],
                        Av_hm_ftG_for = Convert.ToSingle(cells[4]),
                        Av_hm_ftG_con = Convert.ToSingle(cells[5]),
                        Av_hm_htG_for = Convert.ToSingle(cells[6]),
                        Av_hm_htG_con = Convert.ToSingle(cells[7]),
                        Av_hm_shots_for = Convert.ToSingle(cells[8]),
                        Av_hm_shots_con = Convert.ToSingle(cells[9]),
                        Av_hm_shots_target_for = Convert.ToSingle(cells[10]),
                        Av_hm_shots_target_con = Convert.ToSingle(cells[11]),
                        Av_hm_corners_for = Convert.ToSingle(cells[12]),
                        Av_hm_corners_con = Convert.ToSingle(cells[13]),
                        Av_hm_yellow_for = Convert.ToSingle(cells[14]),
                        Av_hm_yellow_con = Convert.ToSingle(cells[15]),
                        Av_hm_red_for = Convert.ToSingle(cells[16]),
                        Av_hm_red_con = Convert.ToSingle(cells[17]),
                        Av_hm_ft_wins = Convert.ToSingle(cells[18]),
                        Av_hm_ft_draws = Convert.ToSingle(cells[19]),
                        Av_hm_ft_losses = Convert.ToSingle(cells[20]),
                        Av_hm_win_streak = Convert.ToSingle(cells[21]),
                        Av_hm_winless_streak = Convert.ToSingle(cells[22]),
                        Av_hm_draw_streak = Convert.ToSingle(cells[23]),
                        Av_hm_drawless_streak = Convert.ToSingle(cells[24]),
                        Av_hm_loss_streak = Convert.ToSingle(cells[25]),
                        Av_hm_lossless_streak = Convert.ToSingle(cells[26]),
                        Av_hm_ht_wins = Convert.ToSingle(cells[27]),
                        Av_hm_ht_draws = Convert.ToSingle(cells[28]),
                        Av_hm_ht_losses = Convert.ToSingle(cells[29]),
                        Av_hm_pld = Convert.ToInt32(cells[30]),
                        Av_aw_ftG_for = Convert.ToSingle(cells[31]),
                        Av_aw_ftG_con = Convert.ToSingle(cells[32]),
                        Av_aw_htG_for = Convert.ToSingle(cells[33]),
                        Av_aw_htG_con = Convert.ToSingle(cells[34]),
                        Av_aw_shots_for = Convert.ToSingle(cells[35]),
                        Av_aw_shots_con = Convert.ToSingle(cells[36]),
                        Av_aw_shots_target_for = Convert.ToSingle(cells[37]),
                        Av_aw_shots_target_con = Convert.ToSingle(cells[38]),
                        Av_aw_corners_for = Convert.ToSingle(cells[39]),
                        Av_aw_corners_con = Convert.ToSingle(cells[40]),
                        Av_aw_yellow_for = Convert.ToSingle(cells[41]),
                        Av_aw_yellow_con = Convert.ToSingle(cells[42]),
                        Av_aw_red_for = Convert.ToSingle(cells[43]),
                        Av_aw_red_con = Convert.ToSingle(cells[44]),
                        Av_aw_ft_wins = Convert.ToSingle(cells[45]),
                        Av_aw_ft_draws = Convert.ToSingle(cells[46]),
                        Av_aw_ft_losses = Convert.ToSingle(cells[47]),
                        Av_aw_win_streak = Convert.ToSingle(cells[48]),
                        Av_aw_winless_streak = Convert.ToSingle(cells[49]),
                        Av_aw_draw_streak = Convert.ToSingle(cells[50]),
                        Av_aw_drawless_streak = Convert.ToSingle(cells[51]),
                        Av_aw_loss_streak = Convert.ToSingle(cells[52]),
                        Av_aw_lossless_streak = Convert.ToSingle(cells[53]),
                        Av_aw_ht_wins = Convert.ToSingle(cells[54]),
                        Av_aw_ht_draws = Convert.ToSingle(cells[55]),
                        Av_aw_ht_losses = Convert.ToSingle(cells[56]),
                        Av_aw_pld = Convert.ToInt32(cells[57]),
                        Home_odds = Convert.ToSingle(cells[58]),
                        Draw_odds = Convert.ToSingle(cells[59]),
                        Away_odds = Convert.ToSingle(cells[60]),
                        //Ftr = cells[61]
                    };

                    float[] thisMatchProbs=new float[3];
                    ConsumeModel(inputData, thisMatchProbs);

                    string newLineStart="";
                    for (int i=0;i<4;i++){
                        newLineStart+=(cells[i]+",");
                    }
                    string[] outcomes={"Home","Draw","Away"};
                    for (int i=0;i<3;i++){
                        string endLine=outcomes[i]+","+Odds[k * 3 - 3 + i]+","+Convert.ToString(thisMatchProbs[i]+","+",");
                        AllProbabilities.Add(newLineStart+endLine);
                    }
                }
            }

            //WRITE ALL PROBABILITIES TO NEW FILE
            string fileName="../Data/allProbabilities.csv";
            if (File.Exists(fileName)){
                File.Delete(fileName);
            }
            using (StreamWriter sw=new StreamWriter(fileName)){
                foreach (string ln in AllProbabilities){
                    sw.WriteLine(ln);
                }
            }

            

            bool satisfied = false;
            while (satisfied == false){
                Console.WriteLine("Enter minimum bet value:");
                double minVal = Convert.ToDouble(Console.ReadLine());
                List<string> AllBets=new List<string>();
                List<string> SuggestedBets=new List<string>();
                using (StreamReader sr=new StreamReader("../Data/allProbabilities.csv")){
                    while (sr.Peek()>0){
                        AllBets.Add(sr.ReadLine());
                    }
                }
                //double minVal=0;
                double maxValue=0;
                string headers="";
                //double kitty=0;
                int betsMade=0;
                //int winningBets=0;
                //int losingBets=0;
                /*int howManyMatches=4000;
                for (int i=1;i<=howManyMatches;i++){
                    string thisMatch=AllBets[AllBets.Count-1-howManyMatches+i];*/
                for (int i=1;i<AllBets.Count;i++){
                    //string thisMatch=AllBets[AllBets.Count-1-howManyMatches+i];
                    string thisMatch=AllBets[i];
                    string[] cells=thisMatch.Split(',');
                    if (cells[0]=="league"){
                        headers+=thisMatch;
                    }else{
                        double odds=Convert.ToDouble(cells[5]);
                        double betValue=odds*Convert.ToDouble(cells[6]);
                        //minVal=1.00;
                        maxValue=20;
                        if (betValue>minVal && betValue<maxValue){
                            //Console.WriteLine(thisMatch);
                            thisMatch+=(","+betValue);
                            if ((cells[4]=="Home"&&cells[7]=="H") || (cells[4]=="Draw"&&cells[7]=="D") || (cells[4]=="Away"&&cells[7]=="A")){
                                //winningBets++;
                                //kitty+=(odds-1);
                                thisMatch+=",Y";
                                
                            }else{
                                //losingBets++;
                                //kitty-=1;
                                thisMatch+=",N";
                            }
                            
                            SuggestedBets.Add(thisMatch);
                            betsMade+=1;
                        }
                    }
                    
                }
                fileName="../Data/suggestedBets.csv";
                if (File.Exists(fileName)){
                    File.Delete(fileName);
                }
                using (StreamWriter sw=new StreamWriter(fileName)){
                    foreach (string bet in SuggestedBets){
                        Console.WriteLine(bet);
                        sw.WriteLine(bet);
                    }
                }
                
                Console.WriteLine(betsMade+" recommended bets");
                //Console.WriteLine(winningBets+" winning bets");
                //Console.WriteLine(losingBets+" losing bets");
                //Console.WriteLine(kitty+" in the kitty from £1 each bet");
                //Console.WriteLine(kitty/Convert.ToDouble(betsMade)+" profit margin");
                Console.WriteLine(minVal+"= min value, "/*+maxValue+"=max value"*/);
                Console.WriteLine("Is this satisfactory? y or n");
                string satAns = Console.ReadLine();
                if (satAns == "y" || satAns == "Y"){
                    satisfied=true;
                }
            }
        }
    }
}
