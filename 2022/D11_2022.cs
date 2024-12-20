using AOC.Items.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AOC.Items.Operations.Operations;

namespace AOC.Y2022;

internal class D11_2022 : Day
{
    class Monkey
    {
        public static List<Monkey> Monkeys;
        public static int worryDivisor = 1;
        public static int moduloMulti;

        public int ID;
        public List<long> Items;
        public int Inspections { get; private set; }
        public int NewInspections { get; private set; } 
        Operation operation;
        int modTest;
        int trueMonkey;
        int falseMonkey;

        delegate long Operation(long old);

        public static void Initialize(string[][] inputBlocks)
        {
            Monkeys = inputBlocks.Select(m => new Monkey(m)).ToList();
            moduloMulti = Monkeys.Select(m => m.modTest).Mul();
        }
        public Monkey(string[] inputBlock)
        {
            ID = inputBlock[0].ExtractNumber<int>();
            Items = inputBlock[1].ExtractNumbers<long>().ToList();
            MathsOperation2N<long> action = MathsOpFromChar<long>(inputBlock[2].First("*+-/%".Contains));
            if (inputBlock[2].TryExtractNumber(out long n)) operation = i => action(i, n);
            else operation = i => action(i, i);
            modTest = inputBlock[3].ExtractNumber<int>();
            trueMonkey = inputBlock[4].ExtractNumber<int>();
            falseMonkey = inputBlock[5].ExtractNumber<int>();
        }
        public static void PerformRound(int worry)
        {
            worryDivisor = worry;
            foreach (Monkey monkey in Monkeys)
            {
                monkey.NewInspections = -monkey.Inspections;
                monkey.InspectAllItems();
                monkey.NewInspections += monkey.Inspections;
            }
        }
        public void InspectAllItems() => Utility.RepeatAction(Inspect, Items.Count);
        public void Inspect()
        {
            Items[0] = operation(Items[0]);
            if (worryDivisor == 1) Items[0] %= moduloMulti;
            else Items[0] /= worryDivisor; //phew
            if (Items[0] % modTest == 0) Monkeys[trueMonkey].Items.Add(Items[0]);
            else Monkeys[falseMonkey].Items.Add(Items[0]);
            Items.RemoveAt(0);
            Inspections++;
        }
        internal static void AddInspections(long nextInspections)
        {
            for (int i = Monkeys.Count - 1; i >= 0; i--)
            {
                Monkeys[i].NewInspections = (int)(nextInspections % (1 << 8));
                Monkeys[i].Inspections += Monkeys[i].NewInspections;
                nextInspections >>= 8;
            }
        }
    }
    public override void PartOne()
    {
        Monkey.Initialize(InputBlocks);
        for (int i = 0; i < 20; i++) Monkey.PerformRound(3);
        Submit(Monkey.Monkeys.Select(m => m.Inspections).Top(2).Mul());
    }
    public override void PartTwo()
    {
        Monkey.Initialize(InputBlocks);
        for (int i = 0; i < 10_000; i++) Monkey.PerformRound(1);
        Submit(Monkey.Monkeys.Select(m => (long)m.Inspections).Top(2).Mul());
    }
}
