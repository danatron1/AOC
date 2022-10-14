//Day.Solve(2015, 19);

int start = 1;
while (true)
{
    Console.Write(start);
    start = Tausworthe(start);
    Console.ReadLine();
}

int Tausworthe(int seed)
{
    seed ^= seed >> 13;
    seed ^= seed << 18;
    return seed & 0x7fffffff;
}