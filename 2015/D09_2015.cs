using AOC.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D09_2015 : Day
    {
        void MapCities()
        {
            TravellingSalesman.Reset();
            foreach (string item in Input)
            {
                TravellingSalesman.TryAddConnection(item.PullColumns(0, 2, 4).ToArray());
            }
        }
        public override void PartA()
        {
            MapCities();
            Submit(TravellingSalesman.GetShortest(false));
        }
        public override void PartB()
        {
            MapCities();
            Submit(TravellingSalesman.GetLongest(false));
        }
    }
}
