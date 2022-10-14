using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2015;

internal class D17_2015 : Day<int>
{
    public override void PartA()
    {
        Submit(Input.Combinations().Where(x => x.Sum() == 150).Count());
    }
    public override void PartB()
    {
        int len = Input.Combinations().Where(x => x.Sum() == 150).Shortest().Count();
        Submit(Input.Combinations().Where(x => x.Sum() == 150 && x.Count() == len).Count());
    }
}
