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
                Hwpg = 0.28408098F,
                Hdpg = 0.38517413F,
                Hlpg = 0.330745F,
                Hgspg = 1.3423195F,
                Hgcpg = 0.9094573F,
                Hsfpg = 9.047229F,
                Hsapg = 10.050814F,
                Hstfpg = 6.3157806F,
                Hstapg = 6.6664624F,
                Awpg = 0.092881F,
                Adpg = 0.18516196F,
                Alpg = 0.721957F,
                Agspg = 0.45868862F,
                Agcpg = 1.9061095F,
                Asfpg = 8.026802F,
                Asapg = 11.358909F,
                Astfpg = 3.8654711F,
                Astapg = 5.815963F,
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            Console.WriteLine("Using model to make single prediction -- Comparing actual FTR with predicted FTR from sample data...\n\n");
            Console.WriteLine($"Hwpg: {sampleData.Hwpg}");
            Console.WriteLine($"Hdpg: {sampleData.Hdpg}");
            Console.WriteLine($"Hlpg: {sampleData.Hlpg}");
            Console.WriteLine($"Hgspg: {sampleData.Hgspg}");
            Console.WriteLine($"Hgcpg: {sampleData.Hgcpg}");
            Console.WriteLine($"Hsfpg: {sampleData.Hsfpg}");
            Console.WriteLine($"Hsapg: {sampleData.Hsapg}");
            Console.WriteLine($"Hstfpg: {sampleData.Hstfpg}");
            Console.WriteLine($"Hstapg: {sampleData.Hstapg}");
            Console.WriteLine($"Awpg: {sampleData.Awpg}");
            Console.WriteLine($"Adpg: {sampleData.Adpg}");
            Console.WriteLine($"Alpg: {sampleData.Alpg}");
            Console.WriteLine($"Agspg: {sampleData.Agspg}");
            Console.WriteLine($"Agcpg: {sampleData.Agcpg}");
            Console.WriteLine($"Asfpg: {sampleData.Asfpg}");
            Console.WriteLine($"Asapg: {sampleData.Asapg}");
            Console.WriteLine($"Astfpg: {sampleData.Astfpg}");
            Console.WriteLine($"Astapg: {sampleData.Astapg}");
            Console.WriteLine($"\n\nPredicted FTR value {predictionResult.Prediction} \nPredicted FTR scores: [{String.Join(",", predictionResult.Score)}]\n\n");
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }
    }
}
