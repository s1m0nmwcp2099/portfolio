using System;
using SampleMulticlassClassification.Model.DataModels;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.ML;

namespace ConsumeModelApp
{
    class Program
    {
        static void Main(string[] args)
        {

            //PUT PROCESSED MATCHES INTO LIST
            List<string> ProcessedMatches=new List<string>();
            using (StreamReader sr=new StreamReader("../Data/modifiedMatches3Test.csv")){
                while (sr.Peek()>0){
                    ProcessedMatches.Add(sr.ReadLine());
                }
            }

            List<string> AllProbabilities=new List<string>();
            AllProbabilities.Add("league,date,home,away,prediction,odds,probability,real");
            foreach (string thisMatch in ProcessedMatches){
                string[] cells=thisMatch.Split(',');
                if (cells[0]!="league"){
                    Console.WriteLine(thisMatch);
                    ModelInput inputData=new ModelInput(){
                        League=cells[0],
                        Date=cells[1],
                        Home_team=cells[2],
                        Away_team=cells[3],
                        Hm_odds=Convert.ToSingle(cells[4]),
                        Dr_odds=Convert.ToSingle(cells[5]),
                        Aw_odds=Convert.ToSingle(cells[6]),
                        HmGfAtHm=Convert.ToSingle(cells[7]),
                        HmGagAtHm=Convert.ToSingle(cells[8]),
                        HmWatHm=Convert.ToSingle(cells[9]),
                        HmDrAtHm=Convert.ToSingle(cells[10]),
                        HmLosAtHm=Convert.ToSingle(cells[11]),
                        AwGfAtAw=Convert.ToSingle(cells[12]),
                        AwGagAtAw=Convert.ToSingle(cells[13]),
                        AwWatAw=Convert.ToSingle(cells[14]),
                        AwDrAtAw=Convert.ToSingle(cells[15]),
                        AwLosAtAw=Convert.ToSingle(cells[16]),
                        /*HmOppGf=Convert.ToSingle(cells[17]),
                        HmOppGagg=Convert.ToSingle(cells[18]),
                        HmOppWin=Convert.ToSingle(cells[19]),
                        HmOppDraw=Convert.ToSingle(cells[20]),
                        HmOppLoss=Convert.ToSingle(cells[21]),
                        AwOppGf=Convert.ToSingle(cells[22]),
                        AwOppGagg=Convert.ToSingle(cells[23]),
                        AwOppWin=Convert.ToSingle(cells[24]),
                        AwOppDraw=Convert.ToSingle(cells[25]),
                        AwOppLoss=Convert.ToSingle(cells[26]),*/
                    };

                    float[] thisMatchProbs=new float[3];
                    ConsumeModel(inputData, thisMatchProbs);

                    string newLineStart="";
                    for (int i=0;i<4;i++){
                        newLineStart+=(cells[i]+",");
                    }
                    string[] outcomes={"Home","Draw","Away"};
                    for (int i=0;i<3;i++){
                        string endLine=outcomes[i]+","+cells[4+i]+","+Convert.ToString(thisMatchProbs[i]+","+cells[/*27*/17]);
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
    }
}
