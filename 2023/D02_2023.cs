using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D02_2023 : Day
{
    class Game
    {
        public int ID;
        public List<Dictionary<string, int>> runs = new();
        public void AddRun(string run)
        {
            string[] colours = run.Split(',', StringSplitOptions.TrimEntries);
            Dictionary<string, int> coloursSorted = new();
            foreach (string c in colours)
            {
                string[] split = c.Split(' ');
                coloursSorted[split[1]] = int.Parse(split[0]);
            }
            runs.Add(coloursSorted);
        }
        public int GetMaxOf(string colour)
        {
            return runs.Select(x => x.TryGetValue(colour, out int count) ? count : 0).Max();
        }
        public bool PossibleWith(int red, int green, int blue)
        {
            return GetMaxOf("red") <= red
                && GetMaxOf("green") <= green
                && GetMaxOf("blue") <= blue;
        }
        public int GetPower()
        {
            return GetMaxOf("red") * GetMaxOf("green") * GetMaxOf("blue");
        }
    }
    Game GetGameFromLine(string line)
    {
        Game game = new();
        string[] split = line.Split(':');
        game.ID = int.Parse(split[0][5..]);

        string[] runSplit = split[1].Split(";");
        foreach (string run in runSplit)
        {
            game.AddRun(run);
        }
        return game;
    }
    public override void PartA()
    {
        List<Game> games = Input.Select(GetGameFromLine).ToList();
        Submit(games.Where(g => g.PossibleWith(red: 12, green: 13, blue: 14)).Sum(x => x.ID));
    }
    public override void PartB()
    {
        List<Game> games = Input.Select(GetGameFromLine).ToList();
        Submit(games.Sum(x => x.GetPower()));
    }
}
