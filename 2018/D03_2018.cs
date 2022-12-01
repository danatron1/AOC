namespace AOC.Y2018;

internal class D03_2018 : Day
{
    public override void PartA()
    {
        int[,] fabric = new int[1000, 1000];
        Claim[] claims = Input.Select(c => new Claim(c)).ToArray();
        foreach (var item in claims)
        {
            for (int x = item.minX; x < item.maxX; x++)
            {
                for (int y = item.minY; y < item.maxY; y++)
                {
                    fabric[x, y]++;
                }
            }
        }
        Submit(fabric.Flatten().Where(i => i > 1).Count());
    }
    public override void PartB()
    {
        Claim[] claims = Input.Select(c => new Claim(c)).ToArray();
        Submit(claims.Where(e => claims.Without(e).All(c => !c.Overlap(e))).First().ID);
    }
    class Claim
    {
        public int ID;
        public int minX, maxX;
        public int minY, maxY;
        public int area;
        public Claim(string line)
        {
            string[] lines = line.Split(' ', ',', 'x');
            ID = int.Parse(lines[0].Trim('#'));
            minX = int.Parse(lines[2]);
            minY = int.Parse(lines[3].Trim(':'));
            maxX = int.Parse(lines[4]) + minX;
            maxY = int.Parse(lines[5]) + minY;
            area = (maxX - minX) * (maxY - minY);
        }
        public bool Overlap(Claim other)
        {
            if (maxX <= other.minX || maxY <= other.minY) return false;
            if (minX >= other.maxX || minY >= other.maxY) return false;
            return true;
        }
    }
}
