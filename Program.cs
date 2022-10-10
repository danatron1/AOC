string[] colours = { "White", "Blue", "Black", "Red", "Green" };

foreach (string[] item in colours.Pairs())
{
    Console.WriteLine($"[{string.Join(", ", item)}]");
}

string s = "the quick brown fox jumps over the lazy dog";

Console.WriteLine(s.FrequencyOf("the"));