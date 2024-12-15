using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2024;

internal class D07_2024 : Day
{
    public override void PartOne()
    {
        Submit(CalibrationTestsWithOperators(Input, 2));
    }
    public override void PartTwo()
    {
        Submit(CalibrationTestsWithOperators(Input, 3));
    }
    static long CalibrationTestsWithOperators(string[] input, int ops)
    {
        long sum = 0;
        foreach (var test in input.Select(x => new Calibration(x)))
        {
            if (OperatorCombos(test.Values.Length - 1, ops).Any(test.TryWithOperators)) sum += test.TestValue;
        }
        return sum;
    }

    //modified from https://stackoverflow.com/questions/923771
    static IEnumerable<Operator[]> OperatorCombos(int length, int operatorOptions)
    {
        int possibleOptions = (int)Math.Pow(operatorOptions, length);
        byte[] buffer = new byte[length];
        for (int i = 0; i < possibleOptions; i++)
        {
            int x = length, value = i;
            do
            {
                buffer[--x] = (byte)(value % operatorOptions);
                value /= operatorOptions;
            }
            while (value > 0);
            Operator[] result = new Operator[length];
            Array.Copy(buffer, result, length);
            yield return result;
        }
    }
    enum Operator
    {
        Add,
        Multiply,
        Concat
    }
    struct Calibration
    {
        public long TestValue;
        public int[] Values;
        public Calibration(string inputLine)
        {
            string[] split = inputLine.Split(':');
            TestValue = long.Parse(split[0]);
            Values = split[1].ExtractNumbers<int>();
        }
        public readonly bool TryWithOperators(params Operator[] ops)
        {
            if (ops.Length + 1 != Values.Length) throw new ArgumentException("Bro wtf");
            long total = Values[0];
            for (int i = 0; i < ops.Length; i++)
            {
                switch (ops[i])
                {
                    case Operator.Add:
                        total += Values[i + 1];
                        break;
                    case Operator.Multiply:
                        total *= Values[i + 1];
                        break;
                    case Operator.Concat:
                        total = long.Parse(total.ToString() + Values[i + 1].ToString());
                        break;
                }
            }
            return total == TestValue;
        }
    }
}
