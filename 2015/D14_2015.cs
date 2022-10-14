using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D14_2015 : Day
    {
        class Reindeer
        {
            public string name;
            public int speed;
            public int uptime;
            public int downtime;
            public int cycleTime => uptime + downtime;
            public Reindeer(string input)
            {
                string[] line = input.PullColumns(0, 3, 6, 13).ToArray();
                name = line[0];
                speed = int.Parse(line[1]);
                uptime = int.Parse(line[2]);
                downtime = int.Parse(line[3]);
            }
            public int DistanceAt(int seconds)
            {
                return (seconds / cycleTime * speed * uptime) + speed * Math.Min(uptime, seconds % cycleTime);
            }
        }
        const int time = 2503;
        public override void PartA()
        {
            Submit(Input.Select(c => (new Reindeer(c)).DistanceAt(time)).Max());
        }
        public override void PartB()
        {
            Reindeer[] deer = Input.Select(c => new Reindeer(c)).ToArray();
            Submit(Enumerable.Range(1, time).Select(i => deer.MaxBy(d => d.DistanceAt(i))).Mode(v => v.name).count);
        }
    }
}
