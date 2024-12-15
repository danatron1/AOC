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
    internal class D12_2015 : Day<JToken>
    {
        public override void PartOne()
        {
            Submit(GetSum(InputLine));
        }
        int GetSum(string s, string? skip = null) => GetSum(JToken.Parse(s), skip);
        int GetSum(JToken t, string? skip = null)
        {
            if (t.Type == JTokenType.Integer) return (int)t;
            if (t is JObject o && o.Properties().Any(j => j.Value.ToString() == skip)) return 0;
            return t.Children().Sum(x => GetSum(x, skip));
        }
        public override void PartTwo()
        {
            Submit(GetSum(InputLine, "red"));
        }
    }
}
