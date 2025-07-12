using System.Security.Cryptography;
using System.Text;

namespace AOC.Y2016;

internal class D05_2016 : Day
{
    public override void PartOne()
    {
        Submit(new string(GetCharacters(InputLine).Take(8).ToArray()));
    }
    public override void PartTwo()
    {
        char[] password = new char[8];
        HashSet<char> toDo = new() { '0', '1', '2', '3', '4', '5', '6', '7' };
        string hash;
        for (int i = 0; i < int.MaxValue; i++)
        {
            hash = Hash(InputLine + i);
            if (hash.StartsWith("00000") && toDo.Contains(hash[5]))
            {
                toDo.Remove(hash[5]);
                password[int.Parse(hash[5].ToString())] = hash[6];
                if (toDo.Count == 0) break;
            }
        }
        Submit(new string(password));
    }
    string Hash(string input)
    {
        // Use input string to calculate MD5 hash
        using MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes);
    }
    IEnumerable<char> GetCharacters(string password)
    {
        string hash;
        for (int i = 0; i < int.MaxValue; i++)
        {
            hash = Hash(password + i);
            if (hash.StartsWith("00000")) yield return hash[5];
        }
    }
}
