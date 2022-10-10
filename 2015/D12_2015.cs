using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AOC
{
    internal class D12_2015 : Day
    {
        public override void PartA()
        {
            JToken a = GetInputForDayJson();
            Submit(GetSum(a));
        }
        int GetSum(JToken t, string? skip = null)
        {
            if (t.Type == JTokenType.Integer) return (int)t;
            if (t.Children().Any(c => c.ToString() == skip)) return 0;
            return t.Children().Sum(x => GetSum(x));
        }

        public override void PartB()
        {
            JToken json = JToken.Parse("[1,{\"c\":\"red\",\"b\":2},3]");
            Console.WriteLine(GetSum(json, "red"));
        }
    }
}
