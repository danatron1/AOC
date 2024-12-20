using AOC.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D13_2015 : Day
    {
        void MapHappiness()
        {
            TravellingSalesman.Reset();
            foreach (string[] row in Input.Select(s => s.Trim('.')).PullColumns(0, 2, 3, 10).Select(x => x.ToArray()))
            {
                int happiness = row[1] == "lose" ? -int.Parse(row[2]) : int.Parse(row[2]);
                TravellingSalesman.AddConnection(row[0], row[3], happiness, true);
            }
        }
        public override void PartOne()
        {
            MapHappiness();
            Submit(TravellingSalesman.GetLongest(true));
        }
        public override void PartTwo()
        {
            MapHappiness();
            Submit(TravellingSalesman.GetLongest(false));
        }
    }
}
