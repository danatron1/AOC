using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items
{ //formerly contained in Point2D
    public enum Direction 
    {                       //          North
        North,              //            ^
        East,               //     West < + > East
        South,              //            v
        West                //          South
    }
    public static class DirectionExt
    {
        public static bool Vertical(this Direction dir) => dir is Direction.North or Direction.South;
        public static bool Horizontal(this Direction dir) => dir is Direction.East or Direction.West;
        public static Direction Opposite(this Direction dir)
        {
            return dir switch
            {
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.East => Direction.West,
                Direction.West => Direction.East,
                _ => dir
            };
        }
        public static Direction Left(this Direction dir)
        {
            return dir switch
            {
                Direction.North => Direction.West,
                Direction.South => Direction.East,
                Direction.East => Direction.North,
                Direction.West => Direction.South,
                _ => dir
            };
        }
        public static Direction Right(this Direction dir)
        {
            return dir switch
            {
                Direction.North => Direction.East,
                Direction.South => Direction.West,
                Direction.East => Direction.South,
                Direction.West => Direction.North,
                _ => dir
            };
        }
    }
}
