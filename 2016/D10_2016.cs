namespace AOC.Y2016;

internal class D10_2016 : Day
{
    public override void PartOne()
    {
        useExampleInput = true;

        //WIP
        return;

        int[] robots = new int[255];
        int[] outputs = new int[255];
        Dictionary<int, (int low, int high)> robotRules = new();

        foreach (string line in Input)
        {
            int[] numbers = line.ExtractNumbers<int>();
            switch (line[..3])
            {
                case "bot":
                    int low = numbers[1];
                    int high = numbers[2];
                    if (line.Split("low to ")[1][0] == 'o') low = -low;
                    if (line.Split("high to ")[1][0] == 'o') high = -high;
                    robotRules.Add(numbers[0], (low, high));
                    break;
                case "val":
                    break;
                default: throw new NotImplementedException();
            }
        }
        foreach (int robot in robots.Order())
        {
            Console.WriteLine(robot);
        }

        void GiveToBot(int value, int bot)
        {

        }
    }
    public override void PartTwo()
    {
        throw new NotImplementedException();
    }
}
