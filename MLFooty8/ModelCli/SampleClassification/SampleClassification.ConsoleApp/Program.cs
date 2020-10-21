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
                Hwpg = 0.59133595F,
                Hdpg = 0.108467914F,
                Hlpg = 0.30019596F,
                Hgspg = 1.478371F,
                Hgcpg = 0.8484498F,
                Hgspst = 0.20663865F,
                Hgcpst = 0.42139843F,
                Hgsps = 0.11350633F,
                Hgcps = 0.10008782F,
                Hstfpg = 6.2124457F,
                Hstapg = 3.0265427F,
                Hstfps = 0.50406796F,
                Hstaps = 0.37374297F,
                Hsfpg = 12.205923F,
                Hsapg = 7.6205597F,
                Awpg = 0.35132524F,
                Adpg = 0.32293084F,
                Alpg = 0.3257439F,
                Agspg = 1.3096795F,
                Agcpg = 0.94594413F,
                Agspst = 0.28381938F,
                Agcpst = 0.1547411F,
                Agsps = 0.1465731F,
                Agcps = 0.08820769F,
                Astfpg = 4.752082F,
                Astapg = 4.8568244F,
                Astfps = 0.50366336F,
                Astaps = 0.5741905F,
                Asfpg = 9.667408F,
                Asapg = 9.330173F,
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            Console.WriteLine("Using model to make single prediction -- Comparing actual Over with predicted Over from sample data...\n\n");
            Console.WriteLine($"Hwpg: {sampleData.Hwpg}");
            Console.WriteLine($"Hdpg: {sampleData.Hdpg}");
            Console.WriteLine($"Hlpg: {sampleData.Hlpg}");
            Console.WriteLine($"Hgspg: {sampleData.Hgspg}");
            Console.WriteLine($"Hgcpg: {sampleData.Hgcpg}");
            Console.WriteLine($"Hgspst: {sampleData.Hgspst}");
            Console.WriteLine($"Hgcpst: {sampleData.Hgcpst}");
            Console.WriteLine($"Hgsps: {sampleData.Hgsps}");
            Console.WriteLine($"Hgcps: {sampleData.Hgcps}");
            Console.WriteLine($"Hstfpg: {sampleData.Hstfpg}");
            Console.WriteLine($"Hstapg: {sampleData.Hstapg}");
            Console.WriteLine($"Hstfps: {sampleData.Hstfps}");
            Console.WriteLine($"Hstaps: {sampleData.Hstaps}");
            Console.WriteLine($"Hsfpg: {sampleData.Hsfpg}");
            Console.WriteLine($"Hsapg: {sampleData.Hsapg}");
            Console.WriteLine($"Awpg: {sampleData.Awpg}");
            Console.WriteLine($"Adpg: {sampleData.Adpg}");
            Console.WriteLine($"Alpg: {sampleData.Alpg}");
            Console.WriteLine($"Agspg: {sampleData.Agspg}");
            Console.WriteLine($"Agcpg: {sampleData.Agcpg}");
            Console.WriteLine($"Agspst: {sampleData.Agspst}");
            Console.WriteLine($"Agcpst: {sampleData.Agcpst}");
            Console.WriteLine($"Agsps: {sampleData.Agsps}");
            Console.WriteLine($"Agcps: {sampleData.Agcps}");
            Console.WriteLine($"Astfpg: {sampleData.Astfpg}");
            Console.WriteLine($"Astapg: {sampleData.Astapg}");
            Console.WriteLine($"Astfps: {sampleData.Astfps}");
            Console.WriteLine($"Astaps: {sampleData.Astaps}");
            Console.WriteLine($"Asfpg: {sampleData.Asfpg}");
            Console.WriteLine($"Asapg: {sampleData.Asapg}");
            Console.WriteLine($"\n\nPredicted Over value {predictionResult.Prediction} \nPredicted Over scores: [{String.Join(",", predictionResult.Score)}]\n\n");
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }
    }
}
