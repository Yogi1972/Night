using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// PvP Arena system for player vs player combat
    /// </summary>
    internal static class PvPArena
    {
        private static readonly Random _rng = new Random();

        #region Arena Rankings

        private static readonly Dictionary<Character, ArenaStats> _arenaStats = new Dictionary<Character, ArenaStats>();

        private class ArenaStats
        {
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int TotalDamageDealt { get; set; }
            public string Rank => CalculateRank();

            private string CalculateRank()
            {
                int score = (Wins * 3) - Losses;
                return score switch
                {
                    >= 50 => "Legendary Champion",
                    >= 30 => "Grand Champion",
                    >= 20 => "Champion",
                    >= 10 => "Veteran",
                    >= 5 => "Fighter",
                    >= 0 => "Novice",
                    _ => "Rookie"
                };
            }

            public double WinRate => (Wins + Losses) > 0 ? (double)Wins / (Wins + Losses) * 100.0 : 0.0;
        }

        #endregion

        #region Arena Menu

        public static void OpenArena(List<Character> party)
        {
            if (party.Count < 2)
            {
                VisualEffects.WriteInfo("Need at least 2 characters for arena battles!\n");
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
                VisualEffects.WriteLineColored("║                   ⚔️  PVP ARENA  ⚔️                       ║", ConsoleColor.Red);
                Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

                Console.WriteLine("Welcome to the Arena! Test your skills against your allies!");
                Console.WriteLine();
                Console.WriteLine("1) ⚔️  Quick Duel (1v1)");
                Console.WriteLine("2) 🏆 Tournament Mode (Bracket-style)");
                Console.WriteLine("3) 📊 View Arena Rankings");
                Console.WriteLine("4) 💰 Wagered Match (Bet gold)");
                Console.WriteLine("0) 🚪 Leave Arena");
                Console.WriteLine();
                Console.Write("Choice: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        QuickDuel(party);
                        break;
                    case "2":
                        TournamentMode(party);
                        break;
                    case "3":
                        ViewRankings(party);
                        break;
                    case "4":
                        WageredMatch(party);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        #endregion

        #region Duel System

        private static void QuickDuel(List<Character> party)
        {
            Console.WriteLine("\n=== SELECT FIGHTERS ===\n");

            Console.WriteLine("Select Fighter 1:");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} (Lv {party[i].Level} {party[i].GetType().Name})");
            }
            Console.Write("Choice: ");

            if (!int.TryParse(Console.ReadLine(), out int f1Idx) || f1Idx < 1 || f1Idx > party.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Console.WriteLine("\nSelect Fighter 2:");
            for (int i = 0; i < party.Count; i++)
            {
                if (i != f1Idx - 1)
                    Console.WriteLine($"{i + 1}) {party[i].Name} (Lv {party[i].Level} {party[i].GetType().Name})");
            }
            Console.Write("Choice: ");

            if (!int.TryParse(Console.ReadLine(), out int f2Idx) || f2Idx < 1 || f2Idx > party.Count || f2Idx == f1Idx)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var fighter1 = party[f1Idx - 1];
            var fighter2 = party[f2Idx - 1];

            ExecuteDuel(fighter1, fighter2);
        }

        private static void ExecuteDuel(Character fighter1, Character fighter2)
        {
            Console.Clear();

            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            VisualEffects.WriteLineColored("║                    ARENA DUEL                             ║", ConsoleColor.Yellow);
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            VisualEffects.WriteLineColored($"⚔️  {fighter1.Name} vs {fighter2.Name}!", ConsoleColor.Red);
            Console.WriteLine();

            // Save original HP
            int f1OriginalHp = fighter1.Health;
            int f2OriginalHp = fighter2.Health;

            // Set both to full HP for fair fight
            fighter1.Heal(fighter1.MaxHealth);
            fighter2.Heal(fighter2.MaxHealth);

            Console.WriteLine($"{fighter1.Name}: {fighter1.Health} HP");
            Console.WriteLine($"{fighter2.Name}: {fighter2.Health} HP");
            Console.WriteLine();
            Console.WriteLine("Press any key to begin!");
            Console.ReadKey(true);

            int round = 1;
            while (fighter1.Health > 0 && fighter2.Health > 0)
            {
                Console.WriteLine();
                VisualEffects.WriteLineColored($"=== ROUND {round} ===", ConsoleColor.Yellow);
                Console.WriteLine();

                // Fighter 1 attacks
                int roll1 = CombatSystem.RollD20();
                int damage1 = CalculateDuelDamage(fighter1, roll1);

                if (roll1 == 20)
                {
                    VisualEffects.ShowCriticalHitEffect();
                    Console.WriteLine();
                }

                Console.Write($"{fighter1.Name} rolls {roll1} - ");
                if (roll1 > 1 && damage1 > 0)
                {
                    fighter2.ReceiveDamage(damage1);
                    VisualEffects.WriteDamage($"Deals {damage1} damage!\n");

                    // Show health bars
                    Console.Write("  ");
                    VisualEffects.DrawProgressBarLine(fighter2.Health, fighter2.MaxHealth, 20, $"{fighter2.Name}");
                }
                else
                {
                    VisualEffects.WriteInfo($"{VisualEffects.GetRandomMissMessage()}\n");
                }

                if (fighter2.Health <= 0) break;

                // Fighter 2 attacks
                int roll2 = CombatSystem.RollD20();
                int damage2 = CalculateDuelDamage(fighter2, roll2);

                if (roll2 == 20)
                {
                    VisualEffects.ShowCriticalHitEffect();
                    Console.WriteLine();
                }

                Console.Write($"{fighter2.Name} rolls {roll2} - ");
                if (roll2 > 1 && damage2 > 0)
                {
                    fighter1.ReceiveDamage(damage2);
                    VisualEffects.WriteDamage($"Deals {damage2} damage!\n");

                    // Show health bars
                    Console.Write("  ");
                    VisualEffects.DrawProgressBarLine(fighter1.Health, fighter1.MaxHealth, 20, $"{fighter1.Name}");
                }
                else
                {
                    VisualEffects.WriteInfo($"{VisualEffects.GetRandomMissMessage()}\n");
                }

                round++;
                System.Threading.Thread.Sleep(1000);
            }

            // Determine winner
            Character winner = fighter1.Health > 0 ? fighter1 : fighter2;
            Character loser = winner == fighter1 ? fighter2 : fighter1;

            Console.WriteLine();
            VisualEffects.ShowVictoryBanner();
            VisualEffects.WriteLineColored($"🏆 {winner.Name} wins the duel!", ConsoleColor.Green);
            Console.WriteLine();

            // Update stats
            EnsureArenaStats(winner);
            EnsureArenaStats(loser);
            _arenaStats[winner].Wins++;
            _arenaStats[loser].Losses++;

            // Award XP
            int xpReward = 150;
            winner.GainExperience(xpReward);
            loser.GainExperience(xpReward / 2);

            Console.WriteLine($"{winner.Name} earned {xpReward} XP!");
            Console.WriteLine($"{loser.Name} earned {xpReward / 2} XP for participating!");

            // Restore original HP
            fighter1.ReceiveDamage(fighter1.Health - Math.Min(f1OriginalHp, fighter1.MaxHealth));
            fighter2.ReceiveDamage(fighter2.Health - Math.Min(f2OriginalHp, fighter2.MaxHealth));

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private static int CalculateDuelDamage(Character attacker, int roll)
        {
            if (roll == 1) return 0;

            int baseDamage = attacker.GetTotalStrength();
            if (attacker is Mage) baseDamage = attacker.GetTotalIntelligence();
            else if (attacker is Rogue) baseDamage = attacker.GetTotalAgility();

            return roll == 20 ? baseDamage * 2 : baseDamage;
        }

        #endregion

        #region Tournament Mode

        private static void TournamentMode(List<Character> party)
        {
            if (party.Count < 2)
            {
                Console.WriteLine("Need at least 2 characters for a tournament!");
                return;
            }

            Console.WriteLine("\n🏆 TOURNAMENT MODE 🏆");
            Console.WriteLine("All characters will compete in elimination brackets!");
            Console.WriteLine("Press any key to begin...");
            Console.ReadKey(true);

            var competitors = new List<Character>(party);
            int round = 1;

            while (competitors.Count > 1)
            {
                Console.WriteLine();
                VisualEffects.WriteLineColored($"=== TOURNAMENT ROUND {round} ===", ConsoleColor.Yellow);
                Console.WriteLine();

                var nextRound = new List<Character>();

                for (int i = 0; i < competitors.Count - 1; i += 2)
                {
                    ExecuteDuel(competitors[i], competitors[i + 1]);
                    var winner = competitors[i].Health > competitors[i + 1].Health ? competitors[i] : competitors[i + 1];
                    nextRound.Add(winner);
                }

                // If odd number, last one gets a bye
                if (competitors.Count % 2 == 1)
                {
                    var byeChar = competitors[competitors.Count - 1];
                    Console.WriteLine($"\n{byeChar.Name} receives a bye to the next round!");
                    nextRound.Add(byeChar);
                }

                competitors = nextRound;
                round++;
            }

            // Tournament winner
            var champion = competitors[0];
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            VisualEffects.WriteLineColored("║                                                           ║", ConsoleColor.Yellow);
            VisualEffects.WriteLineColored($"║          🏆 TOURNAMENT CHAMPION: {champion.Name.PadRight(20)} 🏆       ║", ConsoleColor.Yellow);
            VisualEffects.WriteLineColored("║                                                           ║", ConsoleColor.Yellow);
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");

            // Massive reward
            champion.Inventory.AddGold(1000);
            champion.GainExperience(1000);

            VisualEffects.WriteSuccess("\n💰 Champion's Reward: 1000 gold and 1000 XP!\n");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Wagered Match

        private static void WageredMatch(List<Character> party)
        {
            Console.WriteLine("\n💰 WAGERED MATCH 💰");
            Console.WriteLine("Both fighters bet gold - winner takes all!");
            Console.WriteLine();

            Console.Write("Bet amount (gold): ");
            if (!int.TryParse(Console.ReadLine(), out int bet) || bet <= 0)
            {
                Console.WriteLine("Invalid bet amount.");
                return;
            }

            Console.WriteLine("\nSelect Fighter 1:");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} (Gold: {party[i].Inventory.Gold})");
            }

            if (!int.TryParse(Console.ReadLine(), out int f1Idx) || f1Idx < 1 || f1Idx > party.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Console.WriteLine("\nSelect Fighter 2:");
            for (int i = 0; i < party.Count; i++)
            {
                if (i != f1Idx - 1)
                    Console.WriteLine($"{i + 1}) {party[i].Name} (Gold: {party[i].Inventory.Gold})");
            }

            if (!int.TryParse(Console.ReadLine(), out int f2Idx) || f2Idx < 1 || f2Idx > party.Count || f2Idx == f1Idx)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var fighter1 = party[f1Idx - 1];
            var fighter2 = party[f2Idx - 1];

            // Check if both can afford the bet
            if (fighter1.Inventory.Gold < bet || fighter2.Inventory.Gold < bet)
            {
                VisualEffects.WriteDanger("One or both fighters cannot afford the bet!\n");
                return;
            }

            // Take the bets
            fighter1.Inventory.SpendGold(bet);
            fighter2.Inventory.SpendGold(bet);
            int pot = bet * 2;

            VisualEffects.WriteLineColored($"💰 Total pot: {pot} gold!", ConsoleColor.Yellow);
            Console.WriteLine();

            ExecuteDuel(fighter1, fighter2);

            // Award pot to winner
            var winner = fighter1.Health > fighter2.Health ? fighter1 : fighter2;
            winner.Inventory.AddGold(pot);

            VisualEffects.WriteSuccess($"💰 {winner.Name} wins {pot} gold!\n");
        }

        #endregion

        #region Rankings

        private static void ViewRankings(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            VisualEffects.WriteLineColored("║                 ARENA RANKINGS                            ║", ConsoleColor.Cyan);
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            var ranked = party
                .Select(p => new
                {
                    Character = p,
                    Stats = _arenaStats.ContainsKey(p) ? _arenaStats[p] : new ArenaStats()
                })
                .OrderByDescending(x => x.Stats.Wins)
                .ThenBy(x => x.Stats.Losses)
                .ToList();

            int position = 1;
            foreach (var entry in ranked)
            {
                var stats = entry.Stats;
                var color = position switch
                {
                    1 => ConsoleColor.Yellow,
                    2 => ConsoleColor.Gray,
                    3 => ConsoleColor.DarkYellow,
                    _ => ConsoleColor.White
                };

                var rankIcon = position switch
                {
                    1 => "🥇",
                    2 => "🥈",
                    3 => "🥉",
                    _ => $"{position}."
                };

                Console.ForegroundColor = color;
                Console.WriteLine($"{rankIcon} {entry.Character.Name}");
                Console.ResetColor();
                Console.WriteLine($"   Rank: {stats.Rank}");
                Console.WriteLine($"   Record: {stats.Wins}W - {stats.Losses}L ({stats.WinRate:F1}% win rate)");
                Console.WriteLine($"   Total Damage: {stats.TotalDamageDealt:N0}");
                Console.WriteLine();

                position++;
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static void EnsureArenaStats(Character character)
        {
            if (!_arenaStats.ContainsKey(character))
            {
                _arenaStats[character] = new ArenaStats();
            }
        }

        #endregion
    }
}
