using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D04_2015 : Day
    {
        public override void PartA()
        {
            FindMD5StartingWith("00000");
        }
        public override void PartB()
        {
            Submit(FindMD5StartingWith("000000"));
        }













        int FindMD5StartingWith(string sequence)
        {
            string input = GetInputForDay()[0];
            for (int i = 0; i < int.MaxValue; i++)
            {
                if (CreateMD5(input + i).StartsWith(sequence))
                {
                    Console.WriteLine($"{input + i} generates {CreateMD5(input + i)}");
                    return i;
                }
            }
            return -1;
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
