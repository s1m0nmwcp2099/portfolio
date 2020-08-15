using System;
using System.Collections.Generic;

namespace Sudoku
{
    class Program
    {
        static bool PuzzleCompleted(int[,] puzzleGrid){
            bool comp = true;
            for (int i = 0; i < 9; i++){
                for (int j = 0; j < 9; j++){
                    if (puzzleGrid[i, j] > 0){
                        comp = false;
                    }
                }
            }
            return comp;
        }
        static void Main(string[] args)
        {
            bool[,,] grid = new bool[9, 9, 9];
            for (int i = 0; i < 9; i++){
                for (int j = 0; j < 9; j++){
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
                    grid[i, j, Convert.ToInt32[j - 1]] = true; 
                }
            }
            while (PuzzleCompleted(grid) == false){

            }
        }
    }
}
