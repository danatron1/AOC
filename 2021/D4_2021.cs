using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D4_2021 : Day
    {
        public override void Solve() => PartTwo();

        public override void PartOne()
        {
            EstablishBoards();
            foreach (int num in numbersToDraw)
            {
                if (MarkAllBoards(num) > 0) break;
            }
            Console.WriteLine(BingoBoard.mostRecentWin);
        }

        public override void PartTwo()
        {
            EstablishBoards();
            int lastWinCount = 0;
            foreach (int num in numbersToDraw)
            {
                int wins = MarkAllBoards(num);
                if (wins == lastWinCount) continue;
                Console.WriteLine($"Called number {num,-2}, now at {wins,-2} wins (+{wins - lastWinCount})");
                lastWinCount = wins;
                if (wins == 1) Console.WriteLine($"First win!\n{BingoBoard.mostRecentWin}\n");
                if (wins == boards.Count) Console.WriteLine($"Last win!\n{BingoBoard.mostRecentWin}\n");
            }
        }

        int[] numbersToDraw;
        List<BingoBoard> boards;
        void EstablishBoards()
        {
            boards = new List<BingoBoard>();
            string[] input = GetInputForDay();
            string[] numsToDraw = input[0].Split(',');
            numbersToDraw = new int[numsToDraw.Length];
            for (int i = 0; i < numbersToDraw.Length; i++)
            {
                numbersToDraw[i] = int.Parse(numsToDraw[i]);
            }
            for (int i = 2; i < input.Length; i += 6)
            {
                boards.Add(new BingoBoard(input[i..(i + 5)]));
            }
        }
        int MarkAllBoards(int num)
        {
            int numberOfBingos = 0;
            foreach (BingoBoard board in boards)
            {
                if (board.MarkNumber(num)) numberOfBingos++;
            }
            return numberOfBingos;
        }
    }

    public class BingoBoard
    {
        public static BingoBoard? mostRecentWin = null;
        const int boardSize = 5;
        int[] numbers;
        bool[] marked = new bool[boardSize * boardSize];
        int lastCalled = -1;
        public bool bingo = false;
        public override string ToString()
        {
            int score = Score();
            string board = $"Bingo: {bingo}, Last number: {lastCalled}, Score: {score}";
            for (int i = 0; i < numbers.Length; i++)
            {
                if (i % boardSize == 0) board += "\n";
                if (marked[i]) board += $"[{numbers[i],-2}] ";
                else board += $" {numbers[i],-4}";
            }
            if (score > 0) Utility.Copy(score);
            return board;
        }
        public BingoBoard(string[] bb)
        {
            List<int> nums = new List<int>();
            for (int i = 0; i < bb.Length; i++)
            {
                string[] values = bb[i].Split(' ');
                for (int v = 0; v < values.Length; v++)
                {
                    if (values[v].Length == 0) continue;
                    nums.Add(int.Parse(values[v]));
                }
            }
            numbers = nums.ToArray();
            if (numbers.Length != marked.Length) throw new Exception("WTF?");
        }
        public bool MarkNumber(int num)
        {
            int index = Array.IndexOf(numbers, num);
            if (index != -1) marked[index] = true;
            else return bingo;
            lastCalled = num;
            if (!bingo && HasWin())
            {
                bingo = true;
                mostRecentWin = this;
            }
            return bingo;
        }
        bool HasWin()
        {
            //horizontal rows
            int[] horizontals = new int[boardSize];
            int[] verticals = new int[boardSize];

            for (int i = 0; i < marked.Length; i++)
            {
                if (!marked[i]) continue;
                horizontals[i / boardSize]++;
                verticals[i % boardSize]++;
            }
            return horizontals.Contains(boardSize) || verticals.Contains(boardSize);
        }
        public int Score()
        {
            if (!bingo) return 0;
            int sumOfUnmarked = 0;
            for (int i = 0; i < numbers.Length; i++)
            {
                if (marked[i]) continue;
                sumOfUnmarked += numbers[i];
            }
            return sumOfUnmarked * lastCalled;
        }
    }
}
