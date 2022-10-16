using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Y2015;

internal class D21_2015 : Day
{
    List<ShopItem> shop = new();
    List<ShopItem> weapons = new();
    List<ShopItem> armour = new();
    List<ShopItem> rings = new();
    void LoadShop()
    {
        shop = new();
        string[] selling = ExtraInput;
        ItemType currentType = 0;
        for (int i = 0; i < selling.Length; i++)
        {
            string[] parts = selling[i].Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (parts.Length == 0) continue;
            if (Enum.TryParse(parts[0].Trim(':', 's'), out ItemType type)) currentType = type;
            else
            {
                shop.Add(new ShopItem(parts[0], int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]), currentType));
            }
        }
        weapons = shop.Where(t => t.Type == ItemType.Weapon).ToList();
        armour = shop.Where(t => t.Type == ItemType.Armor).ToList();
        rings = shop.Where(t => t.Type == ItemType.Ring).ToList();
        //rings.Add(ShopItem.Nothing); //allows you to buy only 1 ring with .Pairs()
        //armour.Add(ShopItem.Nothing); //allows you to go into battle naked
    }
    IEnumerable<IEnumerable<ShopItem>> LegalLoadouts()
    {
        foreach (IEnumerable<ShopItem> item in shop.Combinations(1..4))
        {
            if (item.Where(x => x.Type == ItemType.Weapon).Count() != 1) continue;
            if (item.Where(x => x.Type == ItemType.Armor).Count() > 1) continue;
            if (item.Where(x => x.Type == ItemType.Ring).Count() > 2) continue;
            yield return item;
        }
    }
    Character you, boss;
    public override void Setup()
    {
        LoadShop();
        boss = new(Input.Select(s => int.Parse(s.Split(' ')[^1])).ToArray());
        you = new Character(100, 0, 0);
    }
    public override void PartA()
    {
        int lowest = int.MaxValue;
        #region Original solution - faster but less neat
        /*
        foreach (var wep in weapons)
        {
            you.Weapon = wep;
            if (you.EquipmentCost >= lowest) continue;
            if (you.CanBeat(boss)) lowest = you.EquipmentCost;
            foreach (var arm in armour)
            {
                you.Armour = arm;
                if (you.EquipmentCost >= lowest) continue;
                if (you.CanBeat(boss)) lowest = you.EquipmentCost;
                foreach (ShopItem[] r in rings.Pairs())
                {
                    you.Rings = r;
                    if (you.EquipmentCost >= lowest) continue;
                    if (you.CanBeat(boss)) lowest = you.EquipmentCost;
                }
                you.Rings = new ShopItem[] { ShopItem.Nothing, ShopItem.Nothing };
            }
            you.Armour = ShopItem.Nothing;
        }
        /**/
        #endregion
        foreach (IEnumerable<ShopItem> loadout in LegalLoadouts())
        {
            if (loadout.Sum(x => x.Cost) >= lowest) continue;
            if (you.CanBeat(boss, loadout)) lowest = you.EquipmentCost;
        }
        Submit(lowest);
        //also one line solution because why not;
        //Submit(LegalLoadouts().Where(l => you.CanBeat(boss, l)).MinBy(c => c.Sum(d => d.Cost)).Sum(d => d.Cost));
    }
    public override void PartB()
    {
        int highest = 0;
        foreach (IEnumerable<ShopItem> loadout in LegalLoadouts())
        {
            if (loadout.Sum(x => x.Cost) <= highest) continue;
            if (!you.CanBeat(boss, loadout)) highest = you.EquipmentCost;
        }
        Submit(highest);
    }
    internal class Character
    {
        public int HP;
        private int _baseHP;
        private int _baseDamage;
        private int _baseDefense;
        public ShopItem[] Loadout = new ShopItem[4];
        public bool Dead => HP <= 0;
        public int Damage => _baseDamage + Loadout.Sum(a => a.Damage);
        public int Defense => _baseDefense + Loadout.Sum(a => a.Defense);
        public int EquipmentCost => Loadout.Sum(a => a.Cost);
        public int DamageTo(Character target) => Math.Max(1, Damage - target.Defense);
        public int HitsToKill(Character target) => (int)Math.Ceiling((double)target.HP / DamageTo(target));
        public void Attack(Character target) => target.HP -= DamageTo(target);
        public bool CanBeat(Character target)
        {
            Heal();
            target.Heal();
            return HitsToKill(target) <= target.HitsToKill(this);
        }
        public bool CanBeat(Character target, IEnumerable<ShopItem> withLoadout)
        {
            UnequipAll();
            Equip(withLoadout.ToArray());
            return CanBeat(target);
        }
        public void SimulateBattle(Character target)
        {
            for (int i = 1; i < 100000; i++)
            {
                Attack(target);
                Console.WriteLine($"{i} => You attack the target for {DamageTo(target)}, putting their health at {target.HP}/{target._baseHP}");
                if (target.Dead)
                {
                    Console.WriteLine("You win");
                    break;
                }
                target.Attack(this);
                Console.WriteLine($"{i} <= The target attakcs you for {target.DamageTo(this)}, putting your health at {HP}/{_baseHP}");
                if (Dead)
                {
                    Console.WriteLine("You lost!");
                    break;
                }
            }
        }
        internal void Heal() => HP = _baseHP;
        public void Unequip(params int[] ids)
        {
            foreach (int i in ids)
            {
                Loadout[i] = ShopItem.Nothing;
            }
        }
        public void UnequipAll() => Unequip(0, 1, 2, 3);
        public void Equip(params ShopItem[] items)
        {
            foreach (ShopItem item in items)
            {
                if (item.Type == ItemType.Weapon) Loadout[0] = item;
                if (item.Type == ItemType.Armor) Loadout[1] = item;
                if (item.Type == ItemType.Ring)
                {
                    if (Loadout[2] == ShopItem.Nothing) Loadout[2] = item;
                    else Loadout[3] = item;
                }
            }
        }
        public Character(params int[] input) : this(input[0], input[1], input[2]) { }
        public Character(int hp, int damage, int armour)
        {
            _baseHP = hp;
            _baseDamage = damage;
            _baseDefense = armour;
            Heal();
            UnequipAll();
        }
    }
    internal class ShopItem
    {
        public static ShopItem Nothing = new ShopItem("Nothing", 0, 0, 0, ItemType.Ring);
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Damage { get; set; }
        public int Defense { get; set; }
        public ItemType Type { get; set; }
        public ShopItem(string name, int cost, int damage, int armour, ItemType type)
        {
            Name = name;
            Cost = cost;
            Damage = damage;
            Defense = armour;
            Type = type;
        }
        public override string ToString() => Name;
    }
    internal enum ItemType
    {
        Weapon,
        Armor,
        Ring
    }
}
