using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2022
{
    internal class D02_2022 : Day
    {
        public override void PartA()
        {
            //Explanation:
            //First I calculate the result. The result is either 0, 3, or 6 for a loss, draw, or win respectively.
            //So I calculate 0, 1, or 2 for lose/draw/win, then multiply by 3. 
            //
            //I calculate this by subtracting the first character (A/B/C) from the last (X/Y/Z),
            //this gives a range of integer values, but what matters is their relative position;
            //Y beats A, Z beats B, and X beats C
            //all those values give the same result once modulo'd, since they're all one off from eachother. X wraps around to be Z+1
            //the +2 I throw in is just to fudge the numbers so that I get what I want, for example, X-A = draw, so 2+X-A % 3 = 1
            //
            //I then add on my score; 1 for rock, 2 for paper, 3 for scissors. This is just the last character minus W, 
            //which effectively counts the number of characters after W that that letter is, which gives me the score.
            Submit(Input.Select(x => (2 + x[2] - x[0]) % 3 * 3 + x[2] - 'W').Sum());
        }
        public override void PartB()
        {
            //Explanation:
            //Similar strategy to above. I already know the outcome (x[2]) so I just convert that into 0, 1, or 2 by subtracting X
            //Like above, this gives me the number of characters after X. 0 for loss, 1 for draw, 2 for win. I multiply that by 3.
            //
            //I then add on my score. This is once again the offset from my play to the opponents, so I use the same modulo 3 trick.
            //This time, A Y, B X, and C Z all mean I choose rock, so get a score of 1.
            //One counts up (ABC) while the other counts down (ZYX) add them to cancel that out, add 2 for the exact same reason as above,
            //Then finally modulo 3 to get 0, 1, or 2 (corresponding to whether I picked rock, paper, or scissors).
            //The scores are actually 1, 2, or 3, so the final step is to just add 1 to that.
            Submit(Input.Select(x => (x[2]-'X')*3 + (x[2] + x[0] + 2) % 3 + 1).Sum());
        }
    }
}
