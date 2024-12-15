using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static AOC.Y2015.D22_2015;

namespace AOC.Y2015;

internal class D22_2015 : Day
{
    Spell[] spells =
    {
        new("Magic Missile", 53, 0, 4),
        new("Drain", 73, 2, 2),
        new("Shield", 113, 0, 0, new("Shield", 6, 0, 7, 0)),
        new("Poison", 173, 0, 0, new("Poison", 6, 0, 0, 3)),
        new("Recharge", 229, 0, 0, new("Recharge", 5, 101, 0, 0))
    };
    void PlayGame()
    {
        GameState game = new(int.Parse(Input[1].Split(' ')[^1]))
        {
            bossHP = int.Parse(Input[0].Split(' ')[^1]),
            yourHP = 50,
            yourMana = 500,
            effectsActive = new(),
            nextSpell = Spell.NoSpell
        };
        while (true)
        {
            TickEffects();
            if (GameEnd(true)) break;
            Console.WriteLine("\n-- Player turn --");
            PrintHealths();
            int toCastID = -1;
            do
            {
                Console.WriteLine("What spell do you want to cast? (input a number)");
                for (int i = 0; i < spells.Length; i++)
                {
                    Console.ForegroundColor = spells[i].Cost <= game.yourMana ? ConsoleColor.White : ConsoleColor.DarkGray;
                    Console.WriteLine($"{i}: {spells[i].Name} ({spells[i].Cost} mana) {(spells[i].LingeringEffect == null ? "" : "- Has lingering effect")}");
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    if (result < 0 || result >= spells.Length) continue;
                    if (spells[result].Cost > game.yourMana) continue;
                    if (spells[result].LingeringEffect != null
                        && game.effectsActive
                            .Where(i => i.turnsRemaining > 1)
                            .Select(x => x.effect)
                            .Contains(spells[result].LingeringEffect)) continue;
                    toCastID = result;
                }
            } while (toCastID < 0);
            game.nextSpell = spells[toCastID];
            Console.WriteLine($"\nYou cast {game.nextSpell}!");
            game.ResolveNewSpell();
            if (GameEnd()) break;
            TickEffects();
            Console.WriteLine("\n-- Boss turn --");
            if (GameEnd(true)) break;
            PrintHealths();
            game.DoBossAttack();
            Console.WriteLine($"The boss attacks you, dealing {GameState.BossDamage - game.yourArmour} damage!\n");
            if (GameEnd()) break;
        }
        bool GameEnd(bool fromTickEffects = false)
        {
            if (!fromTickEffects || game.effectsActive.Count > 1) PrintHealths();
            if (game.CheckIfOver())
            {
                if (game.PlayerWon) Console.WriteLine("You won! The boss was defeated.");
                else Console.WriteLine("Game over! You were killed :(");
                Console.WriteLine("Would you like to play again? (yes/no)");
                if (Console.ReadLine()?.ToLower()[0] == 'y') PlayGame();
                return true;
            }
            return false;
        }
        void PrintHealths()
        {
            Console.WriteLine($"- You have {game.yourHP} HP, {game.yourArmour} defense, and {game.yourMana} mana");
            Console.WriteLine($"- The boss has {game.bossHP} HP");
        }
        void TickEffects()
        {
            game.ActivateEffects();
            foreach (ActiveEffect e in game.effectsActive)
            {
                Console.WriteLine($"EFFECT: {e.effect.name} activates - {e.effect.name}'s timer is now at {--e.turnsRemaining}.");
                if (e.turnsRemaining == 0) Console.WriteLine($"{e.effect.name} wore off!");
            }
            game.RemoveExpiredEffects();
        }
    }
    public override void PartOne()
    {
        Submit(FindLowestSuccessfulMana());
    }
    public override void PartTwo()
    {
        Submit(FindLowestSuccessfulMana(true));
    }
    int? FindLowestSuccessfulMana(bool hardmode = false)
    {
        PriorityQueue<GameState, int> nextMoves = new();
        GameState starting = new(int.Parse(Input[1].Split(' ')[^1]))
        {
            bossHP = int.Parse(Input[0].Split(' ')[^1]),
            yourHP = 50,
            yourMana = 500,
            effectsActive = new(),
            nextSpell = Spell.NoSpell
        };
        nextMoves.Enqueue(starting, 0);
        int? lowestManaSuccess = null;
        int searched = 0;
        while (nextMoves.Count > 0)
        {
            if (lowestManaSuccess.HasValue) break;
            GameState state = nextMoves.Dequeue();
            foreach (Spell spell in LegalSpells(state))
            {
                if (lowestManaSuccess.HasValue) break;
                GameState next = new(state, spell);
                //Console.WriteLine($"{nextMoves.Count}/{searched}: {next}");
                next.Advance(hardmode);
                searched++;
                if (next.FightOver)
                {
                    if (next.PlayerWon) lowestManaSuccess = next.manaUsedSoFar;
                    continue;
                }
                nextMoves.Enqueue(next, next.manaUsedSoFar);
            }
        }
        return lowestManaSuccess;
    }
    IEnumerable<Spell> LegalSpells(GameState state)
    {
        foreach (Spell spell in spells)
        {
            if (spell.LingeringEffect != null 
                && state.effectsActive
                    .Where(s => s.turnsRemaining > 1)
                    .Select(s => s.effect)
                    .Contains(spell.LingeringEffect))
            {
                continue;
            }
            if (state.yourMana >= spell.Cost) yield return spell;
        }
    }
    internal class GameState
    {
        public static int BossDamage;

