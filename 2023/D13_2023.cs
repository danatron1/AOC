using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D13_2023 : Day
{
    class Mirror
    {
        internal uint[] rows;
        internal uint[] cols;
        public Mirror(string[] input)
        {
            rows = new uint[input.Length];
            cols = new uint[input[0].Length];
            for (int i = 0; i < input.Length; i++)
            {
                for(int j = 0; j < input[i].Length; j++)
                {
                    rows[i] <<= 1;
                    cols[j] <<= 1;
                    if (input[i][j] == '#')
                    {
                        rows[i]++;
                        cols[j]++;
                    }
                }
            }
        }
        bool FindReflection(uint[] mirror, bool allowSmudge, out int index)
        {
            bool smudgeUsed, impossible;
            uint diff;
            for (index = 1; index < mirror.Length; index++)
            {
                impossible = false;
                smudgeUsed = !allowSmudge;
                for (int j = index; j > 0; j--)
                {
                    if (index * 2 - j >= mirror.Length) continue;
                    diff = mirror[index * 2 - j] ^ mirror[j - 1];
                    if (diff == 0) continue;
                    if (!smudgeUsed && (diff & (diff - 1)) == 0)
                    {
                        smudgeUsed = true;
                        continue;
                    }
                    impossible = true;
                    break;
                }
                if (impossible) continue;
                if (!smudgeUsed && allowSmudge) continue; //if smudge is allowed, it MUST be used
                return true;
            }
            return false;
        }
        internal int SymmetryLine(bool allowSmudge = false)
        {
            if (FindReflection(cols, allowSmudge, out int i)) return i;
            if (FindReflection(rows, allowSmudge, out i)) return i * 100;
            return 0;
        }
    }
    public override void PartOne()
    {
        Mirror[] mirrors = InputBlocks.Select(x => new Mirror(x)).ToArray();
        Submit(mirrors.Sum(x => x.SymmetryLine()));
    }
    public override void PartTwo()
    {
        Mirror[] mirrors = InputBlocks.Select(x => new Mirror(x)).ToArray();
        Submit(mirrors.Sum(x => x.SymmetryLine(true)));
    }
}
