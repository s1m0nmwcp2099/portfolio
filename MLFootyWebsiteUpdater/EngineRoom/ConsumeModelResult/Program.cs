using System;
using SampleClassification.Model;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.ML;
using Microsoft.Data.Analysis;

namespace ConsumeModelResult
{
    class Program
    {
        public static void ConsumeModel(ModelInput _inputData, float[] _probs){
            MLContext mlContext=new MLContext();
            ITransformer mlModel=mlContext.Model.Load("../ModelCli/SampleClassification/SampleClassification.Model/MLModel.zip", out var inputSchema);
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
            DataFrame fixtureDf = DataFrame.LoadCsv("../Data/processedFixtures.csv");

            List<string> AllProbabilities=new List<string>();
            AllProbabilities.Add("league,date,home,away,probh,probd,proba,");
            
            for (int i=0; i<fixtureDf.Rows.Count; i++){
                DataFrameRow dfRow = fixtureDf.Rows[i];
                //Console.WriteLine(dfRow);
                
                ModelInput inputData = new ModelInput()
                {
                    Hwpg = Convert.ToSingle(dfRow[3]),
                    Hdpg = Convert.ToSingle(dfRow[4]),
                    Hlpg = Convert.ToSingle(dfRow[5]),
                    Hgspg = Convert.ToSingle(dfRow[6]),
                    Hgcpg = Convert.ToSingle(dfRow[7]),
                    Hsfpg = Convert.ToSingle(dfRow[8]),
                    Hsapg = Convert.ToSingle(dfRow[9]),
                    Hstfpg = Convert.ToSingle(dfRow[10]),
                    Hstapg = Convert.ToSingle(dfRow[11]),
                    Awpg = Convert.ToSingle(dfRow[13]),
                    Adpg = Convert.ToSingle(dfRow[14]),
                    Alpg = Convert.ToSingle(dfRow[15]),
                    Agspg = Convert.ToSingle(dfRow[16]),
                    Agcpg = Convert.ToSingle(dfRow[17]),
                    Asfpg = Convert.ToSingle(dfRow[18]),
                    Asapg = Convert.ToSingle(dfRow[19]),
                    Astfpg = Convert.ToSingle(dfRow[20]),
                    Astapg = Convert.ToSingle(dfRow[21]),

                };
                float[] thisMatchProbs=new float[3];
                ConsumeModel(inputData, thisMatchProbs);

                string predLn = dfRow[0] + ", " + dfRow[1] + ", " + dfRow[2] + " v " + dfRow[12] + ",";
                string predLnCsv = dfRow[0] + "," + dfRow[1] + "," + dfRow[2] + "," + dfRow[12] + ",";
                
                string[] outcomes={"H: ","D: ","A: "};
                for (int j=0;j<3;j++){
                    predLn += (outcomes[j] + thisMatchProbs[j] + ", ");
                    predLnCsv += (thisMatchProbs[j] + ",");
                }
                Console.WriteLine(predLn);
                AllProbabilities.Add(predLnCsv);
            }
            string fName = "../Data/PredictionsResults.csv";
            if (File.Exists(fName)){
                File.Delete(fName);
            }
            using (StreamWriter sw = new StreamWriter(fName)){
                for (int i=0;i<AllProbabilities.Count;i++){
                    sw.WriteLine(AllProbabilities[i]);
                }
            }
        }
    }
}