        public int bossHP;
        public int yourHP;
        public int yourMana;
        public int yourArmour;
        public int manaUsedSoFar;
        public List<ActiveEffect> effectsActive = new();
        public Spell nextSpell;
        public string previousSpell;
        public int round;

        internal bool FightOver = false;
        internal bool PlayerWon = false;

        public GameState(int bossDamage) 
        {
            BossDamage = bossDamage;
        }
        public GameState(GameState state, Spell spell)
        {
            round = state.round;
            previousSpell = state.nextSpell.Name;
            bossHP = state.bossHP;
            yourHP = state.yourHP;
            yourMana = state.yourMana;
            manaUsedSoFar = state.manaUsedSoFar;
            foreach (ActiveEffect e in state.effectsActive)
            {
                effectsActive.Add(new ActiveEffect(e));
            }
            nextSpell = spell;
        }

        internal void Advance(bool hardmode = false)
        {
            if (hardmode) yourHP--;
            if (CheckIfOver()) return;
            round++;
            //player turn
            ResolveEffects();
            if (CheckIfOver()) return;
            ResolveNewSpell();
            if (CheckIfOver()) return;
            //boss turn
            ResolveEffects();
            if (CheckIfOver()) return;
            DoBossAttack();
            if (CheckIfOver()) return;
        }
        internal void DoBossAttack()
        {
            yourHP -= BossDamage - yourArmour;
        }
        internal bool CheckIfOver()
        {
            if (bossHP <= 0)
            {
                FightOver = true;
                PlayerWon = true;
                return true;
            }
            if (yourHP <= 0)
            {
                FightOver = true;
                PlayerWon = false;
                return true;
            }
            return false;
        }
        internal void ResolveNewSpell()
        {
            bossHP -= nextSpell.immediateDamage;
            yourHP += nextSpell.immediateHealing;
            manaUsedSoFar += nextSpell.Cost;
            yourMana -= nextSpell.Cost;
            if (nextSpell.LingeringEffect != null) effectsActive.Add(new(nextSpell.LingeringEffect));
        }
        public void ActivateEffects()
        {
            bossHP -= effectsActive.Sum(x => x.effect.damageDealt);
            yourArmour = effectsActive.Sum(x => x.effect.armourGain);
            yourMana += effectsActive.Sum(x => x.effect.manaGain);
        }
        public void ResolveEffects()
        {
            ActivateEffects();
            //tick down effect timer
            foreach (ActiveEffect effect in effectsActive) effect.turnsRemaining--;
            RemoveExpiredEffects();
        }
        public override string ToString()
        {
            return $"Round {round}: You ({yourHP}-{yourMana}) vs Boss ({bossHP}) - upcoming spell ({nextSpell}), last {previousSpell}";
        }

        public void RemoveExpiredEffects()
        {
            effectsActive = effectsActive.Where(e => e.turnsRemaining > 0).ToList();
        }
    }
    internal class ActiveEffect
    {
        public Effect effect;
        public int turnsRemaining;
        public ActiveEffect(ActiveEffect ae) 
        {
            effect = ae.effect;
            turnsRemaining = ae.turnsRemaining;
        }
        public ActiveEffect(Effect e)
        {
            effect = e;
            turnsRemaining = e.baseTurns;
        }
    }
    internal class Effect
    {
        public string name;
        public int baseTurns;
        public int manaGain;
        public int armourGain;
        public int damageDealt;
        public Effect(string name, int baseTurns, int manaGain, int armourGain, int damageDealt)
        {
            this.baseTurns = baseTurns;
            this.manaGain = manaGain;
            this.armourGain = armourGain;
            this.damageDealt = damageDealt;
            this.name = name;
        }
    }
    internal class Spell
    {
        public static Spell NoSpell = new("None", 0, 0, 0);
        public string Name;
        public int Cost;
        public int immediateDamage;
        public int immediateHealing;
        public Effect? LingeringEffect;
        public Spell(string name, int cost, int healing, int damage, Effect? lingering = null)
        {
            Name = name;
            Cost = cost;
            immediateDamage = damage;
            immediateHealing = healing;
            LingeringEffect = lingering;
        }
        public override string ToString() => Name;
    }
    internal class Character
    {
        int _baseHP;
        public int HP;
        public int Damage;
        public int Defense;
        public bool Dead => HP <= 0;
        public void Heal() => HP = _baseHP;
        public int DamageTo(Character target) => Damage - target.Defense;
        public void Attack(Character target) => target.HP -= DamageTo(target);
        public bool CanBeat(Character target)
        {
            Heal();
            target.Heal();
            bool targetsTurn = false;
            while (true)
            {
                ApplyEffects(target);
                if (target.Dead) return true;
                if (targetsTurn)
                {
                    target.Attack(this);
                    if (Dead) return false;
                }
                else
                {

                }

            }
        }
        private void ApplyEffects(Character target)
        {
            throw new NotImplementedException();
        }
    }
}
