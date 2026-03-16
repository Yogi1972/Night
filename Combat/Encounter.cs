using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Encounter Difficulty

    internal enum EncounterDifficulty
    {
        Easy,        // Weaker enemies, fewer than party
        Normal,      // Balanced encounter
        Hard,        // Tougher enemies or outnumbered
        Elite,       // Boss-like encounter
        Boss         // Epic boss fight
    }


    #endregion

    #region Encounter Class

    internal class Encounter
    {
        #region Properties

        public List<Mob> Enemies { get; private set; }
        public EncounterDifficulty Difficulty { get; private set; }
        public string EncounterDescription { get; private set; }
        public int TotalEnemyCount => Enemies?.Count ?? 0;
        public bool IsActive => Enemies != null && Enemies.Count > 0;

        #endregion

        #region Fields

        private readonly Random _rng;
        private int _partySize;
        private int _partyAverageLevel;
        private int _partyTotalPower;

        // Tunable heuristic weights
        private const double WeightLevel = 18.0; // influence of level
        private const double WeightHP = 0.22;    // influence of HP
        private const double WeightStat = 2.2;   // influence of each stat
        private const double WeightArmor = 3.5;  // influence of armor rating

        // Difficulty multipliers - tuned for playtesting
        private static readonly Dictionary<EncounterDifficulty, double> DifficultyMultiplier = new()
        {
            { EncounterDifficulty.Easy, 0.65 },
            { EncounterDifficulty.Normal, 1.0 },
            { EncounterDifficulty.Hard, 1.6 },
            { EncounterDifficulty.Elite, 2.4 },
            { EncounterDifficulty.Boss, 3.5 }
        };

        #endregion

        #region Constructor

        public Encounter(Random? rng = null)
        {
            _rng = rng ?? new Random();
            Enemies = new List<Mob>();
            Difficulty = EncounterDifficulty.Normal;
            EncounterDescription = string.Empty;
        }

        #endregion

        #region Encounter Generation

        /// <summary>
        /// Generate a balanced encounter based on the party's size and strength
        /// </summary>
        public void GenerateEncounter(List<Character> party, EncounterDifficulty difficulty = EncounterDifficulty.Normal)
        {
            if (party == null || party.Count == 0)
            {
                throw new ArgumentException("Party cannot be null or empty");
            }

            AnalyzeParty(party);
            Difficulty = difficulty;
            Enemies = new List<Mob>();

            int mobCount = CalculateMobCount(difficulty);
            int mobLevel = CalculateMobLevel(difficulty);

            // Generate the initial set of mobs (use injected RNG)
            for (int i = 0; i < mobCount; i++)
            {
                var mob = Mobs.GetRandomMobForParty(party, mobLevel, _rng);
                Enemies.Add(mob);
            }

            // Balance enemies to approximate party total power based on difficulty
            // Determine multiplier from tuned table
            double difficultyMultiplier = DifficultyMultiplier.ContainsKey(difficulty) ? DifficultyMultiplier[difficulty] : 1.0;

            int targetEnemyPower = Math.Max(1, (int)Math.Round(_partyTotalPower * difficultyMultiplier));

            Console.WriteLine($"   [Balance] Party power: {_partyTotalPower}, target enemy power: {targetEnemyPower} (multiplier {difficultyMultiplier})");
            int beforePower = Enemies.Sum(e => EstimateMobPower(e));
            Console.WriteLine($"   [Balance] Initial enemy power: {beforePower}");
            BalanceEnemies(party, mobLevel, targetEnemyPower);
            int afterPower = Enemies.Sum(e => EstimateMobPower(e));
            Console.WriteLine($"   [Balance] Final enemy power: {afterPower}");
            Console.WriteLine($"   [Balance] Adjusted by: {afterPower - beforePower}");

            EncounterDescription = GenerateDescription();

            Console.WriteLine($"\n⚔️  {EncounterDescription}");
            Console.WriteLine($"📊 Encounter Difficulty: {Difficulty}");
            Console.WriteLine($"👥 Party Size: {_partySize} | Enemy Count: {mobCount}");
        }

        /// <summary>
        /// Generate a specific encounter with named enemies
        /// </summary>
        public void GenerateSpecificEncounter(List<Character> party, string mobTypeName, int count, int level, EncounterDifficulty difficulty = EncounterDifficulty.Normal)
        {
            if (party == null || party.Count == 0)
            {
                throw new ArgumentException("Party cannot be null or empty");
            }

            AnalyzeParty(party);
            Difficulty = difficulty;
            Enemies = new List<Mob>();

            for (int i = 0; i < count; i++)
            {
                var mob = Mobs.GetRandomMobForParty(party, level);
                Enemies.Add(mob);
            }

            EncounterDescription = $"You encounter {count} {mobTypeName}(s)!";
            Console.WriteLine($"\n⚔️  {EncounterDescription}");
        }

        /// <summary>
        /// Generate a boss encounter
        /// </summary>
        public void GenerateBossEncounter(List<Character> party, string bossName = "Boss")
        {
            if (party == null || party.Count == 0)
            {
                throw new ArgumentException("Party cannot be null or empty");
            }

            AnalyzeParty(party);
            Difficulty = EncounterDifficulty.Boss;
            Enemies = new List<Mob>();

            // Boss is 2-3 levels higher than party average
            int bossLevel = _partyAverageLevel + _rng.Next(2, 4);
            var boss = Mobs.GetRandomMobForParty(party, bossLevel, _rng);

            // Add minions based on party size
            int minionCount = Math.Max(1, _partySize / 2);
            int minionLevel = Math.Max(1, _partyAverageLevel - 1);

            Enemies.Add(boss);
            for (int i = 0; i < minionCount; i++)
            {
                var minion = Mobs.GetRandomMobForParty(party, minionLevel, _rng);
                Enemies.Add(minion);
            }

            EncounterDescription = $"⚠️  BOSS ENCOUNTER: {boss.Name} appears with {minionCount} minion(s)!";
            VisualEffects.ShowBossEncounterIntro(boss.Name);
            // Balance boss encounter: preserve the boss (first enemy) while balancing minions
            int beforePower = Enemies.Sum(e => EstimateMobPower(e));
            int targetEnemyPower = Math.Max(1, (int)Math.Round(_partyTotalPower * DifficultyMultiplier[EncounterDifficulty.Boss]));
            Console.WriteLine($"   [Balance] Boss encounter initial power: {beforePower}, target: {targetEnemyPower}");
            BalanceEnemies(party, minionLevel, targetEnemyPower, preserveFirstEnemy: true);
            int afterPower = Enemies.Sum(e => EstimateMobPower(e));
            Console.WriteLine($"   [Balance] Boss encounter final power: {afterPower} (adjust {afterPower - beforePower})");

            Console.WriteLine($"\n{EncounterDescription}");
        }

        #endregion

        #region Analysis Methods

        private void AnalyzeParty(List<Character> party)
        {
            _partySize = party.Count;

            // Calculate party average level
            int totalLevel = 0;
            int aliveCount = 0;
            foreach (var member in party)
            {
                if (member.IsAlive)
                {
                    totalLevel += member.Level;
                    aliveCount++;
                }
            }

            _partyAverageLevel = aliveCount > 0 ? totalLevel / aliveCount : 1;

            // Calculate party power (level + stats)
            _partyTotalPower = 0;
            foreach (var member in party)
            {
                if (member.IsAlive)
                {
                    int memberPower = member.Level * 10 +
                                     member.GetTotalStrength() +
                                     member.GetTotalAgility() +
                                     member.GetTotalIntelligence();
                    _partyTotalPower += memberPower;
                }
            }
        }

        private int CalculateMobCount(EncounterDifficulty difficulty)
        {
            int baseCount;

            switch (difficulty)
            {
                case EncounterDifficulty.Easy:
                    // Easy: Always fewer enemies than party members
                    baseCount = Math.Max(1, _partySize - 1);
                    break;

                case EncounterDifficulty.Normal:
                    // Normal: Equal to party size or slightly fewer
                    baseCount = _partySize;
                    if (_partySize > 2 && _rng.Next(0, 2) == 0)
                    {
                        baseCount -= 1; // Sometimes one less
                    }
                    break;

                case EncounterDifficulty.Hard:
                    // Hard: More enemies than party (up to 1.5x party size)
                    baseCount = _partySize + _rng.Next(1, Math.Max(2, _partySize / 2 + 1));
                    break;

                case EncounterDifficulty.Elite:
                    // Elite: Fewer but much stronger enemies
                    baseCount = Math.Max(1, _partySize / 2);
                    break;

                case EncounterDifficulty.Boss:
                    // Boss: 1 boss + minions
                    baseCount = 1 + Math.Max(1, _partySize / 2);
                    break;

                default:
                    baseCount = _partySize;
                    break;
            }

            return Math.Max(1, baseCount);
        }

        private int CalculateMobLevel(EncounterDifficulty difficulty)
        {
            int baseLevel = _partyAverageLevel;

            switch (difficulty)
            {
                case EncounterDifficulty.Easy:
                    // Easy: 1-2 levels below party
                    baseLevel = Math.Max(1, _partyAverageLevel - _rng.Next(1, 3));
                    break;

                case EncounterDifficulty.Normal:
                    // Normal: Same level as party, +/- 1
                    baseLevel = _partyAverageLevel + _rng.Next(-1, 2);
                    break;

                case EncounterDifficulty.Hard:
                    // Hard: 1-2 levels above party
                    baseLevel = _partyAverageLevel + _rng.Next(1, 3);
                    break;

                case EncounterDifficulty.Elite:
                    // Elite: 2-4 levels above party
                    baseLevel = _partyAverageLevel + _rng.Next(2, 5);
                    break;

                case EncounterDifficulty.Boss:
                    // Boss: 3-5 levels above party
                    baseLevel = _partyAverageLevel + _rng.Next(3, 6);
                    break;

                default:
                    baseLevel = _partyAverageLevel;
                    break;
            }

            return Math.Max(1, baseLevel);
        }

        // Estimate a simple power value for a mob (level + stats weight)
        private int EstimateMobPower(Mob? m)
        {
            if (m == null) return 0;
            // Tuned heuristic using constants
            double power = 0.0;
            power += m.Level * WeightLevel;
            power += m.Health * WeightHP;
            power += (m.Strength + m.Agility + m.Intelligence) * WeightStat;
            power += m.ArmorRating * WeightArmor;
            return Math.Max(1, (int)Math.Round(power));
        }

        // Adjust the enemy list to better match a target total power
        private void BalanceEnemies(List<Character> party, int mobLevel, int targetPower, bool preserveFirstEnemy = false)
        {
            if (targetPower <= 0) return;

            int currentPower = Enemies.Sum(e => EstimateMobPower(e));

            if (Math.Abs(currentPower - targetPower) <= Math.Max(5, targetPower / 10)) return;

            int attempts = 0;
            while (currentPower < targetPower && attempts < 50)
            {
                if (_rng.Next(100) < 60 || Enemies.Count == 0)
                {
                    var newMob = Mobs.GetRandomMobForParty(party, mobLevel + _rng.Next(0, 2), _rng);
                    Enemies.Add(newMob);
                    currentPower += EstimateMobPower(newMob);
                    Console.WriteLine($"   [Balance] Added mob: {newMob.Name} (Lv {newMob.Level}) -> power {EstimateMobPower(newMob)}");
                }
                else
                {
                    int idx = _rng.Next(Enemies.Count);
                    var scaled = Enemies[idx].ScaleToLevel(Enemies[idx].Level + 1);
                    Console.WriteLine($"   [Balance] Scaled up: {Enemies[idx].Name} from L{Enemies[idx].Level} to L{scaled.Level}");
                    currentPower -= EstimateMobPower(Enemies[idx]);
                    Enemies[idx] = scaled;
                    currentPower += EstimateMobPower(scaled);
                }
                attempts++;
            }

            attempts = 0;
            while (currentPower > targetPower && attempts < 50 && Enemies.Count > 1)
            {
                if (_rng.Next(100) < 60 && Enemies.Count > 1)
                {
                    Mob? weakest = null;
                    if (preserveFirstEnemy && Enemies.Count > 1)
                    {
                        weakest = Enemies.Where((e, i) => i != 0).OrderBy(e => EstimateMobPower(e)).FirstOrDefault();
                        if (weakest == null)
                        {
                            // nothing safe to remove, break out
                            break;
                        }
                    }
                    else
                    {
                        weakest = Enemies.OrderBy(e => EstimateMobPower(e)).First();
                    }
                    currentPower -= EstimateMobPower(weakest);
                    Enemies.Remove(weakest);
                    Console.WriteLine($"   [Balance] Removed weakest: {weakest.Name} (power {EstimateMobPower(weakest)})");
                }
                else
                {
                    int idx = _rng.Next(Enemies.Count);
                    if (preserveFirstEnemy && idx == 0 && Enemies.Count > 1)
                    {
                        idx = 1; // avoid scaling down the preserved boss
                    }
                    var lvl = Math.Max(1, Enemies[idx].Level - 1);
                    var scaled = Enemies[idx].ScaleToLevel(lvl);
                    Console.WriteLine($"   [Balance] Scaled down: {Enemies[idx].Name} from L{Enemies[idx].Level} to L{lvl}");
                    currentPower -= EstimateMobPower(Enemies[idx]);
                    Enemies[idx] = scaled;
                    currentPower += EstimateMobPower(scaled);
                }
                attempts++;
            }
        }

        private string GenerateDescription()
        {
            if (Enemies.Count == 0) return "No enemies encountered.";

            var enemyNames = Enemies.Select(e => e.Name).ToList();
            var grouped = enemyNames.GroupBy(n => n);

            var descriptions = new List<string>();
            foreach (var group in grouped)
            {
                int count = group.Count();
                string name = group.Key;
                descriptions.Add(count > 1 ? $"{count} {name}s" : $"a {name}");
            }

            string enemyList = string.Join(" and ", descriptions);

            string[] intros = {
                $"You encounter {enemyList}!",
                $"A group of enemies appears: {enemyList}!",
                $"Watch out! {enemyList} block your path!",
                $"Battle! {enemyList} attack!",
                $"{enemyList} emerge from the shadows!"
            };

            return intros[_rng.Next(intros.Length)];
        }

        #endregion

        #region Combat Execution

        /// <summary>
        /// Start the encounter and manage multi-mob combat
        /// </summary>
        public bool StartEncounter(List<Character> party)
        {
            if (!IsActive)
            {
                Console.WriteLine("No active encounter.");
                return true;
            }

            // Track mob HP separately since Mob class doesn't have mutable HP
            var mobHealthTracker = new Dictionary<Mob, int>();
            foreach (var mob in Enemies)
            {
                mobHealthTracker[mob] = mob.Health;
            }

            // Display initial encounter state
            DisplayEncounterState(mobHealthTracker, party);

            // Combat loop
            while (mobHealthTracker.Values.Any(hp => hp > 0) && party.Any(p => p.IsAlive))
            {
                // Party turn - each member acts
                foreach (var member in party.Where(p => p.IsAlive))
                {
                    if (!mobHealthTracker.Values.Any(hp => hp > 0)) break;

                    // Display available targets
                    Console.WriteLine($"\n=== {member.Name}'s Turn ===");
                    var aliveEnemies = Enemies.Where(e => mobHealthTracker[e] > 0).ToList();

                    if (aliveEnemies.Count == 0) break;

                    Console.WriteLine("Available targets:");
                    for (int i = 0; i < aliveEnemies.Count; i++)
                    {
                        var enemy = aliveEnemies[i];
                        Console.WriteLine($"{i + 1}) {enemy.Name} (Lv {enemy.Level}) - HP: {mobHealthTracker[enemy]}/{enemy.Health}");
                    }

                    // For now, just run the basic combat against a randomly selected alive enemy
                    // This is a simple implementation - you could expand it to target selection
                    var targetMob = aliveEnemies[_rng.Next(aliveEnemies.Count)];
                    int currentHp = mobHealthTracker[targetMob];

                    // Use existing combat system for single mob
                    bool mobDefeated = CombatSystem.RunEncounter(new List<Character> { member }, targetMob);

                    // Update HP tracker (estimate based on defeat or assume some damage)
                    if (mobDefeated)
                    {
                        mobHealthTracker[targetMob] = 0;
                        Console.WriteLine($"✓ {targetMob.Name} defeated!");

                        // If a goblin was defeated, pause and clear the screen before continuing
                        try
                        {
                            if (targetMob.Name != null && targetMob.Name.IndexOf("goblin", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                Console.WriteLine();
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey(true);
                                Console.Clear();
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        // Assume some damage was done (this is simplified)
                        int estimatedDamage = member.GetTotalStrength() / 2;
                        mobHealthTracker[targetMob] = Math.Max(0, currentHp - estimatedDamage);
                    }
                }

                // Check if all enemies defeated
                if (!mobHealthTracker.Values.Any(hp => hp > 0))
                {
                    break;
                }

                // Enemies turn - target random alive party members
                var aliveParty = party.Where(p => p.IsAlive).ToList();
                if (aliveParty.Count == 0) break;

                Console.WriteLine("\n=== Enemy Turn ===");
                foreach (var enemy in Enemies.Where(e => mobHealthTracker[e] > 0))
                {
                    var target = aliveParty[_rng.Next(aliveParty.Count)];
                    CombatSystem.Attack(enemy, target);
                    Console.WriteLine();

                    if (!target.IsAlive)
                    {
                        Console.WriteLine($"💀 {target.Name} has fallen!");
                        aliveParty = party.Where(p => p.IsAlive).ToList();
                        if (aliveParty.Count == 0) break;
                    }
                }
            }

            bool partyWon = mobHealthTracker.Values.All(hp => hp <= 0);

            if (partyWon)
            {
                Console.WriteLine("\n🎉 Victory! All enemies defeated!");
                DistributeRewards(party);
            }
            else
            {
                Console.WriteLine("\n💀 The party was defeated...");
            }

            return partyWon;
        }

        #endregion

        #region Display Methods

        private void DisplayEncounterState(Dictionary<Mob, int> mobHealthTracker, List<Character> party)
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     COMBAT ENCOUNTER                          ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

            Console.WriteLine("\n🛡️  Your Party:");
            foreach (var member in party)
            {
                string status = member.IsAlive ? "✓" : "✗";
                Console.Write($"   {status} {member.Name} (Lv {member.Level}) - ");
                try
                {
                    VisualEffects.DrawHealthBarLine(member.Health, member.GetTotalMaxHP(), 20);
                }
                catch
                {
                    Console.WriteLine($"HP: {member.Health}/{member.GetTotalMaxHP()}");
                }
            }

            Console.WriteLine("\n⚔️  Enemies:");
            for (int i = 0; i < Enemies.Count; i++)
            {
                var enemy = Enemies[i];
                int hp = mobHealthTracker[enemy];
                string status = hp > 0 ? "⚡" : "💀";
                Console.Write($"   {status} {i + 1}. {enemy.Name} (Lv {enemy.Level}) - ");
                try
                {
                    // Force background-colored bar for goblins which may be white in some terminals
                    bool isGoblin = enemy.Name != null && enemy.Name.IndexOf("goblin", StringComparison.OrdinalIgnoreCase) >= 0;
                    VisualEffects.DrawHealthBarLine(hp, enemy.Health, 20, isGoblin);
                }
                catch
                {
                    Console.WriteLine($"HP: {hp}/{enemy.Health}");
                }
            }
            Console.WriteLine();
        }

        #endregion

        #region Reward Distribution

        private void DistributeRewards(List<Character> party)
        {
            int totalGold = 0;
            var allLoot = new List<Item>();

            // Collect loot from all enemies
            foreach (var enemy in Enemies)
            {
                var loot = enemy.DropLoot(_rng);
                totalGold += loot.Gold;
                allLoot.AddRange(loot.Items);

                // Award XP
                int xp = Playerleveling.CalculateXPReward(enemy, party.Count);
                foreach (var member in party.Where(p => p.IsAlive))
                {
                    int memberXP = xp;

                    // Apply pet XP boost
                    if (member.Pet != null && member.Pet.Ability == PetAbility.ExperienceBoost)
                    {
                        memberXP = (int)(memberXP * 1.10);
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

                    member.GainExperience(memberXP);
                    Console.WriteLine($"   {member.Name} gains {memberXP} XP!");

                    // Pet gains experience
                    if (member.Pet != null)
                    {
                        member.Pet.GainExperience(xp / 4);
                    }

                    // Apply pet post-combat effects
                    if (member.Pet != null)
                    {
                        if (member.Pet.Ability == PetAbility.HealthRegen)
                        {
                            int healAmount = (int)(member.MaxHealth * 0.05);
                            member.Heal(healAmount);
                            Console.WriteLine($"   🐾 {member.Pet.Name} helps {member.Name} recover {healAmount} HP!");
                        }
                        else if (member.Pet.Ability == PetAbility.ManaRegen)
                        {
                            if (member is Warrior || member is Rogue)
                            {
                                int staminaAmount = (int)(member.MaxStamina * 0.05);
                                member.RestoreStamina(staminaAmount);
                                Console.WriteLine($"   🐾 {member.Pet.Name} helps {member.Name} recover {staminaAmount} Stamina!");
                            }
                            else if (member is Mage || member is Priest)
                            {
                                int manaAmount = (int)(member.MaxMana * 0.05);
                                member.RestoreMana(manaAmount);
                                Console.WriteLine($"   🐾 {member.Pet.Name} helps {member.Name} recover {manaAmount} Mana!");
                            }
                        }
                    }
                }
            }

            // Distribute gold
            if (totalGold > 0)
            {
                var bonusReceiver = party.FirstOrDefault(p => p.IsAlive) ?? party[0];

                // Apply pet loot bonus
                if (bonusReceiver.Pet != null && bonusReceiver.Pet.Ability == PetAbility.LootBonus)
                {
                    totalGold = (int)(totalGold * 1.20);
                }

                // Apply skill gold bonus
                if (bonusReceiver.SkillTree != null)
                {
                    double goldBonusPercent = bonusReceiver.SkillTree.GetTotalGoldBonusPercent();
                    if (goldBonusPercent > 0)
                    {
                        totalGold = (int)(totalGold * (1.0 + goldBonusPercent));
                    }
                }

                int goldPerMember = totalGold / party.Count;
                int remainder = totalGold % party.Count;

                foreach (var member in party)
                {
                    int memberGold = goldPerMember;
                    if (member == party[0] && remainder > 0)
                    {
                        memberGold += remainder;
                    }
                    member.Inventory.AddGold(memberGold);
                }

                Console.WriteLine($"   💰 The party receives {totalGold} gold ({goldPerMember} per member)!");
            }

            // Distribute items
            if (allLoot.Count > 0)
            {
                Console.WriteLine($"   🎁 Found {allLoot.Count} item(s):");
                int itemIndex = 0;
                foreach (var item in allLoot)
                {
                    bool itemAdded = false;
                    for (int attempt = 0; attempt < party.Count && !itemAdded; attempt++)
                    {
                        var receiver = party[(itemIndex + attempt) % party.Count];
                        if (receiver.Inventory.AddItem(item))
                        {
                            Console.WriteLine($"      → {receiver.Name} receives {item.Name}");
                            itemAdded = true;
                        }
                    }

                    if (!itemAdded)
                    {
                        Console.WriteLine($"      → {item.Name} left behind (no inventory space)");
                    }

                    itemIndex++;
                }
            }
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Quick method to generate and start an encounter in one call
        /// </summary>
        public static bool QuickEncounter(List<Character> party, EncounterDifficulty difficulty = EncounterDifficulty.Normal)
        {
            var encounter = new Encounter();
            encounter.GenerateEncounter(party, difficulty);
            return encounter.StartEncounter(party);
        }

        /// <summary>
        /// Generate a random encounter based on area level
        /// </summary>
        public static bool RandomEncounter(List<Character> party, int areaLevel)
        {
            var rng = new Random();
            var difficulty = EncounterDifficulty.Normal;

            // 60% normal, 25% easy, 15% hard
            int roll = rng.Next(100);
            if (roll < 25)
                difficulty = EncounterDifficulty.Easy;
            else if (roll >= 85)
                difficulty = EncounterDifficulty.Hard;

            var encounter = new Encounter();
            encounter.GenerateEncounter(party, difficulty);
            return encounter.StartEncounter(party);
        }

        #endregion
    }

    #endregion
}
