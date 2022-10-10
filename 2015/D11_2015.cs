using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC
{
    internal class D11_2015 : Day
    {
        public override void PartA()
        {
            long input = NumberFromPassword(GetInputForDay()[0]);
            while (!Valid(PasswordFromNumber(++input)));
            Submit(PasswordFromNumber(input));
        }
        string PasswordFromNumber(long number)
        {
            string password = "";
            do
            {
                password = (char)((number % 26) + 'a') + password;
                number /= 26;
            } while (password.Length < 8);
            return password;
        }
        long NumberFromPassword(string password)
        {
            long number = 0;
            while (password.Length > 0)
            {
                number *= 26;
                number += password[0] - 'a';
                password = password[1..];
            }
            return number;
        }
        bool Valid(string password)
        {
            if (password.Any(c => "iol".Contains(c))) return false;
            int doublePairs = 0;
            for (int i = 1; i < password.Length; i++)
            {
                if (password[i - 1] == password[i])
                {
                    doublePairs++;
                    i++;
                }
            }
            if (doublePairs < 2) return false;
            bool hasRun = false;
            for (int i = 2; i < password.Length; i++)
            {
                if (password[i - 2] + 1 == password[i - 1] && password[i - 1] + 1 == password[i]) return true;
            }
            return false;
        }
        public override void PartB()
        {
            long input = NumberFromPassword(GetInputForDay()[0]);
            while (!Valid(PasswordFromNumber(++input))) ;
            while (!Valid(PasswordFromNumber(++input))) ;
            Submit(PasswordFromNumber(input));
        }
    }
}
