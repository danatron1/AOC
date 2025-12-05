namespace AOC.Y2025;

internal class D01_2025 : Day
{
    const int DIAL_LIMIT = 100;
    public override void PartOne()
    {
        int zeroes = 0;
        int dial = 50;

        foreach (string line in Input)
        {
            ApplyLine(line);
            if (dial == 0) zeroes++;
        }
        Submit(zeroes);

        void ApplyLine(string line)
        {
            int number = line.ExtractNumber<int>();
            switch (line[0])
            {
                case 'L':
                    dial -= number;
                    while (dial < 0) dial += DIAL_LIMIT;
                    break;
                case 'R':
                    dial += number;
                    while (dial >= DIAL_LIMIT) dial -= DIAL_LIMIT;
                    break;
                default: throw new NotImplementedException();
            }
        }
    }


    public override void PartTwo()
    {
        int dial = 50;
        int loops = 0;

        foreach (string line in Input)
        {
            ApplyLine(line);
        }
        Submit(loops);

        void ApplyLine(string line) //now using password method 0x434C49434B 
        {
            int before = dial;
            int number = line.ExtractNumber<int>();
            switch (line[0])
            {
                case 'L':
                    if (dial == 0) dial = DIAL_LIMIT;
                    dial -= number;
                    while (dial < 0)
                    {
                        dial += DIAL_LIMIT;
                        loops++;
                    }
                    break;
                case 'R':
                    dial += number;
                    while (dial > DIAL_LIMIT)
                    {
                        dial -= DIAL_LIMIT;
                        loops++;
                    }
                    if (dial == DIAL_LIMIT) dial = 0;
                    break;
                default: throw new NotImplementedException();
            }
            if (dial == 0) loops++;
        }
    }

    
}
