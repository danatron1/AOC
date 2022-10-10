using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static AOC.D3_2019;

namespace AOC
{
    internal class D07_2015 : Day
    {
        public override void PartA()
        {
            SetupCircuit();
            Submit(Wire.GetValueOf("a"));
        }
        public override void PartB()
        {
            SetupCircuit();
            ushort aValue = Wire.GetValueOf("a");
            Wire.FlushCircuit();
            Wire.GetWire("b").cached = aValue;
            Submit(Wire.GetValueOf("a"));
        }
        void SetupCircuit()
        {
            string[] instructions = GetInputForDay(example: false);
            foreach (string instruction in instructions)
            {
                Wire.Create(instruction);
            }
            Wire.FlushCircuit();
            Console.WriteLine("Generated circuit");
        }
        class Wire
        {
            public static List<Wire> circuit = new();

            public string name;
            public Wire? inputA;
            public Wire? inputB;
            public string mode;
            public ushort stored;
            public ushort? cached;
            public Wire(string wire)
            {
                name = wire;
                if (ushort.TryParse(name, out stored)) mode = "LITERAL";
                circuit.Add(this);
            }
            public static void Create(string inputLine)
            {
                string[] parts = inputLine.Split(" -> ");
                Wire w = GetWire(parts[1]);
                string[] ins = parts[0].Split(' ');
                if (ins.Length == 1)
                {
                    w.mode = "TRANS";
                    w.inputA = GetWire(ins[0]);
                }
                else if (ins.Length == 2)
                {
                    w.mode = ins[0];
                    w.inputA = GetWire(ins[1]);
                }
                else
                {
                    w.mode = ins[1];
                    w.inputA = GetWire(ins[0]);
                    w.inputB = GetWire(ins[2]);
                }
            }

            internal static void FlushCircuit()
            {
                foreach (Wire wire in circuit)
                {
                    wire.cached = null;
                }
            }

            public static Wire GetWire(string name)
            {
                return circuit.FirstOrDefault(x => x.name == name) ?? new(name);
            }
            public static ushort GetValueOf(string wireName)
            {
                return circuit.First(x => x.name == wireName).Value();
            }
            public ushort Value()
            {
                if (cached.HasValue) return cached.Value;
                cached = mode switch
                {
                    "AND" =>    (ushort)(inputA.Value() & inputB.Value()),
                    "OR" =>     (ushort)(inputA.Value() | inputB.Value()),
                    "RSHIFT" => (ushort)(inputA.Value() >> inputB.Value()),
                    "LSHIFT" => (ushort)(inputA.Value() << inputB.Value()),
                    "NOT" =>    (ushort)~inputA.Value(),
                    "TRANS" =>  inputA.Value(),
                    _ => stored
                };
                Console.WriteLine($"Output from {name} found to be {cached.Value}");
                return cached.Value;
            }
        }
    }
}
