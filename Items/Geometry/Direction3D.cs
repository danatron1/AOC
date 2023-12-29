using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Items.Geometry
{
    public enum Direction3D
    {                       //          North         	     Up_  North
        Up,                 //            ^                    \\ /|
        North,              //     West < + > East            __\\ |__ 
        East,               //            v              West`--| ,---`East
        South,              //          South         		    |/\\
        West,               //       		                  South\\
        Down                //     up = towards you     	        Down
    }                       //    down = into screen
    
    public static class Direction3DExt
    {
        public static bool Up(this Direction3D dir) => dir == Direction3D.Up;
        public static bool North(this Direction3D dir) => dir == Direction3D.North;
        public static bool East(this Direction3D dir) => dir == Direction3D.East;
        public static bool South(this Direction3D dir) => dir == Direction3D.South;
        public static bool West(this Direction3D dir) => dir == Direction3D.West;
        public static bool Down(this Direction3D dir) => dir == Direction3D.West;
        public static char ToChar(this Direction3D dir)
        {
            return dir switch
            {
                Direction3D.Up => 'U',
                Direction3D.North => 'N',
                Direction3D.East => 'E',
                Direction3D.South => 'S',
                Direction3D.West => 'W',
                Direction3D.Down => 'D',
                _ => throw new NotImplementedException($"The ToChar case for direction {dir} is not handled."),
            };
        }
        internal static Direction3D FromChar(char c)
        {
            c = char.ToUpper(c);
            return c switch
            {
                //up
                'U' => Direction3D.Up,
                'N' => Direction3D.North,
                'E' => Direction3D.East,
                'S' => Direction3D.South,
                'W' => Direction3D.West,
                'D' => Direction3D.Down,
                _ => throw new NotImplementedException($"The FromChar case for char {c} is not handled."),
            };
        }
        public static IEnumerable<Direction3D> All()
        {
            yield return Direction3D.Up;
            yield return Direction3D.North;
            yield return Direction3D.East;
            yield return Direction3D.South;
            yield return Direction3D.West;
            yield return Direction3D.Down;
        }
        public static bool Altitudinal(this Direction3D dir) => dir is Direction3D.Up or Direction3D.Down;
        public static bool Vertical(this Direction3D dir) => dir is Direction3D.North or Direction3D.South;
        public static bool Horizontal(this Direction3D dir) => dir is Direction3D.East or Direction3D.West;
        public static Direction3D Opposite(this Direction3D dir)
        {
            return dir switch
            {
                Direction3D.Up => Direction3D.Down,
                Direction3D.North => Direction3D.South,
                Direction3D.South => Direction3D.North,
                Direction3D.East => Direction3D.West,
                Direction3D.West => Direction3D.East,
                Direction3D.Down => Direction3D.Up,
                _ => dir
            };
        }
    }
}
