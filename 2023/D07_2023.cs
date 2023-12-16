using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2023;

internal class D07_2023 : Day
{
    enum CardKinds
    {
        None,
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }
    class Hand : IComparable<Hand>
    {
        public static bool jokerRule = false;
        public readonly string cards;
        public readonly int bid;
        public readonly CardKinds kind;
        public readonly int strength;
        public int rank;
        public int winnings => rank * bid;
        public Hand(string line)
        {
            string[] split = line.Split(' ');
            cards = split[0];
            bid = int.Parse(split[1]);
            if (jokerRule) kind = DetermineKind(cards.Replace("J", null));
            else kind = DetermineKind(cards);
        }
        static CardKinds DetermineKind(string cards)
        {
            int distinct = cards.Distinct().Count();
            int duplicates = cards.Duplicates().Distinct().Count();
            return distinct switch
            {
                5 => CardKinds.HighCard,
                4 => CardKinds.OnePair,
                3 => duplicates > 1 ? CardKinds.TwoPair : CardKinds.ThreeOfAKind,
                2 => duplicates > 1 ? CardKinds.FullHouse : CardKinds.FourOfAKind,
                _ => CardKinds.FiveOfAKind,
            };
        }
        static int CardPower(char c)
        {
            if (c == 'J' && jokerRule) return 1;
            return c switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => 11,
                'T' => 10,
                _ => c - '0'
            };
        }
        public int CompareTo(Hand? other)
        {
            if (other is null) return 1;
            if (other.kind != kind) return kind - other.kind;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != other.cards[i]) return CardPower(cards[i]) - CardPower(other.cards[i]);
            }
            return 0;
        }
    }
    public override void PartA()
    {
        Submit(Input.Select(x => new Hand(x)).Order().WithIndex().Sum(x => (x.index+1)*x.item.bid));
    }
    public override void PartB()
    {
        Hand.jokerRule = true; 
        Submit(Input.Select(x => new Hand(x)).Order().WithIndex().Sum(x => (x.index + 1) * x.item.bid));
    }
}
