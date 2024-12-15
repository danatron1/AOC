using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2018;

internal class D04_2018 : Day
{
    public override void PartOne()
    {
        //sort data
        List<GuardTime> guardTimes = new();
        foreach (var item in Input)
        {
            guardTimes.Add(new(DateTime.ParseExact(item[1..17], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), item[19..]));
        }
        guardTimes.Sort();
        Guard currentGuard = new(-1);
        HashSet<Guard> guards = new();
        for (int i = 0; i < guardTimes.Count; i++)
        {
            if (guardTimes[i].message.Contains("begins shift"))
            {
                int guardID = guardTimes[i].message.ExtractNumber<int>();
                currentGuard = guards.Where(i => i.ID == guardID).FirstOrDefault() ?? new(guardID);
                guards.Add(currentGuard);
            }
            else if (guardTimes[i].message.Contains("wakes up"))
            {
                currentGuard.MinutesAsleep += (guardTimes[i].Time - guardTimes[i - 1].Time).Minutes;
                //godsSleepiestWarriors[currentGuard] += (guardTimes[i].Time - guardTimes[i - 1].Time).Minutes;
            }
        }

        Tally<int> godsSleepiestWarriors = new();
        for (int i = 0; i < guardTimes.Count; i++)
        {
            
        }
        int sleepiestGuard = godsSleepiestWarriors.Maximum;
        Console.WriteLine($"Guard {sleepiestGuard} slept the most");

    }
    public override void PartTwo()
    {
        throw new NotImplementedException();
    }
    class Guard
    {
        public int ID;
        public int MinutesAsleep;
        public int[] napTimes;
        public Guard(int id)
        {
            ID = id;
        }
    }
    class GuardTime
    {
        public DateTime Time { get; set; }
        public string message;
        public GuardTime(DateTime time, string message)
        {
            Time = time;
            this.message = message;
        }
        public override string ToString() => $"[{Time:yyyy-MM-dd hh:mm}] {message}";
    }
}
