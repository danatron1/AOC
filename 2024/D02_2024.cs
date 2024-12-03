using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2024;

internal class D02_2024 : Day
{
    public override void PartA()
    {
        var reports = Input.Select(x => x.ExtractNumbers<int>());
        Submit(reports.Select(Differences).Where(SafeReport).Count());
    }
    public override void PartB()
    {
        var reports = Input.Select(x => x.ExtractNumbers<int>());
        Submit(reports.Where(SafeWithProblemDampener).Count());
    }

    private bool SafeWithProblemDampener(IEnumerable<int> report)
    {
        return report.Pairs(report.Count() - 1).Select(Differences).Any(SafeReport);
    }
    bool SafeReport(IEnumerable<int> report) => SameDirection(report) && SmallChange(report);
    IEnumerable<int> Differences(IEnumerable<int> report)
    {
        return report.SelectWithPrevious((x, y) => x - y);
    }
    bool SameDirection(IEnumerable<int> report)
    {
        return report.All(x => x > 0) || report.All(x => x < 0);
    }
    bool SmallChange(IEnumerable<int> report)
    {
        return report.Select(Math.Abs).All(x => x >= 1 && x <= 3);
    }
}
