using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Displays a comprehensive achievement summary for the player
    /// </summary>
    internal static class AchievementSummary
    {
        public static void ShowHeroicSummary(List<Character> party)
        {
            Console.Clear();

            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            VisualEffects.WriteLineColored("║              🏆 HEROIC ACHIEVEMENTS 🏆                    ║", ConsoleColor.Yellow);
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            if (party == null || party.Count == 0)
            {
                Console.WriteLine("No heroes to display.");
                return;
            }

            // Overall party stats
            int totalLevels = party.Sum(p => p.Level);
            int totalGold = party.Sum(p => p.Inventory.Gold);
            int highestLevel = party.Max(p => p.Level);
            var strongest = party.OrderByDescending(p => p.GetTotalStrength()).First();
            var smartest = party.OrderByDescending(p => p.GetTotalIntelligence()).First();
            var fastest = party.OrderByDescending(p => p.GetTotalAgility()).First();

            VisualEffects.WriteLineColored("═══ PARTY OVERVIEW ═══", ConsoleColor.Cyan);
            Console.WriteLine($"Party Size: {party.Count}");
            Console.WriteLine($"Combined Levels: {totalLevels}");
            Console.WriteLine($"Highest Level: {highestLevel} ({Playerleveling.GetLevelTitle(highestLevel)})");
            Console.WriteLine($"Total Gold: {totalGold:N0}");
            Console.WriteLine();

            VisualEffects.WriteLineColored("═══ NOTABLE HEROES ═══", ConsoleColor.Cyan);
            VisualEffects.WriteColored($"💪 Strongest: ", ConsoleColor.White);
            VisualEffects.WriteLineColored($"{strongest.Name} ({strongest.GetTotalStrength()} STR)", ConsoleColor.Red);

            VisualEffects.WriteColored($"🧠 Smartest: ", ConsoleColor.White);
            VisualEffects.WriteLineColored($"{smartest.Name} ({smartest.GetTotalIntelligence()} INT)", ConsoleColor.Cyan);

            VisualEffects.WriteColored($"⚡ Fastest: ", ConsoleColor.White);
            VisualEffects.WriteLineColored($"{fastest.Name} ({fastest.GetTotalAgility()} AGI)", ConsoleColor.Yellow);
            Console.WriteLine();

            // Individual hero highlights
            VisualEffects.WriteLineColored("═══ INDIVIDUAL HEROES ═══", ConsoleColor.Cyan);
            foreach (var hero in party)
            {
                Console.WriteLine();
                ShowHeroHighlight(hero);
            }

            // Special achievements
            Console.WriteLine();
            VisualEffects.WriteLineColored("═══ SPECIAL ACHIEVEMENTS ═══", ConsoleColor.Cyan);

            var maxLevelHeroes = party.Where(p => p.Level == 100).ToList();
            if (maxLevelHeroes.Any())
            {
                VisualEffects.WriteSuccess($"⭐ ASCENDED HEROES: {maxLevelHeroes.Count}\n");
                foreach (var hero in maxLevelHeroes)
                {
                    Console.WriteLine($"   • {hero.Name} - The Ascended");
                }
            }

            var champions = party.Where(p => p.HasChampionClass).ToList();
            if (champions.Any())
            {
                VisualEffects.WriteSuccess($"👑 CHAMPION CLASSES: {champions.Count}\n");
                foreach (var champ in champions)
                {
                    Console.WriteLine($"   • {champ.Name} - {champ.ChampionClass}");
                }
            }

            var mythicHeroes = party.Where(p => p.HasMythicTitle).ToList();
            if (mythicHeroes.Any())
            {
                VisualEffects.WriteSuccess($"🌟 MYTHIC TITLES: {mythicHeroes.Count}\n");
                foreach (var hero in mythicHeroes)
                {
                    Console.WriteLine($"   • {hero.Name} — \"{hero.MythicTitle}\"");
                }
            }

            var petOwners = party.Where(p => p.Pet != null).ToList();
            if (petOwners.Any())
            {
                VisualEffects.WriteSuccess($"🐾 PET COMPANIONS: {petOwners.Count}\n");
                foreach (var owner in petOwners)
                {
                    Console.WriteLine($"   • {owner.Pet!.Name} (Lv {owner.Pet.Level}) - {owner.Name}'s companion");
                }
            }

            // Count legendary items
            int legendaryCount = 0;
            foreach (var hero in party)
            {
                var items = hero.Inventory.Slots.Where(i => i != null);
                foreach (var item in items)
                {
                    if (item!.Name.Contains("Dragon") || item.Name.Contains("Phoenix") ||
                        item.Name.Contains("Soul") || item.Name.Contains("Cosmos") ||
                        item.Name.Contains("Shadow's Embrace") || item.Name.Contains("Aegis") ||
                        item.Name.Contains("Windwalker") || item.Name.Contains("Eternity") ||
                        item.Name.Contains("Crown of") || item.Name.Contains("Ring of Infinite"))
                    {
                        legendaryCount++;
                    }
                }
            }

            if (legendaryCount > 0)
            {
                VisualEffects.WriteLegendary($"✨ LEGENDARY ITEMS COLLECTED: {legendaryCount}\n");
            }

            Console.WriteLine();
            VisualEffects.WriteInfo("═══════════════════════════════════════════════════════════\n");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static void ShowHeroHighlight(Character hero)
        {
            var color = hero.Level switch
            {
                >= 80 => ConsoleColor.Magenta,
                >= 50 => ConsoleColor.Yellow,
                >= 25 => ConsoleColor.Cyan,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = color;
            Console.WriteLine($"▸ {hero.Name}");
            Console.ResetColor();

            Console.WriteLine($"  Class: {hero.GetType().Name}" + (hero.HasChampionClass ? $" ({hero.ChampionClass})" : ""));
            Console.WriteLine($"  Level: {hero.Level} ({Playerleveling.GetLevelTitle(hero.Level)})");

            // Show HP bar
            Console.Write("  ");
            VisualEffects.DrawProgressBarLine(hero.Health, hero.MaxHealth, 20, "HP");

            Console.WriteLine($"  Power Rating: {CalculatePowerRating(hero):N0}");

            if (hero.Pet != null)
            {
                Console.WriteLine($"  Companion: {hero.Pet.Name} (Lv {hero.Pet.Level})");
            }

            var equipped = GetEquippedCount(hero);
            if (equipped > 0)
            {
                Console.WriteLine($"  Equipped Items: {equipped}");
            }
        }

        private static int CalculatePowerRating(Character character)
        {
            int power = character.Level * 100;
            power += character.GetTotalStrength() * 5;
            power += character.GetTotalAgility() * 5;
            power += character.GetTotalIntelligence() * 5;
            power += character.MaxHealth;
            power += character.GetTotalArmorRating() * 10;

            if (character.HasChampionClass) power += 500;
            if (character.Pet != null) power += character.Pet.Level * 50;

            return power;
        }

        private static int GetEquippedCount(Character character)
        {
            int count = 0;
            if (character.Inventory.EquippedWeapon != null) count++;
            if (character.Inventory.EquippedArmor != null) count++;
            if (character.Inventory.EquippedAccessory != null) count++;
            if (character.Inventory.EquippedNecklace != null) count++;
            if (character.Inventory.EquippedRing1 != null) count++;
            if (character.Inventory.EquippedRing2 != null) count++;
            if (character.Inventory.EquippedOffHand != null) count++;
            return count;
        }
    }
}
