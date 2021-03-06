//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using System;
using SampleClassification.Model;

namespace SampleClassification.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create single instance of sample data from first line of dataset for model input
            ModelInput sampleData = new ModelInput()
            {
                Hgspg = 1.3423195F,
                Hgcpg = 0.9094573F,
                Hstfpg = 6.3157806F,
                Hstapg = 6.6664624F,
                Hsfpg = 9.047229F,
                Hsapg = 10.050814F,
                Agspg = 0.45868862F,
                Agcpg = 1.9061095F,
                Astfpg = 3.8654711F,
                Astapg = 5.815963F,
                Asfpg = 8.026802F,
                Asapg = 11.358909F,
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            Console.WriteLine("Using model to make single prediction -- Comparing actual Over with predicted Over from sample data...\n\n");
            Console.WriteLine($"Hgspg: {sampleData.Hgspg}");
            Console.WriteLine($"Hgcpg: {sampleData.Hgcpg}");
            Console.WriteLine($"Hstfpg: {sampleData.Hstfpg}");
            Console.WriteLine($"Hstapg: {sampleData.Hstapg}");
            Console.WriteLine($"Hsfpg: {sampleData.Hsfpg}");
            Console.WriteLine($"Hsapg: {sampleData.Hsapg}");
            Console.WriteLine($"Agspg: {sampleData.Agspg}");
            Console.WriteLine($"Agcpg: {sampleData.Agcpg}");
            Console.WriteLine($"Astfpg: {sampleData.Astfpg}");
            Console.WriteLine($"Astapg: {sampleData.Astapg}");
            Console.WriteLine($"Asfpg: {sampleData.Asfpg}");
            Console.WriteLine($"Asapg: {sampleData.Asapg}");
            Console.WriteLine($"\n\nPredicted Over value {predictionResult.Prediction} \nPredicted Over scores: [{String.Join(",", predictionResult.Score)}]\n\n");
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }
    }
}
