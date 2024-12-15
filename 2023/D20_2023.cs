using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D20_2023 : Day
{
    class Module
    {
        public static string Output = "rx";
        public static bool Display = false;
        public static int ButtonPresses = 0;
        public static int HighSignals = 0;
        public static int LowSignals = 0;
        public static int[] LCMCountForPartB = new int[4];
        public string Name;
        public string[] Destinations;
        protected bool Tracking = false;
        public Module(string line)
        {
            string[] split = line.Split(' ');
            Name = split[0].Trim('%', '&');
            Destinations = split[2..].Select(x => x.Trim(',')).ToArray();
        }
        public static void ResetStaticCounters()
        {
            Display = false;
            HighSignals = 0; LowSignals = 0; ButtonPresses = 0;
        }
        static string lastCmessage = "";
        internal static void Log(string module, bool signal, Module from)
        {
            if (module == "broadcaster") ButtonPresses++;
            if (signal) HighSignals++;
            else LowSignals++;
            if (Display)
            {
                Console.WriteLine($"({ButtonPresses}) {from.Name} -{(signal ? "low" : "high")}-> {module}");
            }    
            if (from.Tracking && from is ConjunctionModule c && c.Memory.Any(x => x.Value))
            {
                if (Display) Console.WriteLine($"({ButtonPresses}) {string.Join(", ", c.Memory.Select(x => $"{x.Key}:{x.Value}"))}");
                int i = 0;
                foreach (var x in c.Memory)
                {
                    if (LCMCountForPartB[i++] == 0 && x.Value) LCMCountForPartB[i-1] = ButtonPresses;
                }
            }
        }
        public virtual IEnumerable<(string, bool, string)> Signal(bool signal, string from)
        {
            Log(Name, signal, this);
            foreach (string destination in Destinations) yield return (destination, signal, Name);
        }
        public void Initialize(Dictionary<string, Module> modules)
        {
            foreach (string destination in Destinations)
            {
                if (modules.TryGetValue(destination, out Module? m) && m is ConjunctionModule conjunction)
                {
                    conjunction.Memory[Name] = false;
                }
                if (destination == Output) Tracking = true;
            }
        }
    }
    class FlipFlopModule : Module
    {
        bool State = false;
        public FlipFlopModule(string line) : base(line) { }
        public override IEnumerable<(string, bool, string)> Signal(bool signal, string from)
        {
            Log(Name, signal, this);
            if (!signal)
            {
                State = !State;
                foreach (string destination in Destinations) yield return (destination, State, Name);
            }
        }
    }
    class ConjunctionModule : Module
    {
        public Dictionary<string, bool> Memory = new();
        public ConjunctionModule(string line) : base(line) { }
        public override IEnumerable<(string, bool, string)> Signal(bool signal, string from)
        {
            Log(Name, signal, this);
            Memory[from] = signal;
            signal = Memory.Any(x => !x.Value);
            foreach (string destination in Destinations) yield return (destination, signal, Name);
        }
    }
    Dictionary<string, Module> Modules = new();
    void ResetSimulation()
    {
        Module.ResetStaticCounters();
        Module.Display = useExampleInput;
        Modules = Input.Select(ModuleType).ToDictionary(x => x.Name);
        Modules.Add("button", new Module("button "));
        Modules.ForEach(x => x.Value.Initialize(Modules)); //default conjunction modules to zero

        static Module ModuleType(string s)
        {
            return s[0] switch
            {
                '%' => new FlipFlopModule(s),
                '&' => new ConjunctionModule(s),
                _ => new Module(s)
            };
        }
    }
    void RunSimulationWhile(Func<bool> predicate)
    {
        Queue<(string, bool, string)> queue = new();
        (string module, bool signal, string from) next;
        while (predicate())
        {
            queue.Enqueue(("broadcaster", false, "button"));
            while (queue.Any())
            {
                next = queue.Dequeue();
                if (Modules.ContainsKey(next.module))
                {
                    Modules[next.module].Signal(next.signal, next.from).ForEach(queue.Enqueue);
                }
                else Module.Log(next.module, next.signal, Modules[next.from]);
            }
        }
        Console.WriteLine($"After {Module.ButtonPresses} button presses, modules received {Module.LowSignals} low signals, and {Module.HighSignals} high signals");
    }
    public override void PartOne()
    {
        //useExampleInput = true;
        ResetSimulation();
        RunSimulationWhile(() => Module.ButtonPresses < 1000);
        Submit(Module.HighSignals * Module.LowSignals);
    }
    public override void PartTwo()
    {
        //useExampleInput = true;
        ResetSimulation();
        RunSimulationWhile(() => Module.LCMCountForPartB.Any(x => x == 0));
        Submit(Module.LCMCountForPartB.LCMAsLong());
    }
}
