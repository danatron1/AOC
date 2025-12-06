namespace AOC.Y2025;

internal class D06_2025 : Day
{
    public override void PartOne()
    {
        int[][] numbers = [ Input[0].ExtractNumbers<int>(), Input[1].ExtractNumbers<int>(), Input[2].ExtractNumbers<int>(), Input[3].ExtractNumbers<int>() ];
        long sum = 0;
        int problem = 0;
        for (int i = 0; i < Input[4].Length; i++)
        {
            if (Input[4][i] == ' ') continue;
            if (Input[4][i] == '*') sum += (long)numbers[0][problem] * numbers[1][problem] * numbers[2][problem] * numbers[3][problem];
            if (Input[4][i] == '+') sum += numbers[0][problem] + numbers[1][problem] + numbers[2][problem] + numbers[3][problem];
            problem++;
        }
        Submit(sum);
    }
    public override void PartTwo()
    {
        int maxLength = Input.Max(x => x.Length);
        bool mult = false;
        List<int> numbers = new();
        long sum = 0;
        for (int scanLine = 0; scanLine < maxLength; scanLine++)
        {
            if (int.TryParse($"{Input[0][scanLine]}{Input[1][scanLine]}{Input[2][scanLine]}{Input[3][scanLine]}", out int number))
            {
                numbers.Add(number);
                if (Input[4][scanLine] != ' ') mult = Input[4][scanLine] == '*';
            }
            else Calculate();
        }
        Calculate();
        Submit(sum);

        void Calculate()
        {
            if (mult) sum += numbers.MulAsLong();
            else sum += numbers.Sum();
            numbers.Clear();
        }
    }
}
