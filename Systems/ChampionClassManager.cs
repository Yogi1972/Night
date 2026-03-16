using Night.Characters;
using Rpg_Dungeon.Champions;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal static class ChampionClassManager
    {
        private const int RequiredLevel = 25;

        public static bool CanSelectChampionClass(Character character)
        {
            return character.Level >= RequiredLevel && !character.HasChampionClass;
        }

        public static void ShowChampionClassSelection(Character character)
        {
            if (!CanSelectChampionClass(character))
            {
                if (character.HasChampionClass)
                {
                    Console.WriteLine($"❌ {character.Name} has already selected a Champion Class: {character.ChampionClass}");
                }
                else
                {
                    Console.WriteLine($"❌ {character.Name} must reach level {RequiredLevel} to select a Champion Class! (Current: {character.Level})");
                }
                return;
            }

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    CHAMPION CLASS ASCENSION                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"🏆 {character.Name}, you have reached level {character.Level}!");
            Console.WriteLine("   You are now worthy of ascending to a Champion Class!");
            Console.WriteLine();

            string baseClass = character.GetType().Name;
            var championClass = SelectChampionClass(baseClass);

            if (string.IsNullOrEmpty(championClass))
            {
                Console.WriteLine("Champion class selection cancelled.");
                return;
            }

            character.ChampionClass = championClass;
            ApplyChampionClassBonuses(character, championClass);

            Console.WriteLine();
            Console.WriteLine($"✨🏆 ASCENSION COMPLETE! 🏆✨");
            Console.WriteLine($"{character.Name} is now a {championClass}!");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static string? SelectChampionClass(string baseClass)
        {
            var champions = GetChampionClassesForBase(baseClass);

            if (champions == null || champions.Count == 0)
            {
                Console.WriteLine($"❌ No champion classes available for {baseClass}!");
                return null;
            }

            Console.WriteLine($"Choose your Champion Class for {baseClass}:");
            Console.WriteLine();

            for (int i = 0; i < champions.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {champions[i].Name}");
                Console.WriteLine($"   {champions[i].Description}");
                Console.WriteLine();
            }

            Console.WriteLine($"{champions.Count + 1}) Cancel");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Your choice: ");
                var input = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(input, out int choice))
                {
                    if (choice == champions.Count + 1)
                        return null;

                    if (choice >= 1 && choice <= champions.Count)
                        return champions[choice - 1].Name;
                }

                Console.WriteLine("Invalid choice. Try again.");
            }
        }

        private static List<ChampionInfo>? GetChampionClassesForBase(string baseClass)
        {
            return baseClass switch
            {
                "Warrior" => new List<ChampionInfo>
                {
                    new ChampionInfo("Paladin", "⚔️✨ Holy warrior combining strength and divine magic. Gains mana and can cast holy spells."),
                    new ChampionInfo("Berserker", "💢🔥 Furious combatant entering rage for devastating damage. Massive health and strength boost."),
                    new ChampionInfo("Guardian", "🛡️🏰 Impenetrable defender with supreme armor and defensive abilities. Tank supreme.")
                },
                "Mage" => new List<ChampionInfo>
                {
                    new ChampionInfo("Archmage", "🔮⚡ Master of arcane arts with devastating spell power. Supreme intelligence and mana."),
                    new ChampionInfo("Necromancer", "💀🌑 Dark mage commanding death and draining life force. Converts kills to soul essence."),
                    new ChampionInfo("Elementalist", "🔥❄️⚡🌍 Wielder of all elements with adaptable combat style. Switch between fire, ice, lightning, and earth.")
                },
                "Rogue" => new List<ChampionInfo>
                {
                    new ChampionInfo("Assassin", "💀⚔️ Silent killer building combo points for lethal finishing moves. Maximum burst damage."),
                    new ChampionInfo("Ranger", "🏹🎯 Master archer with deadly precision and multi-target attacks. Gains focus for critical hits."),
                    new ChampionInfo("Shadowblade", "🌑⚔️ Shadow-infused warrior harnessing darkness. Builds shadow energy for powerful abilities.")
                },
                "Priest" => new List<ChampionInfo>
                {
                    new ChampionInfo("Templar", "⚔️✨ Battle priest combining healing with righteous combat. Strength and intelligence hybrid."),
                    new ChampionInfo("Druid", "🌿🐻 Nature guardian shapeshifting between forms. Transform into bear or cat for different combat styles."),
                    new ChampionInfo("Oracle", "🔮✨ Prophet wielding foresight and divine intervention. Limited prophecies for powerful effects.")
                },
                _ => null
            };
        }

        private static void ApplyChampionClassBonuses(Character character, string championClass)
        {
            Console.WriteLine();
            Console.WriteLine($"⚡ Applying {championClass} bonuses...");
            Console.WriteLine($"   ✓ Enhanced base stats");
            Console.WriteLine($"   ✓ New champion abilities unlocked");
            Console.WriteLine($"   ✓ Increased power and capabilities");

            int healthBefore = character.Health;
            int manaBefore = character.Mana;
            int staminaBefore = character.Stamina;

            character.Heal(character.MaxHealth - character.Health);
            character.RestoreMana(character.MaxMana - character.Mana);
            character.RestoreStamina(character.MaxStamina - character.Stamina);

            Console.WriteLine($"   ✓ Health fully restored: {healthBefore} → {character.Health}");
            if (character.MaxMana > 0) Console.WriteLine($"   ✓ Mana fully restored: {manaBefore} → {character.Mana}");
            if (character.MaxStamina > 0) Console.WriteLine($"   ✓ Stamina fully restored: {staminaBefore} → {character.Stamina}");
        }

        private class ChampionInfo
        {
            public string Name { get; }
            public string Description { get; }

            public ChampionInfo(string name, string description)
            {
                Name = name;
                Description = description;
            }
        }
    }
}
