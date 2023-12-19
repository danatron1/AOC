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
        public static bool North(this Direction dir) => dir == Direction.North;
        public static bool East(this Direction dir) => dir == Direction.East;
        public static bool South(this Direction dir) => dir == Direction.South;
        public static bool West(this Direction dir) => dir == Direction.West;
        public static char ToChar(this Direction dir)
        {
            return dir switch
            {
                Direction.North => 'N',
                Direction.East => 'E',
                Direction.South => 'S',
                Direction.West => 'W',
                _ => throw new NotImplementedException($"The ToChar case for direction {dir} is not handled."),
            };
        }
        internal static Direction FromChar(char c)
        {
            c = char.ToUpper(c);
            return c switch
            {
                //up
                'U' or 'N' => Direction.North,
                //right
                'R' or 'E' => Direction.East,
                //down
                'D' or 'S' => Direction.South,
                //left
                'L' or 'W' => Direction.West,
                _ => throw new NotImplementedException($"The FromChar case for char {c} is not handled."),
            };
        }
        public static IEnumerable<Direction> All()
        {
            yield return Direction.North;
            yield return Direction.East;
            yield return Direction.South;
            yield return Direction.West;
        }
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
