using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D6_2021 : Day
    {
        public override void Solve() => PartTwo();
        public override void PartOne()
        {
            LoadFishData();
            AdvanceDays(80);
            Copy(fish.Sum());
        }
        class Test
        {
            public int price;
            public Test(int i) => price = i;
        }
        public override void PartTwo()
        {
            Test[] tests =
            {
                new Test(5),
                new Test(3),
                new Test(4),
                new Test(2)
            };

            Test[] testsAbove3 = tests.OrderBy(o => o.price).ToArray();

            foreach (var item in testsAbove3)
            {
                //Console.WriteLine(item.price);
            }

            //double av = tests.Average(o => o.price);
            //Console.WriteLine(av);


            Console.ReadLine();

            LoadFishData();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            AdvanceDays(256);
            long answer = fish.Sum();
            sw.Stop();
            Console.WriteLine($"256 days calculated in {sw.ElapsedMilliseconds} ms");
            Copy(answer);
        }
        Tally<int> fish;
        void LoadFishData()
        {
            string[] inputRaw = GetInputForDay();
            int[] input = inputRaw[0].Split(',').Select(int.Parse).ToArray();
            fish = new Tally<int>();
            fish.AddRange(input);
        }
        void AdvanceDays(int days)
        {
            for (int day = 0; day < days; day++)
            {
                Tally<int> nextfish = new Tally<int>();
                nextfish.Add(8, fish[0]);
                nextfish.Add(6, fish[0]);
                for (int i = 0; i < 8; i++)
                {
                    nextfish.Add(i, fish[i + 1]);
                }
                fish = nextfish;
            }
        }
    }
}
