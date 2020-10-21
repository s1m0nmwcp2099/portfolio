using System;
using System.IO;
using System.Collections.Generic;

namespace SplitData
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] fNames = { "../Data/processedData.csv", "../Data/trainData.csv", "../Data/validationData.csv", "../Data/testData.csv" };
            if (File.Exists(fNames[0])){
                List<string> TrainMatches = new List<string>();
                List<string> ValidationMatches = new List<string>();
                List<string> TestMatches = new List<string>();
                int ind = 0;
                using (StreamReader sr = new StreamReader(fNames[0])){
                    while (sr.Peek() > 0){
                        string thisLine = sr.ReadLine();
                        if (ind == 0){
                            TrainMatches.Add(thisLine);
                            ValidationMatches.Add(thisLine);
                            TestMatches.Add(thisLine);
                        }else if (ind <= 14){
                            TrainMatches.Add(thisLine);
                        }else if (ind <= 17){
                            ValidationMatches.Add(thisLine);
                        }else if (ind <= 20){
                            TestMatches.Add(thisLine);
                        }
                        if (ind < 20){
                            ind++;
                        }else{
                            ind = 1;
                        }
                    }
                }
                using (StreamWriter sw = new StreamWriter(fNames[1])){
                    foreach (string ln in TrainMatches){
                        sw.WriteLine(ln);
                    }
                }
                using (StreamWriter sw = new StreamWriter(fNames[2])){
                    foreach (string ln in ValidationMatches){
                        sw.WriteLine(ln);
                    }
                }
                using (StreamWriter sw = new StreamWriter(fNames[3])){
                    foreach (string ln in TestMatches){
                        sw.WriteLine(ln);
                    }
                }
                Console.Write($"{TrainMatches.Count} training matches \n{ValidationMatches.Count} validation matches \n{TestMatches.Count} test matches");
            }else{
                Console.WriteLine("No processed data to split");
            }
        }
    }
}
