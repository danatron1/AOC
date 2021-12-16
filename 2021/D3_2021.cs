using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D3_2021 : Day
    {
        public override void PartA()
        {
            string[] input = GetInputForDay();
            uint gamma = 0;
            for (int i = 0;i < input[0].Length; i++)
            {
                gamma <<= 1;
                if (MostCommonBit(input, i) == '1') gamma++;
            }
            uint epsilon = ~gamma;
            epsilon %= (uint)Math.Pow(2, input[0].Length);
            Console.WriteLine($"Most common: {gamma}");
            Console.WriteLine($"Least common: {epsilon}");
            Console.Write("Power: ");
            Copy(gamma * epsilon);
        }

        public override void PartB()
        {
            List<string> oxygen = GetInputForDay().ToList();
            List<string> scrubber = GetInputForDay().ToList();
            int pos = 0;
            while (oxygen.Count > 1)
            {
                int one = 0;
                int zero = 0;
                for (int i = 0; i < oxygen.Count; i++)
                {
                    if (oxygen[i][pos] == '1') one++;
                    else zero++;
                }
                char mostCommon;// = MostCommonBit(oxygen, pos);
                if (zero > one) mostCommon = '0';
                else mostCommon = '1';
                for (int i = oxygen.Count - 1; i >= 0; i--)
                {
                    if (oxygen[i][pos] != mostCommon) oxygen.RemoveAt(i);
                }
                pos++;
            }
            pos = 0;
            while (scrubber.Count > 1)
            {
                int one = 0;
                int zero = 0;
                for (int i = 0; i < scrubber.Count; i++)
                {
                    if (scrubber[i][pos] == '1') one++;
                    else zero++;
                }
                char mostCommon;// = MostCommonBit(oxygen, pos);
                if (zero > one) mostCommon = '0';
                else mostCommon = '1';
                for (int i = scrubber.Count - 1; i >= 0; i--)
                {
                    if (scrubber[i][pos] == mostCommon) scrubber.RemoveAt(i);
                }
                pos++;
            }
            Copy(Convert.ToInt32(oxygen[0], 2) * Convert.ToInt32(scrubber[0], 2));
        }

        /*
        public override void PartB()
        {
            string[] input = GetInputForDay();
            List<string> oxygen = input.ToList();
            List<string> scrubber = input.ToList();
            for (int i = 0; i < input[0].Length; i++)
            {
                char oxygenCommon = MostCommonBit(oxygen, i);
                char scrubberCommon = MostCommonBit(scrubber, i);
                if (oxygen.Count > 1)
                {
                    for (int o = oxygen.Count - 1; o >= 0; o--)
                    {
                        if (oxygen[o][i] != oxygenCommon) oxygen.RemoveAt(o);
                    }
                }
                if (scrubber.Count > 1)
                {
                    for (int o = scrubber.Count - 1; o >= 0; o--)
                    {
                        if (scrubber[o][i] == scrubberCommon) scrubber.RemoveAt(o);
                    }
                }

                //oxygen.RemoveAll(o => o[i] != oxygenCommon);
                //if (scrubber.Count > 1) scrubber.RemoveAll(s => s[i] == scrubberCommon);
            }
            int o2 = Convert.ToInt32(oxygen[0], 2);
            int co2 = Convert.ToInt32(scrubber[0], 2);
            Console.WriteLine($"O2: {oxygen[0]} ({o2})\nCO2: {scrubber[0]} ({co2})");
            Copy(o2 * co2);
        }
        */
        char MostCommonBit(IEnumerable<string> input, int position)
        {
            if (input.Count(c => c[position] == '1') >= input.Count() / 2) return '1';
            return '0';
        }
        

        public override void Solve() => PartB();
    }
}
