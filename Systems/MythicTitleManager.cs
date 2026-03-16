using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    #region Mythic Title Info

    internal class MythicTitleInfo
    {
        public string Name { get; }
        public string Description { get; }
        public string PassiveEffect { get; }
        public int HealthBonus { get; }
        public int ManaBonus { get; }
        public int StaminaBonus { get; }
        public int StrengthBonus { get; }
        public int AgilityBonus { get; }
        public int IntelligenceBonus { get; }
        public int ArmorBonus { get; }

        public MythicTitleInfo(string name, string description, string passiveEffect,
            int health, int mana, int stamina, int strength, int agility, int intelligence, int armor)
        {
            Name = name;
            Description = description;
            PassiveEffect = passiveEffect;
            HealthBonus = health;
            ManaBonus = mana;
            StaminaBonus = stamina;
            StrengthBonus = strength;
            AgilityBonus = agility;
            IntelligenceBonus = intelligence;
            ArmorBonus = armor;
        }
    }

    #endregion

    #region Mythic Title Manager

    /// <summary>
    /// Manages the Mythic Title system — a second ascension at level 50 that grants
    /// permanent passive bonuses and a legendary identity independent of class.
    /// </summary>
    internal static class MythicTitleManager
    {
        private const int RequiredLevel = 50;

        #region Title Definitions

        private static readonly List<MythicTitleInfo> _titles =
        [
            new MythicTitleInfo(
                "The Undying",
                "💀✨ Those who bear this title have stared into the abyss and returned. They endure where others fall.",
                "Massive health increase. Survive a killing blow once per combat (revive with 10% HP).",
                200, 25, 50, 5, 5, 5, 10
            ),
            new MythicTitleInfo(
                "The Wrathful",
                "🔥⚔️  Fury given form. The Wrathful channel raw rage into devastating power that shatters all resistance.",
                "Significantly increased strength and damage output. Attacks have a chance to strike twice.",
                50, 0, 75, 25, 10, 0, 0
            ),
            new MythicTitleInfo(
                "The Sage",
                "📖🔮 Wisdom beyond mortal reckoning. The Sage bends magic to their will with effortless precision.",
                "Massive mana pool and spell power. Mana costs reduced by 20%.",
                25, 200, 0, 0, 5, 25, 5
            ),
            new MythicTitleInfo(
                "The Ironclad",
                "🛡️🏔️  Immovable. Unbreakable. The Ironclad stands as the last wall between their allies and oblivion.",
                "Supreme armor and damage reduction. Absorb a portion of damage dealt to nearby allies.",
                150, 0, 50, 10, 0, 5, 25
            ),
            new MythicTitleInfo(
                "The Swift",
                "💨⚡ A blur of motion. The Swift strike before their enemies can blink, turning speed into supremacy.",
                "Greatly increased agility and critical hit chance. Chance to dodge attacks entirely.",
                50, 25, 100, 5, 25, 5, 0
            ),
            new MythicTitleInfo(
                "The Eternal",
                "✨🌟 Touched by forces beyond the mortal plane. The Eternal embody perfect balance across all aspects of power.",
                "Balanced boost to all stats. Slow passive regeneration of health, mana, and stamina each turn.",
                100, 75, 75, 12, 12, 12, 8
            )
        ];

        #endregion

        #region Public Methods

        public static bool CanSelectMythicTitle(Character character)
        {
            return character.Level >= RequiredLevel && string.IsNullOrEmpty(character.MythicTitle);
        }

        public static void ShowMythicTitleSelection(Character character)
        {
            if (!CanSelectMythicTitle(character))
            {
                if (!string.IsNullOrEmpty(character.MythicTitle))
                {
                    Console.WriteLine($"❌ {character.Name} already bears the Mythic Title: {character.MythicTitle}");
                }
                else
                {
                    Console.WriteLine($"❌ {character.Name} must reach level {RequiredLevel} to earn a Mythic Title! (Current: {character.Level})");
                }
                return;
            }

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  ✦  MYTHIC TITLE ASCENSION  ✦                    ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  Your deeds have echoed across the realm. Legends are written    ║");
            Console.WriteLine("║  of those who reach this pinnacle. Choose the title that will    ║");
            Console.WriteLine("║  define your legend for all eternity.                            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            VisualEffects.WriteLineColored($"🌟 {character.Name}, you have reached level {character.Level}!", ConsoleColor.Yellow);
            if (!string.IsNullOrEmpty(character.ChampionClass))
            {
                VisualEffects.WriteLineColored($"   Champion Class: {character.ChampionClass}", ConsoleColor.Cyan);
            }
            Console.WriteLine("   You are now worthy of a Mythic Title!");
            Console.WriteLine();

            for (int i = 0; i < _titles.Count; i++)
            {
                var title = _titles[i];
                Console.WriteLine($"  [{i + 1}] \"{title.Name}\"");
                Console.WriteLine($"      {title.Description}");
                Console.WriteLine($"      Passive: {title.PassiveEffect}");
                Console.Write("      Stats: ");
                var bonuses = new List<string>();
                if (title.HealthBonus > 0) bonuses.Add($"+{title.HealthBonus} HP");
                if (title.ManaBonus > 0) bonuses.Add($"+{title.ManaBonus} Mana");
                if (title.StaminaBonus > 0) bonuses.Add($"+{title.StaminaBonus} Stamina");
                if (title.StrengthBonus > 0) bonuses.Add($"+{title.StrengthBonus} STR");
                if (title.AgilityBonus > 0) bonuses.Add($"+{title.AgilityBonus} AGI");
                if (title.IntelligenceBonus > 0) bonuses.Add($"+{title.IntelligenceBonus} INT");
                if (title.ArmorBonus > 0) bonuses.Add($"+{title.ArmorBonus} AR");
                Console.WriteLine(string.Join(", ", bonuses));
                Console.WriteLine();
            }

            Console.WriteLine($"  [{_titles.Count + 1}] Cancel (choose later)");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Your choice: ");
                var input = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(input, out int choice))
                {
                    if (choice == _titles.Count + 1)
                    {
                        Console.WriteLine("Mythic Title selection postponed. Visit a Training Hall to choose later.");
                        return;
                    }

                    if (choice >= 1 && choice <= _titles.Count)
                    {
                        var selected = _titles[choice - 1];
                        ApplyMythicTitle(character, selected);
                        return;
                    }
                }

                Console.WriteLine("Invalid choice. Try again.");
            }
        }

        /// <summary>
        /// Gets the mythic title info for a given title name.
        /// </summary>
        public static MythicTitleInfo? GetTitleInfo(string titleName)
        {
            foreach (var title in _titles)
            {
                if (string.Equals(title.Name, titleName, StringComparison.OrdinalIgnoreCase))
                    return title;
            }
            return null;
        }

        /// <summary>
        /// Displays mythic title info for a character who already has one.
        /// </summary>
        public static void DisplayMythicTitle(Character character)
        {
            if (string.IsNullOrEmpty(character.MythicTitle))
            {
                Console.WriteLine($"{character.Name} has not yet earned a Mythic Title.");
                return;
            }

            var info = GetTitleInfo(character.MythicTitle);
            if (info == null)
            {
                Console.WriteLine($"{character.Name} bears the Mythic Title: {character.MythicTitle}");
                return;
            }

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  🌟 {character.Name} — \"{info.Name}\"");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"   {info.Description}");
            Console.WriteLine($"   Passive: {info.PassiveEffect}");
        }

        #endregion

        #region Private Methods

        private static void ApplyMythicTitle(Character character, MythicTitleInfo title)
        {
            character.MythicTitle = title.Name;

            Console.Clear();
            Console.WriteLine();

            VisualEffects.WriteLineColored("╔══════════════════════════════════════════════════════════════════╗", ConsoleColor.Magenta);
            VisualEffects.WriteLineColored("║                                                                  ║", ConsoleColor.Magenta);
            VisualEffects.WriteLineColored("║            ✦  MYTHIC ASCENSION COMPLETE  ✦                       ║", ConsoleColor.Yellow);
            VisualEffects.WriteLineColored("║                                                                  ║", ConsoleColor.Magenta);
            VisualEffects.WriteLineColored("╚══════════════════════════════════════════════════════════════════╝", ConsoleColor.Magenta);
            Console.WriteLine();

            System.Threading.Thread.Sleep(500);

            VisualEffects.WriteLineColored($"   {character.Name} shall henceforth be known as:", ConsoleColor.White);
            Console.WriteLine();
            VisualEffects.WriteLineColored($"       ✦  {character.Name} \"{title.Name}\"  ✦", ConsoleColor.Yellow);
            Console.WriteLine();

            System.Threading.Thread.Sleep(500);

            // Apply stat bonuses
            Console.WriteLine("   ⚡ Applying Mythic bonuses...");
            Console.WriteLine();

            if (title.HealthBonus > 0)
            {
                character.ApplyMythicStatBonus("MaxHealth", title.HealthBonus);
                VisualEffects.WriteSuccess($"   ✓ Max Health +{title.HealthBonus}\n");
            }
            if (title.ManaBonus > 0)
            {
                character.ApplyMythicStatBonus("MaxMana", title.ManaBonus);
                VisualEffects.WriteSuccess($"   ✓ Max Mana +{title.ManaBonus}\n");
            }
            if (title.StaminaBonus > 0)
            {
                character.ApplyMythicStatBonus("MaxStamina", title.StaminaBonus);
                VisualEffects.WriteSuccess($"   ✓ Max Stamina +{title.StaminaBonus}\n");
            }
            if (title.StrengthBonus > 0)
            {
                character.ApplyMythicStatBonus("Strength", title.StrengthBonus);
                VisualEffects.WriteSuccess($"   ✓ Strength +{title.StrengthBonus}\n");
            }
            if (title.AgilityBonus > 0)
            {
                character.ApplyMythicStatBonus("Agility", title.AgilityBonus);
                VisualEffects.WriteSuccess($"   ✓ Agility +{title.AgilityBonus}\n");
            }
            if (title.IntelligenceBonus > 0)
            {
                character.ApplyMythicStatBonus("Intelligence", title.IntelligenceBonus);
                VisualEffects.WriteSuccess($"   ✓ Intelligence +{title.IntelligenceBonus}\n");
            }
            if (title.ArmorBonus > 0)
            {
                character.ApplyMythicStatBonus("ArmorRating", title.ArmorBonus);
                VisualEffects.WriteSuccess($"   ✓ Armor Rating +{title.ArmorBonus}\n");
            }

            // Fully restore after ascension
            character.Heal(character.MaxHealth - character.Health);
            character.RestoreMana(character.MaxMana - character.Mana);
            character.RestoreStamina(character.MaxStamina - character.Stamina);

            Console.WriteLine();
            VisualEffects.WriteHealing("   💚 Health, Mana, and Stamina fully restored!\n");
            Console.WriteLine();
            VisualEffects.WriteLineColored($"   Passive: {title.PassiveEffect}", ConsoleColor.Cyan);

            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Combat Passive Helpers

        // Mythic passive constants
        private const int WrathfulDoubleStrikeChance = 20; // 20% chance
        private const int SwiftDodgeChance = 15;           // 15% chance
        private const int IroncladInterceptChance = 25;    // 25% chance
        private const double IroncladAbsorbPercent = 0.30; // 30% of damage redirected
        private const double SageManaCostReduction = 0.20; // 20% reduction
        private const double EternalHpRegenPercent = 0.03; // 3% max HP per turn
        private const double EternalMpRegenPercent = 0.03; // 3% max Mana per turn
        private const double EternalSpRegenPercent = 0.03; // 3% max Stamina per turn

        private static readonly Random _combatRng = new Random();

        /// <summary>
        /// Check if The Wrathful's double-strike triggers (20% chance).
        /// </summary>
        public static bool RollWrathfulDoubleStrike(Character character)
        {
            if (!character.HasMythicTitle || character.MythicTitle != "The Wrathful") return false;
            return _combatRng.Next(1, 101) <= WrathfulDoubleStrikeChance;
        }

        /// <summary>
        /// Check if The Swift's dodge triggers (15% chance).
        /// </summary>
        public static bool RollSwiftDodge(Character target)
        {
            if (!target.HasMythicTitle || target.MythicTitle != "The Swift") return false;
            return _combatRng.Next(1, 101) <= SwiftDodgeChance;
        }

        /// <summary>
        /// Check if The Ironclad intercepts damage for an ally (25% chance when ally is hit).
        /// Returns the Ironclad character if intercept triggers, null otherwise.
        /// </summary>
        public static Character? TryIroncladIntercept(Character target, List<Character> party)
        {
            foreach (var member in party)
            {
                if (member == target) continue;
                if (!member.IsAlive) continue;
                if (!member.HasMythicTitle || member.MythicTitle != "The Ironclad") continue;
                if (_combatRng.Next(1, 101) <= IroncladInterceptChance)
                {
                    return member;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the portion of damage The Ironclad absorbs for an ally.
        /// </summary>
        public static int GetIroncladAbsorbAmount(int damage)
        {
            return (int)(damage * IroncladAbsorbPercent);
        }

        /// <summary>
        /// Apply The Eternal's passive regeneration at the start of a turn.
        /// </summary>
        public static void ApplyEternalRegen(Character character)
        {
            if (!character.HasMythicTitle || character.MythicTitle != "The Eternal") return;
            if (!character.IsAlive) return;

            int hpRegen = Math.Max(1, (int)(character.MaxHealth * EternalHpRegenPercent));
            int mpRegen = Math.Max(1, (int)(character.MaxMana * EternalMpRegenPercent));
            int spRegen = Math.Max(1, (int)(character.MaxStamina * EternalSpRegenPercent));

            bool healed = false;

            if (character.Health < character.MaxHealth)
            {
                character.Heal(hpRegen);
                healed = true;
            }
            if (character.Mana < character.MaxMana)
            {
                character.RestoreMana(mpRegen);
                healed = true;
            }
            if (character.Stamina < character.MaxStamina)
            {
                character.RestoreStamina(spRegen);
                healed = true;
            }

            if (healed)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"✨ {character.Name}'s \"The Eternal\" passive: +{hpRegen} HP, +{mpRegen} MP, +{spRegen} SP");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Reset per-combat Mythic Title state for all party members.
        /// </summary>
        public static void ResetCombatState(List<Character> party)
        {
            foreach (var member in party)
            {
                member.UndyingUsedThisCombat = false;
            }
        }

        #endregion
    }

    #endregion
}
