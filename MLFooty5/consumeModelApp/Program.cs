using System;
using SampleMulticlassClassification.Model.DataModels;
using Microsoft.ML;

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
            using (StreamReader sr=new StreamReader("../Data/modifiedMatches3Test.csv")){
                while (sr.Peek()>0){
                    NormalisedModifiedFixtures.Add(sr.ReadLine());
                }
            }

            List<string> AllProbabilities=new List<string>();
            AllProbabilities.Add("league,date,home,away,prediction,odds,probability,real");
            foreach (string thisMatch in NormalisedModifiedFixtures){
                string[] cells=thisMatch.Split(',');
                if (cells[0]!="league"){
                    Console.WriteLine(thisMatch);
                    ModelInput inputData=new ModelInput(){
                        League = cells[0],
                        Date = cells[1],
                        Home_team=  cells[2],
                        Away_team = cells[3],
                        Av_hm_ftG_for = cells[4],
                        Av_hm_ftG_con = cells[5],
                        Av_hm_htG_for = cells[6],
                        Av_hm_htG_con = cells[7],
                        Av_hm_shots_for = cells[8],
                        Av_hm_shots_con = cells[9],
                        Av_hm_shots_target_for = cells[10],
                        Av_hm_shots_target_con = cells[11],
                        Av_hm_corners_for = cells[12],
                        Av_hm_corners_con = cells[13],
                        Av_hm_yellow_for = cells[14],
                        Av_hm_yellow_con = cells[15],
                        Av_hm_red_for = cells[16],
                        Av_hm_red_con = cells[17],
                        Av_hm_ft_wins = cells[18],
                        Av_hm_ft_draws = cells[19],
                        Av_hm_ft_losses = cells[20],
                        Av_hm_win_streak = cells[21],
                        Av_hm_winless_streak = cells[22],
                        Av_hm_draw_streak = cells[23],
                        Av_hm_drawless_streak = cells[24],
                        Av_hm_loss_streak = cells[25],
                        Av_hm_lossless_streak = cells[26],
                        Av_hm_ht_wins = cells[27],
                        Av_hm_ht_draws = cells[28],
                        Av_hm_ht_losses = cells[29],
                        Av_hm_pld = cells[30],
                        Av_aw_ftG_for = cells[31],
                        Av_aw_ftG_con = cells[32],
                        Av_aw_htG_for = cells[33],
                        Av_aw_htG_con = cells[34],
                        Av_aw_shots_for = cells[35],
                        Av_aw_shots_con = cells[36],
                        Av_aw_shots_target_for = cells[37],
                        Av_aw_shots_target_con = cells[38],
                        Av_aw_corners_for = cells[39],
                        Av_aw_corners_con = cells[40],
                        Av_aw_yellow_for = cells[41],
                        Av_aw_yellow_con = cells[42],
                        Av_aw_red_for = cells[43],
                        Av_aw_red_con = cells[44],
                        Av_aw_ft_wins = cells[45],
                        Av_aw_ft_draws = cells[46],
                        Av_aw_ft_losses = cells[47],
                        Av_aw_win_streak = cells[48],
                        Av_aw_winless_streak = cells[49],
                        Av_aw_draw_streak = cells[50],
                        Av_aw_drawless_streak = cells[51],
                        Av_aw_loss_streak = cells[52],
                        Av_aw_lossless_streak = cells[53],
                        Av_aw_ht_wins = cells[54],
                        Av_aw_ht_draws = cells[55],
                        Av_aw_ht_losses = cells[56],
                        Av_aw_pld = cells[57],
                        Home_odds = cells[58],
                        Draw_odds = cells[59],
                        Away_odds = cells[60],
                        Ftr = cells[61]
                    };

                    float[] thisMatchProbs=new float[3];
                    ConsumeModel(inputData, thisMatchProbs);

                    string newLineStart="";
                    for (int i=0;i<4;i++){
                        newLineStart+=(cells[i]+",");
                    }
                    string[] outcomes={"Home","Draw","Away"};
                    for (int i=0;i<3;i++){
                        string endLine=outcomes[i]+","+cells[4+i]+","+Convert.ToString(thisMatchProbs[i]+","+",");
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
        }
    }
}
