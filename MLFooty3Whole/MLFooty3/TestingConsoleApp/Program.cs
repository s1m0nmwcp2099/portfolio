using System;
using System.Collections.Generic;
using System.IO;

namespace TestingConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
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
                string fileName="../Data/suggestedBets.csv";
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
