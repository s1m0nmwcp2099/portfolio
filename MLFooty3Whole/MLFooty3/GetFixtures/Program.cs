using System;
using System.IO;
using System.Net;

namespace GetFixtures
{
    class Program
    {
        static bool URLExists(string url)
        {
            bool result = true;
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Timeout = 6000; // miliseconds
            webRequest.Method = "HEAD";
            try
            {
                webRequest.GetResponse();
            }
            catch
            {
                result = false;
            }
            return result;
        }
        static void Main(string[] args)
        {
            string filename="../Data/fixtures.csv";
            string url="https://www.football-data.co.uk/fixtures.csv";
            Console.WriteLine("Do you want to download fixtures? y or n");
            string ans=Console.ReadLine();
            if ((ans=="y"||ans=="Y") && URLExists(url)==true){
                if (File.Exists(filename)){
                    File.Delete(filename);
                }
                using (var myClient=new WebClient()){
                    myClient.DownloadFile(url,filename);
                }
            }

        }
    }
}
