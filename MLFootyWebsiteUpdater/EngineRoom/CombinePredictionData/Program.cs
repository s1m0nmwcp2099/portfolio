using System;
using System.IO;
using System.Collections.Generic;

namespace CombinePredictionData
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> fullFixturePredictions = new List<string>();
            string fName1 = "../Data/PredictionsResults.csv";
            using (StreamReader sr = new StreamReader(fName1)){
                while (sr.Peek() > 0){
                    fullFixturePredictions.Add(sr.ReadLine());
                }
            }
            string fName2 = "../Data/PredictionsOvers.csv";
            using (StreamReader sr = new StreamReader(fName2)){
                int i = 0;
                while (sr.Peek() > 0){
                    string[] cells = (sr.ReadLine()).Split(',');
                    fullFixturePredictions[i] += (cells[4] + "," + cells[5]);
                    i++;
                }
            }
            string fName3 = "../Data/CombinedPredictions.csv";
            if (File.Exists(fName3)){
                File.Delete(fName3);
            }
            using (StreamWriter sw = new StreamWriter(fName3)){
                for (int i=0; i<fullFixturePredictions.Count; i++){
                    sw.WriteLine(fullFixturePredictions[i]);
                }
            }
        }
    }
}
