using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D8_2021 : Day
    {
        public override void Solve() => PartB();
        public override void PartA()
        {
            string[] input = GetInputForDay();
            int count = 0;
            int[] lengthsThatMatter = { 2, 3, 4, 7 };
            for (int i = 0; i < input.Length; i++)
            {
                //remove the bits we don't need for part 1
                input[i] = input[i].Substring(input[i].IndexOf('|') + 1).Trim();
                string[] digits = input[i].Split(' ');
                foreach (string digit in digits)
                {
                    if (lengthsThatMatter.Contains(digit.Length)) count++;
                }
            }
            Copy(count);
        }

        int[] LilysMethod()
        {
            int[] validValues = new int[] { 42, 17, 34, 39, 30, 37, 41, 25, 49, 45 };
            string[] inputs = GetInputForDay();
            List<int> answer = new List<int>();
            foreach (string item in inputs)
            {
                string[] input = item.Split(" | ");
                string order = "abcdefg";
                int[] freqs = new int[7];
                for (int i = 0; i < freqs.Length; i++)
                {
                    freqs[i] = input[0].FrequencyOf(order[i]);
                }
                string[] outputs = input[1].Split(' ');
                int outputValue = 0;
                for (int i = 0; i < outputs.Length; i++)
                {
                    outputValue *= 10;
                    int value = 0;
                    foreach (char c in outputs[i])
                    {
                        value += freqs[order.IndexOf(c)];
                    }
                    int index = Array.IndexOf(validValues, value);
                    if (index == -1) Console.WriteLine($"Value of {value} not found for {outputs[i]}");
                    outputValue += index;
                }
                answer.Add(outputValue);
            }
            return answer.ToArray();
        }
        int[] MyMethod()
        {
            string[] input = GetInputForDay();
            string[] validPatterns = { "abcefg", "cf", "acdeg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg" };
            List<int> answers = new List<int>();
            foreach (string item in input)
            {
                string[] split = item.Split(" | ");
                string[] numbers = split[0].Split(' ');
                string[] outputs = split[1].Split(' ');
                string[] permutations = AllPermutations("abcdefg");
                string correctMapping = "";
                foreach (string permutation in permutations)
                {
                    int correctInputs = 0;
                    foreach (string number in numbers)
                    {
                        if (validPatterns.Contains(Untangle(number, permutation))) correctInputs++;
                    }
                    if (correctInputs == 10)
                    {
                        correctMapping = permutation;
                        break;
                    }
                }
                string answer = "";
                foreach (string output in outputs)
                {
                    answer += Array.IndexOf(validPatterns, Untangle(output, correctMapping)).ToString();
                }
                answers.Add(int.Parse(answer));
            }
            return answers.ToArray();
        }
        public override void PartB()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int[] lilyAnswer = LilysMethod();
            sw.Stop();
            Console.WriteLine($"Got answer of {lilyAnswer.Sum()} in {sw.ElapsedMilliseconds}ms using Lily's method");
            sw.Restart();
            int[] myAnswer = MyMethod();
            sw.Stop();
            Console.WriteLine($"Got answer of {myAnswer.Sum()} in {sw.ElapsedMilliseconds}ms using my method");

            for (int i = 0; i < myAnswer.Length; i++)
            {
                if (myAnswer[i] == lilyAnswer[i]) continue;
                Console.WriteLine($"Line {i}, I got {myAnswer[i]} while lily got {lilyAnswer[i]}");
            }
        }
        string Untangle(string tangled, string mapping, string correct = "abcdefg")
        {
            char[] tangle = tangled.ToCharArray();
            for (int i = 0; i < tangle.Length; i++)
            {
                tangle[i] = correct[mapping.IndexOf(tangle[i])];
            }
            Array.Sort(tangle);
            return new string(tangle);
        }
        public static string[] AllPermutations(string list = "abcdefg")
        {
            List<string> output = list.ToCharArray().Select(c => c.ToString()).ToList();
            while (output[0].Length < list.Length)
            {
                foreach (char c in list)
                {
                    if (output[0].Contains(c)) continue;
                    output.Add(output[0] + c);
                }
                output.RemoveAt(0);
            }
            return output.ToArray();
        }

    }
}
