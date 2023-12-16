using AOC.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2015;

internal class D18_2015 : Day<char>
{
    public override void PartA()
    {
        GOL gol = new();
        gol.grid.SetLimits(InputRaw.Length, InputRaw.Length);
        gol.grid.SetPoints(Input2D.Select2D(c => c == '#'));
        gol.Iterate(100);
        Submit(gol.grid.GetPoints().Count());
    }
    public override void PartB()
    {
        GOL gol = new();
        gol.grid.SetLimits(InputRaw.Length, InputRaw.Length);
        gol.grid.SetPoints(Input2D.Select2D(c => c == '#'));
        for (int i = 0; i < 100; i++)
        {
            TurnOnCorners();
            gol.Iterate(1);
        }
        TurnOnCorners();
        Submit(gol.grid.GetPoints().Count());
        void TurnOnCorners()
        {
            var corners = gol.grid.GetCorners();
            gol.grid.SetPoints(true, corners.TopLeft, corners.TopRight, corners.BottomLeft, corners.BottomRight);
        }
    }
}
