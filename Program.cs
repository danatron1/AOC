double ratio = 0.2;

for (int size = 9; size < 50; size++)
{
    Console.WriteLine($"{size}x{size}: {(int)Math.Ceiling(size*size*ratio)} mines");
}