using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            //SET UP PUZZLE
            List<string> Lines = new List<string>();
            Lines.Add("2,0,0,7,0,0,0,0,0");
            Lines.Add("5,0,0,0,0,1,0,0,0");
            Lines.Add("0,0,8,0,0,9,0,6,3");
            Lines.Add("9,0,0,0,0,4,0,0,0");
            Lines.Add("0,0,0,0,0,0,8,3,0");
            Lines.Add("0,4,1,0,6,0,0,0,9");
            Lines.Add("0,9,7,0,0,8,6,0,0");
            Lines.Add("0,0,4,0,0,0,0,1,5");
            Lines.Add("0,0,0,0,0,0,3,0,0");
            int[,] grid = new int[9, 9];
            for (int i = 0; i < 9; i++){
                string[] cells = Lines[i].Split(',');
                for (int j = 0; j < 9; j++){
                    grid[i, j] = Convert.ToInt32(cells[j]);
                }
            }

            //SOLVE
            bool solved = false;
            while (solved == false){
                solved = true;
                //go through each square checking off single numbers
                for (int i = 0; i < 9; i++){
                    for (int j = 0; j < 9; j++){
                        if (grid[i, j] == 0){
                            List<int> PossibleNos = new List<int>();
                            for (int x = 1; x <= 9; x++){
                                PossibleNos.Add(x);
                            }
                            for (int k = 0; k < 9; k++){
                                //check vertically
                                if (grid[k, j] > 0){
                                    PossibleNos.Remove(grid[k, j]);
                                }
                                //check horizontally
                                if (grid[i, k] > 0){
                                    PossibleNos.Remove(grid[i, k]);
                                }
                            }
                            //check subsquare
                            for (int si = i / 3; si < i / 3 + 3; si++){
                                for (int sj = j / 3; sj < j / 3 + 3; sj++){
                                    if (grid[si, sj] > 0){
                                        PossibleNos.Remove(grid[si, sj]);
                                    }
                                }
                            }
                            if (PossibleNos.Count == 1){
                                grid[i, j] = PossibleNos[0];
                            }else{
                                solved = false;
                            }
                        }
                        Console.Write(grid[i, j] + " ");
                    }
                    Console.Write("\n");
                }
                Console.ReadLine();
            }
        }
    }
}
