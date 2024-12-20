using AOC.Items.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D08_2022 : Day<int>
    {
        public override void PartOne()
        {
            Grid<int> forest = new(Input2D, -1);
            Submit(forest.Count(t => VisibleFromEdge(forest, t)));
        }
        bool VisibleFromEdge(Grid<int> forest, KeyValuePair<Point2D, int> point)
        {
            foreach (Direction dir in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                Point2D evaluating = point.Key.NextIn(dir);
                while (forest[evaluating] < point.Value)
                {
                    evaluating = evaluating.NextIn(dir);
                    if (forest[evaluating] < 0) return true;
                }
            }
            return false;
        }
        public override void PartTwo()
        {
            Grid<int> forest = new(Input2D, -1);
            Submit(forest.Max(t => ScenicScore(forest, t)));
        }

        private int ScenicScore(Grid<int> forest, KeyValuePair<Point2D, int> point)
        {
            int score = 1;
            int rowScore = 0;
            foreach (Direction dir in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                Point2D evaluating = point.Key.NextIn(dir);
                int visibleTrees = 0;
                while (forest[evaluating] >= 0)
                {
                    visibleTrees++;
                    if (forest[evaluating] >= point.Value) break;
                    evaluating = evaluating.NextIn(dir);
                }
                score *= visibleTrees;
            }
            return score;
        }
    }
}
