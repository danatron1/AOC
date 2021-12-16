using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D7_2021 : Day
    {
        public override void Solve() => PartA();
        int[] input;
        public override void PartA()
        {
            input = Array.ConvertAll(GetInputForDay()[0].Split(','), int.Parse);
            Array.Sort(input);
            Console.WriteLine($"Median: {306}");
            Console.WriteLine($"Geometric median: {GeometricMedian()}");

            double target = GeometricMedian();
            Copy((int)SumDistanceTo(target));
            //correct answer is 340056 with a target of 307
        }
        double GeometricMedian(double accuracy = 0.1)
        {
            double target = input.Average();
            double lastTarget = 0;
            double nextStep = target;
            while (true)
            {
                nextStep /= 2;
                if (nextStep < accuracy) break;
                //Console.WriteLine($"{(target > lastTarget ? "^" : "v")} {target}");
                lastTarget = target;
                if (SumDistanceTo(target + nextStep) > SumDistanceTo(target - nextStep)) target -= nextStep;
                else target += nextStep;
            }
            return target;
        }
        double GeometricMedianExp(double accuracy = 0.1)
        {
            double target = input.Average();
            double lastTarget = 0;
            double nextStep = target;
            while (true)
            {
                nextStep /= 2;
                if (nextStep < accuracy) break;
                Console.WriteLine($"{(target > lastTarget ? "^" : "v")} {target}");
                lastTarget = target;
                if (ExpDistanceTo(target + nextStep) > ExpDistanceTo(target - nextStep)) target -= nextStep;
                else target += nextStep;
            }
            return target;
        }
        int ExpDistanceTo(double target)
        {
            int sumOfDistances = 0;
            foreach (int position in input)
            {
                sumOfDistances += Tri((int)Math.Abs(position - (int)target));
            }
            return sumOfDistances;

            static int Tri(int n) => (n * n + n) / 2;
        }
        int SumDistanceTo(double target)
        {
            double sumOfDistances = 0;
            foreach (int position in input)
            {
                sumOfDistances += Math.Abs(position - target);
            }
            return (int)sumOfDistances;
        }
        public override void PartB()
        {
            input = Array.ConvertAll(GetInputForDay()[0].Split(','), int.Parse);
            int target = (int)GeometricMedianExp(0.000000001);
            for (int i = target - 10; i < target + 10; i++)
            {
                int dist = ExpDistanceTo(i);
                if (dist >= 96592329) continue;
                Copy(dist);
            }
            //Copy(ExpDistanceTo(target));
            //96592329 too high
        }
    }
}
