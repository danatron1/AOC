using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D12_2023 : Day
{
    public override void PartA()
    {
        // 7 3 1
        // 7 << ? + 3 << ? + 1 << ?
        //      ^ always has to be at least 1 + the length of the next one (e.g. 3 if it's 7)
        //          (unless it's the last one, then it can be 0)
        // 1 << 2 + 1 << 2 + 3 << 0
        // total length = 7
        // bit lengths = 1 + 1 + 3 = 5
        // gaps = [1,1,3].len - 1 = 2
        // options = 1 << (7 - 5 - 2) = 1
        //
        // proposal:
        // #.##...
        // known mask:
        // ##...##
        // value mask:
        // #......

        Submit(Input.Sum(x => Arrangements(x).Count()));
    }
    IEnumerable<uint> Arrangements(string line)
    {
        string[] split = line.Split(' ');
        string sample = split[0];
        int[] sequence = split[1].Split(',').Select(int.Parse).ToArray();
        uint knownMask = Known(sample, out uint valueMask);
        foreach (uint proposal in GenerateProposals(sequence, sample.Length))
        {
            if ((proposal & knownMask) == valueMask) yield return proposal;
        }
    }
    IEnumerable<uint> GenerateProposals(IEnumerable<int> sequence, int maxAllowedLength)
    {
        if (sequence.Any())
        {
            IEnumerable<int> rest = sequence.Skip(1);
            int restLength = rest.Sum() + rest.Count();
            int workingOn = sequence.First();
            int wiggleRoom = maxAllowedLength - workingOn - restLength;
            for (int i = 0; i <= wiggleRoom; i++)
            {
                foreach (uint remaining in GenerateProposals(rest, i + restLength - 1))
                {
                    yield return SequenceOfOnes(workingOn) << (i + restLength) | remaining;
                }
            }
        }
        else yield return 0;
        static uint SequenceOfOnes(int len)
        {
            uint num = 0;
            for (int i = 0; i < len; i++)
            {
                num = (num << 1) + 1;
            }
            return num;
        }
    }
    static uint Known(string sample, out uint valueMask)
    {
        uint k = 0;
        valueMask = 0;
        for (int i = 0; i < sample.Length; i++) 
        { 
            k <<= 1;
            valueMask <<= 1;
            if (sample[i] == '#')
            {
                valueMask++;
                k++;
            }
            else if (sample[i] == '.') k++;
        }
        return k;
    }
    void PrintBits(uint i)
    {
        Console.WriteLine(Convert.ToString(i, 2));
    }
    private static IEnumerable<string> Unfold(params string[] input)
    {
        string[] split;
        foreach (string line in input)
        {
            split = line.Split(' ');
            split[0] = string.Join('?', Enumerable.Repeat(split[0], 5));
            split[1] = string.Join(',', Enumerable.Repeat(split[1], 5));
            yield return $"{split[0]} {split[1]}";
        }
    }
    public override void PartB()
    {
        long sum = 0;
        string line = ".??.??#?. 2,3";
        //foreach (string line in Unfold(Input))
        {
            string[] split = line.Split(' ');
            Tally<int> states = new();
            List<char> stateChars = new() { '.' };
            //dot to dot
            //hash to dot
            //hash to hash
            //dot to hash
            //turns "1, 3" into ['.', '#', '.', '#', '#', '#', '.'] 
            foreach (int num in split[1].ExtractNumbers<int>())
            {
                for (int i = 0; i < num; i++) stateChars.Add('#');
                stateChars.Add('.');
            }
            states[0] = 1;
            foreach (char c in split[0])
            {
                for (int i = stateChars.Count - 1; i >= 0; i--)
                {
                    switch (c)
                    {
                        case '.':
                            if (stateChars[i] == '.') break;
                            if (stateChars[i + 1] == '.')
                            {
                                states[i + 1] += states[i];
                            }
                            states[i] = 0;
                            break;
                        case '#':
                            if (i + 1 < stateChars.Count && stateChars[i+1] == '#')
                            {
                                states[i+1] += states[i];
                            }
                            states[i] = 0;
                            break;
                        case '?':
                            if (i + 1 < stateChars.Count)
                            {
                                states[i + 1] += states[i];
                            }
                            if (stateChars[i] == '#') states[i] = 0;
                            break;
                    }
                }
                Console.WriteLine($"[{string.Join(", ", states.ToKeyValuePairs().Select(x => $"{stateChars[x.Key]}{x.Key}:{x.Value}"))}]");
            }
            sum += states[stateChars.Count - 1] + states[stateChars.Count - 2];
            Console.WriteLine($"Answer = {states[stateChars.Count - 1] + states[stateChars.Count - 2]}");
        }
        Submit(sum);
    }



    #region Attempt 2 (works but not fast enough)
    class CrawlerMother
    {
        Crawler crawler;
        internal string pattern;
        internal int[] instructions;
        internal int minimumLength;
        public int ProcessPattern(string line)
        {
            string[] split = line.Split(" ");
            pattern = split[0];
            instructions = split[1].Split(',').Select(int.Parse).ToArray();
            minimumLength = instructions.Sum() + instructions.Length - 1;
            crawler = new Crawler(this);
            return crawler.Process();
        }
    }
    class Crawler
    {
        int patternIndex;
        bool happyWithHash, happyWithDot;
        int chainLength, instrIndex;
        int chainFinishes;
        int otherSuccesses = 0;
        bool instructionsFinished = false;
        bool chainGoing => chainLength > 0;
        bool chainFull => chainLength == mother.instructions[instrIndex];
        bool wiggleRoomLeft => mother.pattern.Length - patternIndex > mother.minimumLength - chainFinishes - chainLength;

        int Clone() //the clone always starts with a hash
        {
            Crawler crawler = new Crawler(mother, instrIndex, patternIndex + 1);
            crawler.chainLength = 1;
            crawler.chainFinishes = chainFinishes;
            return crawler.Process();
        }
        private CrawlerMother mother;
        public Crawler(CrawlerMother crawlerMother, int instructionStart, int patternStart)
        {
            mother = crawlerMother;
            instrIndex = instructionStart;
            patternIndex = patternStart;
        }
        public Crawler(CrawlerMother crawlerMother) : this(crawlerMother, 0, 0) { }
        internal int Process()
        {
            for (; patternIndex < mother.pattern.Length; patternIndex++)
            {
                happyWithHash = !(instructionsFinished || chainFull);
                happyWithDot = !(happyWithHash && chainGoing) && (wiggleRoomLeft || chainGoing);
                switch (mother.pattern[patternIndex])
                {
                    case '?':
                        if (happyWithDot && happyWithHash) otherSuccesses += Clone();
                        if (happyWithDot) goto case '.'; //non-clone always prefers dot
                        else goto case '#';
                    case '.':
                        if (!happyWithDot) return otherSuccesses;
                        if (chainLength > 0)
                        {
                            chainFinishes += chainLength + 1;
                            chainLength = 0;
                            instrIndex++;
                            instructionsFinished = instrIndex == mother.instructions.Length;
                        }
                        break;
                    case '#':
                        if (!happyWithHash) return otherSuccesses;
                        chainLength++;
                        break;
                    default: throw new NotImplementedException($"Char {mother.pattern[patternIndex]} not handled");
                }
                
            }
            return 1 + otherSuccesses;
        }
    }
    #endregion
}
