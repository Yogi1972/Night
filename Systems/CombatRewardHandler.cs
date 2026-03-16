using Night.Characters;
using Night.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Handles all combat rewards including XP, gold, and loot distribution
    /// </summary>
    internal static class CombatRewardHandler
    {
        /// <summary>
        /// Award experience, gold, and loot to the party after victory
        /// </summary>
        public static void AwardVictoryRewards(List<Character> party, Mob mob, Random rng)
        {
            Console.WriteLine();
            VisualEffects.ShowVictoryBanner();
            VisualEffects.WriteSuccess($"💀 {mob.Name} was {VisualEffects.GetRandomKillMessage()}\n");

            var loot = mob.DropLoot(rng);

            // Check for legendary drop
            CheckForLegendaryDrop(party);

            // Award experience
            AwardExperience(party, mob);

            // Distribute gold
            DistributeGold(party, loot.Gold);

            // Distribute items
            DistributeItems(party, loot.Items);
        }

        /// <summary>
        /// Check for and award legendary item drops
        /// </summary>
        private static void CheckForLegendaryDrop(List<Character> party)
        {
            var leader = party.FirstOrDefault(p => p.IsAlive);
            if (leader != null)
            {
                var legendaryItem = LegendaryItemSystem.TryGenerateLegendaryDrop(leader.Level);
                if (legendaryItem != null)
                {
                    LegendaryItemSystem.AnnounceItemFound(legendaryItem);
                    leader.Inventory.AddItem(legendaryItem);
                    VisualEffects.WriteSuccess($"✨ {leader.Name} received: {legendaryItem.Name}!\n");
                }
            }
        }

        /// <summary>
        /// Award experience to all alive party members
        /// </summary>
        private static void AwardExperience(List<Character> party, Mob mob)
        {
            int xp = Playerleveling.CalculateXPReward(mob, party.Count);

            foreach (var member in party.Where(p => p.IsAlive))
            {
                int memberXP = xp;

                // Apply pet XP boost
                if (member.Pet != null && member.Pet.Ability == PetAbility.ExperienceBoost)
                {
                    memberXP = (int)(memberXP * 1.10); // +10% XP
                }

                // Apply skill XP bonus
                if (member.SkillTree != null)
                {
                    double xpBonusPercent = member.SkillTree.GetTotalXPBonusPercent();
                    if (xpBonusPercent > 0)
                    {
                        memberXP = (int)(memberXP * (1.0 + xpBonusPercent));
                    }
                }

                Console.WriteLine($"{member.Name} gains {memberXP} XP!");
                member.GainExperience(memberXP);

                // Show XP progress bar
                VisualEffects.DrawProgressBarLine(
                    member.Experience,
                    member.ExperienceToNextLevel,
                    20,
                    $"  {member.Name}'s XP"
                );

                // Pet gains experience too
                if (member.Pet != null)
                {
                    member.Pet.GainExperience(xp / 4); // Pets gain 25% of character XP
                }

                // Apply pet post-combat effects
                ApplyPetPostCombatEffects(member);
            }
        }

        /// <summary>
        /// Apply pet post-combat healing/restoration effects
        /// </summary>
        private static void ApplyPetPostCombatEffects(Character member)
        {
            if (member.Pet == null) return;

            if (member.Pet.Ability == PetAbility.HealthRegen)
            {
                int healAmount = (int)(member.MaxHealth * 0.05);
                member.Heal(healAmount);
                Console.WriteLine($"🐾 {member.Pet.Name} helps {member.Name} recover {healAmount} HP!");
            }
            else if (member.Pet.Ability == PetAbility.ManaRegen)
            {
                // Restore mana for mana users, stamina for stamina users
                if (member is Warrior || member is Rogue)
                {
                    int staminaAmount = (int)(member.MaxStamina * 0.05);
                    member.RestoreStamina(staminaAmount);
                    Console.WriteLine($"🐾 {member.Pet.Name} helps {member.Name} recover {staminaAmount} Stamina!");
                }
                else if (member is Mage || member is Priest)
                {
                    int manaAmount = (int)(member.MaxMana * 0.05);
                    member.RestoreMana(manaAmount);
                    Console.WriteLine($"🐾 {member.Pet.Name} helps {member.Name} recover {manaAmount} Mana!");
                }
            }
        }

        /// <summary>
        /// Distribute gold to all party members
        /// </summary>
        private static void DistributeGold(List<Character> party, int baseGold)
        {
            if (baseGold <= 0) return;

            int totalGoldAmount = baseGold;

            // Calculate bonuses from first alive member (party leader for bonuses)
            var bonusReceiver = party.FirstOrDefault(p => p.IsAlive) ?? party[0];

            // Apply pet loot bonus
            if (bonusReceiver.Pet != null && bonusReceiver.Pet.Ability == PetAbility.LootBonus)
            {
                totalGoldAmount = (int)(totalGoldAmount * 1.20); // +20% gold
            }

            // Apply skill gold bonus
            if (bonusReceiver.SkillTree != null)
            {
                double goldBonusPercent = bonusReceiver.SkillTree.GetTotalGoldBonusPercent();
                if (goldBonusPercent > 0)
                {
                    totalGoldAmount = (int)(totalGoldAmount * (1.0 + goldBonusPercent));
                }
            }

            // Split gold evenly among all party members
            int goldPerMember = totalGoldAmount / party.Count;
            int remainder = totalGoldAmount % party.Count;

            foreach (var member in party)
            {
                int memberGold = goldPerMember;
                // Give remainder to first member
                if (member == party[0] && remainder > 0)
                {
                    memberGold += remainder;
                }
                member.Inventory.AddGold(memberGold);
            }

            VisualEffects.WriteSuccess($"💰 The party receives {totalGoldAmount} gold ({goldPerMember} per member)!\n");
        }

        /// <summary>
        /// Distribute items to party members in round-robin fashion
        /// </summary>
        private static void DistributeItems(List<Character> party, List<Item> items)
        {
            int itemIndex = 0;
            foreach (var item in items)
            {
                // Try to give item to party members in order, cycling through
                bool itemAdded = false;
                for (int attempt = 0; attempt < party.Count && !itemAdded; attempt++)
                {
                    var receiver = party[(itemIndex + attempt) % party.Count];
                    if (receiver.Inventory.AddItem(item))
                    {
                        VisualEffects.WriteSuccess($"🎁 {receiver.Name} finds {item.Name}!\n");
                        itemAdded = true;
                    }
                }

                if (!itemAdded)
                {
                    VisualEffects.WriteInfo($"❌ No space to pick up {item.Name}; it was left behind.\n");
                }

                itemIndex++;
            }
        }
    }
}
