using System.Text.RegularExpressions;

namespace AOC.Y2025;

internal class D02_2025 : Day
{
    internal class ItemRange
    {
        public string lowerBoundString, upperBoundString;
        public long lowerBound, upperBound;

        public ItemRange(string line)
        {
            string[] lines = line.Split('-');
            lowerBoundString = lines[0];
            upperBoundString = lines[1];
            lowerBound = long.Parse(lowerBoundString);
            upperBound = long.Parse(upperBoundString);
        }
        public IEnumerable<long> InvalidIDsPartA()
        {
            int lowerDigits = lowerBoundString.Length;
            int upperDigits = upperBoundString.Length;

            int start = lowerDigits % 2 == 0 ? int.Parse(lowerBoundString[..(lowerDigits / 2)]) : (int)Math.Pow(10, lowerDigits / 2);
            int end = upperDigits % 2 == 0 ? int.Parse(upperBoundString[..(upperDigits / 2)]) : int.Parse("9".Repeat(upperDigits / 2));

            for (int i = start; i <= end; i++)
            {
                long number = long.Parse($"{i}{i}");
                if (IsInRange(number)) yield return number;
            }
        }
        public bool IsInRange(long number) => number >= lowerBound && number <= upperBound;

        public IEnumerable<long> InvalidIDsPartB()
        {
            Regex regex = new Regex("\\b(\\d+)\\1+\\b");
            for (long i = lowerBound; i <= upperBound; i++)
            {
                if (regex.IsMatch(i.ToString())) yield return i;
            }
        }
    }
    public override void PartOne()
    {
        Submit(InputLine.Split(",").Select(l => new ItemRange(l)).Sum(x => x.InvalidIDsPartA().Sum()));
    }
    public override void PartTwo()
    {
        Submit(InputLine.Split(",").Select(l => new ItemRange(l)).Sum(x => x.InvalidIDsPartB().Sum()));
    }
}
