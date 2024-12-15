using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D05_2023 : Day
{
    struct MapRange
    {
        public long start, end, effect;
        public long endEffect => end + effect;
        public long startEffect => start + effect;
        public SeedRange ToSeedRange()
        {
            return new SeedRange(start + effect, end+  effect);
        }
    }
    class Conversion
    {
        MapRange[] changes;
        string name;
        public Conversion(string[] inputBlock)
        {
            name = inputBlock[0].Trim(':');
            changes = new MapRange[inputBlock.Count()-1];
            for (int i = 1; i <= changes.Length; i++)
            {
                long[] numbers = inputBlock[i].ExtractNumbers<long>();
                changes[i-1].start = numbers[1];
                changes[i-1].end = numbers[1] + numbers[2];
                changes[i-1].effect = numbers[0] - numbers[1];
            }
        }
        public long Pass(long pass)
        {
            foreach (MapRange change in changes)
            {
                if (pass >= change.start && pass < change.end) return pass + change.effect;
            }
            return pass;
        }
        public SeedRange[] Pass(SeedRange pass)
        {
            List<SeedRange> nextPass = new();
            if (pass.start == pass.end) return nextPass.ToArray();
            foreach (var change in changes)
            {
                bool startInRange = pass.start >= change.start && pass.start < change.end;
                bool endInRange = pass.end > change.start && pass.end <= change.end;

                // ---<------------->---
                //       |------|
                if (startInRange && endInRange) return new SeedRange[] { pass.Effect(change.effect) };
                // ------<------>-------
                //    |------------|
                if (pass.start <= change.start && pass.end >= change.end)
                {
                    nextPass.Add(change.ToSeedRange());
                    nextPass.AddRange(Pass(new SeedRange(pass.start, change.start)));
                    nextPass.AddRange(Pass(new SeedRange(change.end, pass.end)));
                    return nextPass.ToArray();
                }
                // ---<---->-------------
                //            |------|
                if (!(startInRange || endInRange)) continue;

                // ---<------->----------
                //        |----------|
                if (startInRange)
                {
                    nextPass.Add(new SeedRange(pass.start + change.effect, change.endEffect));
                    nextPass.AddRange(Pass(new SeedRange(change.end, pass.end)));
                    return nextPass.ToArray();
                }
                // --------<-------->----
                //   |----------|
                if (endInRange)
                {
                    nextPass.Add(new SeedRange(change.startEffect, pass.end + change.effect));
                    nextPass.AddRange(Pass(new SeedRange(pass.start, change.start)));
                    return nextPass.ToArray();
                }
            }
            return new SeedRange[] { pass };
        }
    }
    public override void PartOne()
    {
        List<Conversion> conversions = new();
        foreach (string[] block in InputBlocks.Skip(1))
        {
            conversions.Add(new Conversion(block));
        }
        long[] seeds = InputBlocks[0][0].ExtractNumbers<long>();
        foreach (Conversion conversion in conversions)
        {
            for (int i = 0; i < seeds.Length; i++)
            {
                seeds[i] = conversion.Pass(seeds[i]);
            }
        }
        Submit(seeds.Min());
    }
    struct SeedRange
    {
        public long start, end;
        public SeedRange(long s, long e)
        {
            start = s; end = e;
        }

        internal SeedRange Effect(long effect)
        {
            return new SeedRange(start + effect, end + effect);
        }
    }
    public override void PartTwo()
    {
        List<Conversion> conversions = new();
        foreach (string[] block in InputBlocks.Skip(1))
        {
            conversions.Add(new Conversion(block));
        }
        long[] seedIndexes = InputBlocks[0][0].ExtractNumbers<long>();
        List<SeedRange> seeds = new();
        for (int i = 0; i < seedIndexes.Length; i+=2)
        {
            seeds.Add(new SeedRange(seedIndexes[i], seedIndexes[i] + seedIndexes[i+1]));
        }
        foreach (Conversion conversion in conversions)
        {
            List<SeedRange> nextGen = new();
            foreach (SeedRange seed in seeds)
            {
                nextGen.AddRange(conversion.Pass(seed));
            }
            seeds = nextGen.Where(x => x.start != x.end).ToList();
        }
        Submit(seeds.MinBy(x => x.start).start);
    }
}
