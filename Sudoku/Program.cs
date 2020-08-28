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
            int[,] possibleCt = new int[9, 9];
            bool[,,] grid = new bool[9, 9, 9];
            for (int i = 0; i < 9; i++){
                for (int j = 0; j < 9; j++){
                    possibleCt[i, j] = 9;
                    for (int k = 0; k < 9; k++){
                        grid[i, j, k] = false;
                    }
                }
            }
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
            for (int i = 0; i < 9; i++){
                string[] thisLine = Lines[i].Split(',');
                for (int j = 0; j < 9; j++){
                    int num = Convert.ToInt32(thisLine[j]);
                    if (num > 0){
                        grid[i, j, num - 1] = true;
                        possibleCt[i, j] = 1;
                    }else{
                        for (int k = 0; k < 9; k++){
                            grid[i, j, k] = true;
                        }
                    }
                }
            }
            
            //GO THROUGH EACH SQUARE
            for (int rw = 0; rw < 9; rw++){
                for (int cl = 0; cl < 9; cl++){
                    (int, int) here = (rw, cl);
                    //get vertical, horizontal and subsquare coordinates for comparison
                    List<(int, int)> VertTheres = new List<(int, int)>();
                    List<(int, int)> HorizTheres = new List<(int, int)>();
                    List<(int, int)> SubTheres = new List<(int, int)>();
                    for (int i = 0; i < 9; i++){
                        VertTheres.Add((i, cl));
                        HorizTheres.Add((rw, i));
                    }
                    //get subsquare  coords for comparison
                    int subRw = rw / 3;
                    int subCl = cl / 3;
                    for (int i = 0; i < 3; i++){
                        for (int j = 0; j < 3; j++){
                            SubTheres.Add((subRw * 3 + i, subCl * 3 + j));
                        }
                    }
                    //here.Item1
                    //Compare vertical
                    for (int i = 1; i <= 9; i++){//check where other('there') square can be i possibilities
                        List<List<int>> AvNos = new List<List<int>>();
                        for (int thr = 0; thr < 9; thr++){
                            if (thr != here.Item1 && possibleCt[VertTheres[thr].Item1, VertTheres[thr].Item2] == i){
                                AvNos.Add(new List<int>());
                                for (int num = 1; num <= 9; num++){
                                    if (grid[VertTheres[thr].Item1, VertTheres[thr].Item2, num - 1] == true){
                                        AvNos[AvNos.Count - 1].Add(num);
                                    }
                                }
                            }
                        }
                        for (int j = 0; j < AvNos.Count - i; j++){
                            int matches = 0;
                            for (int k = j + 1; k < AvNos.Count; k++){
                                if (AvNos[j].SequenceEqual(AvNos[k])){
                                    matches++;
                                }
                            }
                            if (matches >= i){
                                
                            }
                        }
                    }
                }
            }
        }
    }
}
