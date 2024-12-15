using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D16_2021 : Day
    {
        internal interface Packet
        {
            public byte Version { get; }
            public byte Type { get; }
            public int PacketLength { get; }
            public int SumOfVersions();
        }
        struct Literal : Packet
        {
            public byte Version { get; set; }
            public byte Type => 4;
            public byte[] Segments;
            public int Value => GetValueFromSegments();
            public int PacketLength => 6 + 5 * Segments.Length;
            public int PacketLengthPadded => HexLength * 4;
            public int HexLength => (5 * Segments.Length + 9) / 4;
            public int SumOfVersions() => Version;
            private int GetValueFromSegments()
            {
                int v = 0;
                for (int i = 0; i < Segments.Length; i++)
                {
                    v <<= 4;
                    v += Segments[i] % 0b10000;
                }
                return v;
            }
            public Literal(string input)
            {
                while (true)
                {
                    //string chunk = Convert.ToInt64(input.Substring(0, 16), 16);
                    while (true)
                    {

                    }
                }
            }
        }
        class Operator : Packet
        {
            public byte Version { get; set; }
            public byte Type { get; set; }
            public bool LengthType;
            // true = 1 = number of subpackets
            //false = 0 = length in bits of subpackets

            public int LengthValue;
            public int PacketLength { get; set; }
            public Packet[] SubPackets;

            public Operator()
            {
                SubPackets = new Packet[0];
            }

            public Operator(byte[] b)
            {
                Version = (byte)(b[0] >> 5);
                Type = (byte)((b[0] >> 2) % 8);
                LengthType = (b[0] >> 1) % 2 == 1;
            }

            public int SumOfVersions()
            {
                int versionSum = Version;
                foreach (Packet p in SubPackets) versionSum += p.SumOfVersions();
                return versionSum;
            }
        }
        class BitReader
        {
            public byte[] bits;
            public BitReader(byte[] Bits)
            {
                bits = Bits;
                Pointer = 0;
            }
            int Pointer;
            int BitPointer => Pointer % 8;
            int BytePointer => Pointer / 8;
            int NextBits(int count)
            {
                int result = 0;
                int rightshift = Math.Max(0, 8 - count - BitPointer);
                int leftmod = 1 << count;




                if (BitPointer + count < 8) //if this taking wouldn't finish a byte
                {
                    result = (bits[BytePointer] >> 8 - BitPointer - count) & ((1 << count) - 1);
                }
                else
                {
                    //finish first byte
                    result = bits[BytePointer] & ((1 << 8 - BitPointer) - 1);
                    count -= 8 - BitPointer;
                    //middle bytes
                    while (count >= 8)
                    {
                        result <<= 8;
                        result += bits[BytePointer];

                    }
                }
                return result;
            }

        }
        public override void PartOne()
        {
            //3  5
            byte test = 0b01110110;
            int pointer = 3;
            int length = 3;

            Console.WriteLine(Convert.ToString((test >> 8 - pointer - length) & ((1 << length) - 1), 2).PadLeft(8, '0'));
            int rightshift = test >> 8 - pointer - length;
            Console.WriteLine(Convert.ToString(rightshift, 2).PadLeft(8, '0'));
            int mask = (1 << length) - 1;
            Console.WriteLine(Convert.ToString(mask, 2).PadLeft(8, '0'));
            Console.WriteLine(Convert.ToString(rightshift & mask, 2).PadLeft(8, '0'));

            Console.ReadLine();
            byte[] input = HexToBytes(GetInputForDay()[0]);
            Operator BITS = new Operator(input);
            Copy(BITS.SumOfVersions());
        }
        static byte[] HexToBytes(string hex)
        {
            if (hex.Length % 2 == 1) hex += "0";
            byte[] output = new byte[hex.Length >> 1];
            for (int i = 0; i < output.Length; ++i)
            {
                output[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
            }
            return output;
            int GetHexVal(char hex) => hex - (hex < 58 ? 48 : 55);
        }
        public override void PartTwo()
        {
            throw new NotImplementedException();
        }

        public override void Solve() => PartOne();
        public int Double(int x) => x * 2;
    }
}
