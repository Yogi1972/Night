using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Utility for rating party power and dungeon difficulty and computing chance to complete a dungeon.
    /// This uses heuristic weights (tunable) and produces integer ratings 1..100.
    /// Also provides a small simulator to try several run attempts (e.g. 5 tries) using a Random instance.
    /// </summary>
    internal static class PowerRating
    {
        // Tunable normalization constants
        private const double BaselinePerMember = 220.0; // higher = easier dungeons relative to party

        // Rate an individual character into a raw power score
        private static double RateCharacterRaw(Character c)
        {
            if (c == null) return 0;

            double raw = 0.0;

            // Core progression
            raw += c.Level * 26.0;

            // Stats (include equipment + skills via GetTotal* helpers)
            raw += c.GetTotalStrength() * 2.2;
            raw += c.GetTotalAgility() * 1.8;
            raw += c.GetTotalIntelligence() * 2.1;

            // Health and defenses
            raw += c.GetTotalMaxHP() * 0.12;
            raw += c.GetTotalArmorRating() * 3.2;

            // Skill tree bonuses (damage/defense/hp)
            if (c.SkillTree != null)
            {
                raw += c.SkillTree.GetTotalDamageBonus() * 4.0;
                raw += c.SkillTree.GetTotalDefenseBonus() * 3.2;
                raw += c.SkillTree.GetTotalMaxHPBonus() * 0.3;
                raw += c.SkillTree.GetTotalCritChanceBonus() * 1.5;
            }

            // Combat abilities (count and potential damage multipliers)
            if (c.Abilities != null)
            {
                raw += c.Abilities.Count * 6.0;
                foreach (var a in c.Abilities)
                {
                    raw += a.BaseDamage * 0.8;
                    raw += (a.DamageMultiplier - 1.0) * 12.0;
                }
            }

            // Pet influence
            if (c.Pet != null)
            {
                raw += c.Pet.Level * 9.0;
                switch (c.Pet.Ability)
                {
                    case PetAbility.DamageBoost:
                        raw += 6.0;
                        break;
                    case PetAbility.DefenseBoost:
                        raw += 6.0;
                        break;
                    case PetAbility.ExperienceBoost:
                        raw += 3.0;
                        break;
                    case PetAbility.LootBonus:
                        raw += 2.0;
                        break;
                    case PetAbility.HealthRegen:
                    case PetAbility.ManaRegen:
                        raw += 4.0;
                        break;
                }
            }

            // Minor bonuses: champion class, stance, status effects
            if (!string.IsNullOrEmpty(c.ChampionClass)) raw += 8.0;
            if (c.CurrentStance == CombatStance.Balanced) raw += 1.0;

            // Mythic Title passive combat bonuses
            if (c.HasMythicTitle)
            {
                raw += 12.0; // base bonus for having any Mythic Title
                switch (c.MythicTitle)
                {
                    case "The Undying":  raw += 18.0; break; // survive killing blow
                    case "The Wrathful": raw += 16.0; break; // double-strike chance
                    case "The Sage":     raw += 14.0; break; // mana cost reduction
                    case "The Ironclad": raw += 15.0; break; // ally damage intercept
                    case "The Swift":    raw += 14.0; break; // dodge chance
                    case "The Eternal":  raw += 13.0; break; // passive regen
                }
            }

            return Math.Max(0.0, raw);
        }

        /// <summary>
        /// Returns a party power rating on a 1..100 scale.
        /// Includes all characters that are alive in the party.
        /// </summary>
        public static int RateParty(List<Character> party)
        {
            if (party == null || party.Count == 0) return 1;

            var alive = party.Where(p => p.IsAlive).ToList();
            if (alive.Count == 0) return 1;

            double totalRaw = alive.Sum(p => RateCharacterRaw(p));

            // Normalize against baseline per-member
            double baseline = BaselinePerMember * alive.Count;
            double percent = (totalRaw / baseline) * 100.0;

            int result = (int)Math.Round(Math.Max(1.0, Math.Min(100.0, percent)));

            // Debug-like summary printed for visibility
            Console.WriteLine($"[PowerRating] Party raw power: {totalRaw:F1} | normalized: {result}% (baseline {baseline:F1})");
            int idx = 0;
            foreach (var c in alive)
            {
                idx++;
                Console.WriteLine($"   [{idx}] {c.Name} Lv{c.Level} raw: {RateCharacterRaw(c):F1}");
            }

            return result;
        }

        /// <summary>
        /// Rate a dungeon (DungeonLocation) into a 1..100 difficulty score.
        /// Uses recommended level, floors and optional seed-based variance.
        /// </summary>
        public static int RateDungeon(DungeonLocation loc)
        {
            if (loc == null) return 1;

            double raw = 0.0;
            raw += loc.RecommendedLevel * 30.0;
            raw += loc.Floors * 18.0;
            // Seed-derived variance (small) to differentiate dungeons
            raw += (loc.Seed % 7) * 6.0;

            // Boss presence (floors always include a boss on last floor)
            raw += 20.0;

            // Normalize roughly to 0..100
            double normalized = (raw / 500.0) * 100.0; // 500 chosen as upper raw anchor
            int result = (int)Math.Round(Math.Max(1.0, Math.Min(100.0, normalized)));
            Console.WriteLine($"[PowerRating] Dungeon '{loc.Name}' raw difficulty: {raw:F1} -> {result}%");
            return result;
        }

        /// <summary>
        /// Compute the chance (1..100) to complete the dungeon based on party and dungeon ratings.
        /// Higher party rating increases odds. Returns integer percent.
        /// </summary>
        public static int ChanceToComplete(List<Character> party, DungeonLocation loc)
        {
            int partyRating = RateParty(party);
            int dungeonRating = RateDungeon(loc);

            // Basic chance: base 50, modify by difference scaled
            int chance = 50 + (partyRating - dungeonRating);

            // Further adjustments: party size & composition
            int aliveCount = party.Count(p => p.IsAlive);
            chance += Math.Min(5, aliveCount - 1); // small bonus for larger parties

            // Cap and floor
            chance = Math.Max(1, Math.Min(100, chance));

            Console.WriteLine($"[PowerRating] Computed chance to complete: {chance}% (Party {partyRating}% vs Dungeon {dungeonRating}%)");
            return chance;
        }

        /// <summary>
        /// Simulate N attempts to complete the dungeon using RNG. Returns true if any attempt succeeds.
        /// Also prints summary of attempts. Default attempts=5.
        /// </summary>
        public static bool SimulateDungeonAttempts(List<Character> party, DungeonLocation loc, int attempts = 5, Random? rng = null)
        {
            if (attempts <= 0) attempts = 5;
            rng ??= new Random();

            int chance = ChanceToComplete(party, loc);
            int successes = 0;
            for (int i = 0; i < attempts; i++)
            {
                int roll = rng.Next(1, 101);
                bool ok = roll <= chance;
                Console.WriteLine($"   Attempt {i + 1}: roll {roll} -> {(ok ? "SUCCESS" : "FAIL")}");
                if (ok) successes++;
            }

            Console.WriteLine($"[PowerRating] Simulation: {successes}/{attempts} successful attempts.");
            return successes > 0;
        }

        /// <summary>
        /// Provide simple recommended actions before entering a dungeon based on detected deficits.
        /// Returns a list of short guidance strings.
        /// </summary>
        public static List<string> RecommendActions(List<Character> party, DungeonLocation loc)
        {
            var suggestions = new List<string>();
            if (party == null || party.Count == 0) return suggestions;

            // Party avg level vs dungeon
            double partyAvgLevel = party.Where(p => p.IsAlive).DefaultIfEmpty(party[0]).Average(p => (double)p.Level);
            if (partyAvgLevel < loc.RecommendedLevel - 2)
            {
                suggestions.Add("Your party is underleveled for this dungeon. Consider leveling up or choosing an easier dungeon.");
            }

            // Health status
            double avgHealthPercent = party.Where(p => p.IsAlive).Select(p => (double)p.Health / Math.Max(1, p.GetTotalMaxHP())).DefaultIfEmpty(1.0).Average();
            if (avgHealthPercent < 0.75)
            {
                suggestions.Add("Average party HP is low. Rest at camp or use health potions before entering.");
            }

            // Healer presence
            bool hasHealer = party.Any(p => p is Priest || (p.SkillTree != null && p.SkillTree.HasSkill("Healing Prayer")));
            int chance = ChanceToComplete(party, loc);
            if (!hasHealer && chance < 60)
            {
                suggestions.Add("No obvious healer detected. Consider bringing a Priest or healing consumables.");
            }

            // Mana/stamina supplies
            var manaUsers = party.Where(p => p is Mage || p is Priest).ToList();
            if (manaUsers.Count > 0)
            {
                double avgMana = manaUsers.Average(m => m.GetTotalMaxMana());
                if (avgMana < 40)
                    suggestions.Add("Party spellcasters have low mana pools — buy Mana Potions or rest to recover.");
            }

            var stamUsers = party.Where(p => p is Warrior || p is Rogue).ToList();
            if (stamUsers.Count > 0)
            {
                double avgStam = stamUsers.Average(m => m.GetTotalMaxStamina());
                if (avgStam < 40)
                    suggestions.Add("Warriors/rogues have low stamina — buy Stamina Potions or rest.");
            }

            // Gold check for buying supplies
            int totalGold = party.Sum(p => p.Inventory?.Gold ?? 0);
            if (totalGold < party.Count * 30)
            {
                suggestions.Add("Party has little gold — you may lack funds for potions or gear.");
            }

            // Generic suggestions if chance is very low
            if (chance < 35 && suggestions.Count == 0)
            {
                suggestions.Add("This dungeon looks extremely dangerous. Consider leaving to level up, improve gear, or recruit allies.");
            }

            // Advanced stat improvement recommendations
            try
            {
                var alive = party.Where(p => p.IsAlive).ToList();
                if (alive.Count > 0 && loc != null)
                {
                    double partyRaw = alive.Sum(p => RateCharacterRaw(p));
                    double dungeonRaw = GetDungeonRaw(loc);
                    if (partyRaw < dungeonRaw)
                    {
                        double deficit = dungeonRaw - partyRaw;
                        double perMember = deficit / alive.Count;

                        // Using the same contribution weights from RateCharacterRaw
                        // HP weight = 0.12, Armor weight = 3.2, Strength weight = 2.2
                        int hpPerMember = (int)Math.Ceiling(perMember / 0.12);
                        int armorPerMember = (int)Math.Ceiling(perMember / 3.2);
                        int strengthPerMember = (int)Math.Ceiling(perMember / 2.2);

                        // Round hp to nearest 5 for readability
                        hpPerMember = ((hpPerMember + 4) / 5) * 5;

                        suggestions.Add($"Estimated per-member improvement to close the gap: +{hpPerMember} Max HP OR +{armorPerMember} Armor OR +{strengthPerMember} Strength.");
                        suggestions.Add("Actionable: buy HP/armor upgrades, equip stronger gear, or increase strength via weapons/skills.");
                    }
                }
            }
            catch { }

            return suggestions;
        }

        // Return the dungeon raw score used by RateDungeon (reverse of normalization)
        private static double GetDungeonRaw(DungeonLocation loc)
        {
            if (loc == null) return 0.0;
            double raw = 0.0;
            raw += loc.RecommendedLevel * 30.0;
            raw += loc.Floors * 18.0;
            raw += (loc.Seed % 7) * 6.0;
            raw += 20.0;
            return raw;
        }
    }
}
