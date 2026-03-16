using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class TrainingHall
    {
        #region Constants

        // Base costs for training
        private const int BaseTrainingCost = 50;
        private const double ChampionTrainingMultiplier = 2.5;
        private const double LevelCostMultiplier = 1.15; // 15% increase per level

        #endregion

        #region Public Methods

        public void EnterTrainingHall(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           TRAINING HALL - Academy of Mastery             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine("⚔️ Master trainers stand ready to teach their arts.");
            Console.WriteLine("🏆 Specialized champion trainers are available for advanced students.");
            Console.WriteLine();
            Console.WriteLine("💡 Training grants experience to help you level up faster!");
            Console.WriteLine("💰 Cost increases with your level and specialization.");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("\n--- Training Hall Menu ---");
                Console.WriteLine("1) View Available Trainers");
                Console.WriteLine("2) Train a Character");
                Console.WriteLine("3) View Training Costs");
                Console.WriteLine("4) 🌟 Mythic Title Ascension (Lv 50)");
                Console.WriteLine("0) Leave Training Hall");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        DisplayAvailableTrainers(party);
                        break;
                    case "2":
                        TrainCharacter(party);
                        break;
                    case "3":
                        DisplayTrainingCosts(party);
                        break;
                    case "4":
                        SelectMythicTitle(party);
                        break;
                    case "0":
                        Console.WriteLine("\n'Train hard, fight harder!' - The trainers salute you.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private void DisplayAvailableTrainers(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  AVAILABLE TRAINERS                        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("🗡️  BASE CLASS TRAINERS:");
            Console.WriteLine("   ⚔️  Master Thorin - Warrior Trainer");
            Console.WriteLine("      'Strength and honor guide the warrior's path!'");
            Console.WriteLine();
            Console.WriteLine("   🔮 Archmage Elara - Mage Trainer");
            Console.WriteLine("      'Master the arcane, bend reality to your will!'");
            Console.WriteLine();
            Console.WriteLine("   🗡️  Shadow Master Kael - Rogue Trainer");
            Console.WriteLine("      'Strike from the shadows, never be seen!'");
            Console.WriteLine();
            Console.WriteLine("   ✨ High Priestess Lyra - Priest Trainer");
            Console.WriteLine("      'Faith and healing are your greatest weapons!'");
            Console.WriteLine();

            Console.WriteLine("🏆 CHAMPION CLASS TRAINERS:");
            Console.WriteLine();
            Console.WriteLine("   ⚔️✨ Sir Galahad - Paladin Trainer");
            Console.WriteLine("   💢🔥 Warlord Grom - Berserker Trainer");
            Console.WriteLine("   🛡️🏰 Commander Stone - Guardian Trainer");
            Console.WriteLine();
            Console.WriteLine("   🔮⚡ Grand Wizard Merlin - Archmage Trainer");
            Console.WriteLine("   💀🌑 Lich King Mortis - Necromancer Trainer");
            Console.WriteLine("   🔥❄️⚡ Sage of Elements - Elementalist Trainer");
            Console.WriteLine();
            Console.WriteLine("   💀⚔️ Master of Blades - Assassin Trainer");
            Console.WriteLine("   🏹🎯 Ranger General - Ranger Trainer");
            Console.WriteLine("   🌑⚔️ Shadow Lord - Shadowblade Trainer");
            Console.WriteLine();
            Console.WriteLine("   ⚔️✨ Grand Templar - Templar Trainer");
            Console.WriteLine("   🌿🐻 Archdruid Malfurion - Druid Trainer");
            Console.WriteLine("   🔮✨ Prophet Velen - Oracle Trainer");

            Console.WriteLine("\n💡 Note: Champion trainers are only available to those who have ascended!");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private void PerformTraining(Character character)
        {
            int trainingCost = CalculateTrainingCost(character);
            int xpReward = CalculateTrainingXP(character);

            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     TRAINING SESSION                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            DisplayTrainerInfo(character);

            Console.WriteLine($"Character: {character.Name}");
            Console.WriteLine($"Class: {character.GetType().Name}");
            if (character.HasChampionClass)
            {
                Console.WriteLine($"Champion Class: 🏆 {character.ChampionClass}");
            }
            if (character.HasMythicTitle)
            {
                Console.WriteLine($"Mythic Title: 🌟 \"{character.MythicTitle}\"");
            }
            Console.WriteLine($"Current Level: {character.Level}");
            Console.WriteLine($"Current XP: {character.Experience}/{character.ExperienceToNextLevel}");
            Console.WriteLine();
            Console.WriteLine($"💰 Training Cost: {trainingCost} gold");
            Console.WriteLine($"⭐ Experience Gain: {xpReward} XP");
            Console.WriteLine();

            if (character.Inventory.Gold < trainingCost)
            {
                Console.WriteLine($"❌ Not enough gold! You need {trainingCost} gold but only have {character.Inventory.Gold}.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("Proceed with training? (y/n): ");
            var confirm = Console.ReadLine() ?? string.Empty;

            if (!confirm.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\nTraining cancelled.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
                return;
            }

            // Deduct gold
            character.Inventory.SpendGold(trainingCost);

            // Display training sequence
            Console.WriteLine();
            Console.WriteLine("════════════════════════════════════════════════════════════");
            DisplayTrainingSequence(character);
            Console.WriteLine("════════════════════════════════════════════════════════════");
            Console.WriteLine();

            // Award experience
            int oldLevel = character.Level;
            character.GainExperience(xpReward);
            int newLevel = character.Level;

            if (newLevel > oldLevel)
            {
                Console.WriteLine($"\n🎉 Training was so effective that {character.Name} gained {newLevel - oldLevel} level(s)!");
            }

            Console.WriteLine($"\n✅ Training complete! {character.Name} gained {xpReward} experience.");
            Console.WriteLine($"💰 Paid: {trainingCost} gold (Remaining: {character.Inventory.Gold})");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private void DisplayTrainerInfo(Character character)
        {
            if (character.HasChampionClass)
            {
                string trainerName = GetChampionTrainerName(character.ChampionClass!);
                string trainerQuote = GetChampionTrainerQuote(character.ChampionClass!);
                Console.WriteLine($"🏆 Champion Trainer: {trainerName}");
                Console.WriteLine($"   '{trainerQuote}'");
            }
            else
            {
                string baseClass = character.GetType().Name;
                string trainerName = GetBaseTrainerName(baseClass);
                string trainerQuote = GetBaseTrainerQuote(baseClass);
                Console.WriteLine($"⚔️ Class Trainer: {trainerName}");
                Console.WriteLine($"   '{trainerQuote}'");
            }
            Console.WriteLine();
        }

        private void DisplayTrainingSequence(Character character)
        {
            if (character.HasChampionClass)
            {
                DisplayChampionTraining(character);
            }
            else
            {
                DisplayBaseClassTraining(character);
            }
        }

        private void DisplayBaseClassTraining(Character character)
        {
            string baseClass = character.GetType().Name;

            switch (baseClass)
            {
                case "Warrior":
                    Console.WriteLine("⚔️ You practice sword techniques with the training dummy...");
                    Console.WriteLine("🛡️ You learn advanced blocking and parrying maneuvers...");
                    Console.WriteLine("💪 Your strength and combat prowess improve!");
                    break;
                case "Mage":
                    Console.WriteLine("🔮 You study ancient tomes and practice spell formations...");
                    Console.WriteLine("⚡ You channel mana through intricate patterns...");
                    Console.WriteLine("🧠 Your magical knowledge deepens!");
                    break;
                case "Rogue":
                    Console.WriteLine("🗡️ You practice precise strikes on target dummies...");
                    Console.WriteLine("👤 You learn advanced stealth and evasion techniques...");
                    Console.WriteLine("⚡ Your speed and agility increase!");
                    break;
                case "Priest":
                    Console.WriteLine("✨ You meditate and commune with divine forces...");
                    Console.WriteLine("🙏 You practice healing spells and prayers...");
                    Console.WriteLine("💫 Your faith and spiritual power grow!");
                    break;
                default:
                    Console.WriteLine("📚 You undergo rigorous training exercises...");
                    Console.WriteLine("💪 Your skills and abilities improve!");
                    break;
            }
        }

        private void DisplayChampionTraining(Character character)
        {
            string championClass = character.ChampionClass ?? "";

            switch (championClass)
            {
                case "Paladin":
                    Console.WriteLine("⚔️✨ You train with holy combat techniques...");
                    Console.WriteLine("🛡️ You practice channeling divine power through your strikes...");
                    Console.WriteLine("✨ Your righteousness and holy might grow stronger!");
                    break;
                case "Berserker":
                    Console.WriteLine("💢 You enter a controlled rage, pushing your limits...");
                    Console.WriteLine("🔥 You practice devastating power attacks...");
                    Console.WriteLine("💪 Your raw fury becomes even more destructive!");
                    break;
                case "Guardian":
                    Console.WriteLine("🛡️ You practice advanced defensive formations...");
                    Console.WriteLine("🏰 You train to withstand massive damage...");
                    Console.WriteLine("🗿 You become an even more impenetrable fortress!");
                    break;
                case "Archmage":
                    Console.WriteLine("🔮 You study the most powerful arcane secrets...");
                    Console.WriteLine("⚡ You practice world-shattering spells...");
                    Console.WriteLine("🌟 Your mastery over magic reaches new heights!");
                    break;
                case "Necromancer":
                    Console.WriteLine("💀 You commune with the forces of death...");
                    Console.WriteLine("🌑 You practice dark rituals and life drain...");
                    Console.WriteLine("👻 Your command over undeath strengthens!");
                    break;
                case "Elementalist":
                    Console.WriteLine("🔥❄️⚡ You attune yourself to all elements...");
                    Console.WriteLine("🌍 You practice switching between elemental forms...");
                    Console.WriteLine("🌟 Your elemental mastery becomes second nature!");
                    break;
                case "Assassin":
                    Console.WriteLine("💀 You practice lethal finishing techniques...");
                    Console.WriteLine("⚔️ You hone your combo execution to perfection...");
                    Console.WriteLine("🗡️ Your strikes become even deadlier!");
                    break;
                case "Ranger":
                    Console.WriteLine("🏹 You practice precision archery techniques...");
                    Console.WriteLine("🎯 You train your focus to lethal precision...");
                    Console.WriteLine("🦅 Your aim becomes truly deadly!");
                    break;
                case "Shadowblade":
                    Console.WriteLine("🌑 You train in the ways of shadow manipulation...");
                    Console.WriteLine("⚔️ You harness darkness for devastating attacks...");
                    Console.WriteLine("👤 You become one with the shadows!");
                    break;
                case "Templar":
                    Console.WriteLine("⚔️✨ You practice balancing martial and divine arts...");
                    Console.WriteLine("🛡️ You train in righteous combat techniques...");
                    Console.WriteLine("💫 Your battle prowess and faith grow together!");
                    break;
                case "Druid":
                    Console.WriteLine("🌿 You commune with nature's primal forces...");
                    Console.WriteLine("🐻 You practice shapeshifting forms...");
                    Console.WriteLine("🌳 Your connection with nature deepens!");
                    break;
                case "Oracle":
                    Console.WriteLine("🔮 You meditate on visions of the future...");
                    Console.WriteLine("✨ You practice divine foresight...");
                    Console.WriteLine("👁️ Your prophetic powers strengthen!");
                    break;
                default:
                    Console.WriteLine("📚 You undergo specialized training...");
                    Console.WriteLine("💪 Your abilities improve significantly!");
                    break;
            }
        }

        private void TrainCharacter(List<Character> party)
        {
            if (party == null || party.Count == 0)
            {
                Console.WriteLine("No party members available!");
                return;
            }

            Console.WriteLine("\n--- Select Character to Train ---");
            for (int i = 0; i < party.Count; i++)
            {
                var character = party[i];
                string championInfo = character.HasChampionClass ? $" [🏆{character.ChampionClass}]" : "";
                int cost = CalculateTrainingCost(character);
                int xp = CalculateTrainingXP(character);

                Console.WriteLine($"{i + 1}) {character.Name} - Lv.{character.Level} {character.GetType().Name}{championInfo}");
                Console.WriteLine($"    💰 Cost: {cost} gold | ⭐ XP Gain: {xp} | Gold: {character.Inventory.Gold}");
            }
            Console.WriteLine($"{party.Count + 1}) Cancel");
            Console.Write("\nChoice: ");

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > party.Count + 1)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            if (choice == party.Count + 1)
                return;

            var selectedCharacter = party[choice - 1];

            if (selectedCharacter.Level >= Playerleveling.MaxLevel)
            {
                Console.WriteLine($"\n❌ {selectedCharacter.Name} is already at maximum level!");
                Console.WriteLine("There is nothing more to learn through training.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
                return;
            }

            PerformTraining(selectedCharacter);
        }

        private void DisplayTrainingCosts(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    TRAINING COSTS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("💡 Training costs are calculated based on:");
            Console.WriteLine("   - Base Cost: 50 gold");
            Console.WriteLine("   - Level Multiplier: +15% per level");
            Console.WriteLine("   - Champion Class: 2.5x cost multiplier");
            Console.WriteLine();
            Console.WriteLine("📊 Cost Formula:");
            Console.WriteLine("   Base Classes: 50 × (1.15 ^ Level)");
            Console.WriteLine("   Champion Classes: 50 × (1.15 ^ Level) × 2.5");
            Console.WriteLine();

            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║            YOUR PARTY'S TRAINING COSTS                    ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");

            foreach (var character in party)
            {
                int cost = CalculateTrainingCost(character);
                int xp = CalculateTrainingXP(character);
                string championMark = character.HasChampionClass ? " 🏆" : "";

                Console.WriteLine();
                Console.WriteLine($"📜 {character.Name}{championMark}");
                Console.WriteLine($"   Class: {character.GetType().Name}");
                if (character.HasChampionClass)
                {
                    Console.WriteLine($"   Champion: {character.ChampionClass}");
                }
                Console.WriteLine($"   Level: {character.Level}");
                Console.WriteLine($"   Training Cost: {cost} gold");
                Console.WriteLine($"   XP Reward: {xp}");
                Console.WriteLine($"   Current Gold: {character.Inventory.Gold}");
                Console.WriteLine($"   Can Afford: {(character.Inventory.Gold >= cost ? "✅ Yes" : "❌ No")}");
            }

            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  EXAMPLE COSTS BY LEVEL                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Level │ Base Class │ Champion Class");
            Console.WriteLine("──────┼────────────┼───────────────");

            int[] exampleLevels = { 1, 5, 10, 15, 20, 25, 30, 40, 50, 75, 100 };
            foreach (int level in exampleLevels)
            {
                int baseCost = CalculateCostForLevel(level, false);
                int championCost = CalculateCostForLevel(level, true);
                Console.WriteLine($"{level,5} │ {baseCost,9}g │ {championCost,12}g");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private int CalculateTrainingCost(Character character)
        {
            int level = character.Level;
            bool isChampion = character.HasChampionClass;

            return CalculateCostForLevel(level, isChampion);
        }

        private int CalculateCostForLevel(int level, bool isChampion)
        {
            // Base cost increases exponentially with level
            double cost = BaseTrainingCost * Math.Pow(LevelCostMultiplier, level);

            // Champion classes cost significantly more
            if (isChampion)
            {
                cost *= ChampionTrainingMultiplier;
            }

            return (int)Math.Round(cost);
        }

        private int CalculateTrainingXP(Character character)
        {
            // Training gives about 30-40% of the XP needed for next level
            // This makes training useful but not a replacement for actual adventuring
            int xpToNext = character.ExperienceToNextLevel;
            double xpPercent = 0.35; // 35% of next level

            // Champion classes get slightly more XP from training (40%)
            if (character.HasChampionClass)
            {
                xpPercent = 0.40;
            }

            int xpReward = (int)(xpToNext * xpPercent);
            return Math.Max(10, xpReward); // Minimum 10 XP
        }

        private string GetBaseTrainerName(string baseClass)
        {
            return baseClass switch
            {
                "Warrior" => "Master Thorin",
                "Mage" => "Archmage Elara",
                "Rogue" => "Shadow Master Kael",
                "Priest" => "High Priestess Lyra",
                _ => "Master Trainer"
            };
        }

        private string GetBaseTrainerQuote(string baseClass)
        {
            return baseClass switch
            {
                "Warrior" => "Every scar is a lesson. Every battle, a teacher.",
                "Mage" => "Magic is not just power—it is understanding.",
                "Rogue" => "The unseen blade is the deadliest.",
                "Priest" => "True strength comes from faith and compassion.",
                _ => "Train hard, fight harder."
            };
        }

        private string GetChampionTrainerName(string championClass)
        {
            return championClass switch
            {
                "Paladin" => "Sir Galahad the Righteous",
                "Berserker" => "Warlord Grom Hellscream",
                "Guardian" => "Commander Stone the Unbreakable",
                "Archmage" => "Grand Wizard Merlin",
                "Necromancer" => "Lich King Mortis",
                "Elementalist" => "Sage of Four Elements",
                "Assassin" => "Master of a Thousand Blades",
                "Ranger" => "Ranger General Hawkeye",
                "Shadowblade" => "Shadow Lord Erebus",
                "Templar" => "Grand Templar Uther",
                "Druid" => "Archdruid Malfurion",
                "Oracle" => "Prophet Velen the Seer",
                _ => "Champion Master"
            };
        }

        private string GetChampionTrainerQuote(string championClass)
        {
            return championClass switch
            {
                "Paladin" => "The Light shall guide your blade to righteousness.",
                "Berserker" => "Embrace your rage, let it consume your enemies!",
                "Guardian" => "An immovable object defeats any unstoppable force.",
                "Archmage" => "Reality bends to those who master its secrets.",
                "Necromancer" => "Death is not the end—it is a beginning.",
                "Elementalist" => "The elements obey those who understand their nature.",
                "Assassin" => "One perfect strike ends all battles.",
                "Ranger" => "Patience and precision—the hunter's creed.",
                "Shadowblade" => "Darkness is not absence of light, but a weapon itself.",
                "Templar" => "Faith and steel—united they are unbreakable.",
                "Druid" => "Nature's balance is maintained through strength.",
                "Oracle" => "To see the future is to shape it.",
                _ => "Master your champion powers."
            };
        }

        private void SelectMythicTitle(List<Character> party)
        {
            if (party == null || party.Count == 0)
            {
                Console.WriteLine("No party members available!");
                return;
            }

            // Find eligible characters
            var eligible = new List<(int index, Character character)>();
            for (int i = 0; i < party.Count; i++)
            {
                if (MythicTitleManager.CanSelectMythicTitle(party[i]))
                {
                    eligible.Add((i, party[i]));
                }
            }

            if (eligible.Count == 0)
            {
                Console.WriteLine("\n🌟 Mythic Title Ascension");
                Console.WriteLine("   No party members are eligible for a Mythic Title.");
                Console.WriteLine("   Requirements: Level 50+ and no existing Mythic Title.");
                Console.WriteLine();

                // Show current mythic titles if any
                foreach (var member in party)
                {
                    if (member.HasMythicTitle)
                    {
                        Console.WriteLine($"   ✦ {member.Name} — \"{member.MythicTitle}\"");
                    }
                    else if (member.Level < 50)
                    {
                        Console.WriteLine($"   ○ {member.Name} — Level {member.Level}/50");
                    }
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\n🌟 Select a character for Mythic Title Ascension:");
            for (int i = 0; i < eligible.Count; i++)
            {
                var (_, character) = eligible[i];
                string championInfo = character.HasChampionClass ? $" [🏆{character.ChampionClass}]" : "";
                Console.WriteLine($"  [{i + 1}] {character.Name} - Lv.{character.Level} {character.GetType().Name}{championInfo}");
            }
            Console.WriteLine($"  [0] Cancel");
            Console.Write("Choice: ");

            var input = Console.ReadLine()?.Trim() ?? "";
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= eligible.Count)
            {
                MythicTitleManager.ShowMythicTitleSelection(eligible[choice - 1].character);
            }
        }

        #endregion
    }
}
