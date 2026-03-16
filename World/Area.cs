using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Area Class

    internal class Area
    {
        #region Properties

        public string Name { get; }
        public string Description { get; }
        public int RecommendedLevel { get; }
        public bool IsUnlocked { get; set; }
        public List<DungeonLocation> Dungeons { get; }
        public List<QuestSpot> QuestSpots { get; }
        public List<string> EnemyCampNames { get; }
        public Weather? Weather { get; private set; }
        public TimeOfDay? TimeTracker { get; private set; }

        #endregion

        #region Constructor

        public Area(string name, string description, int recommendedLevel, bool isUnlocked = false)
        {
            Name = name;
            Description = description;
            RecommendedLevel = recommendedLevel;
            IsUnlocked = isUnlocked;
            Dungeons = new List<DungeonLocation>();
            QuestSpots = new List<QuestSpot>();
            EnemyCampNames = new List<string>();
        }

        #endregion

        #region Methods

        public void SetWeather(Weather weather)
        {
            Weather = weather;
        }

        public void SetTimeTracker(TimeOfDay timeTracker)
        {
            TimeTracker = timeTracker;
        }

        public void DisplayAreaInfo()
        {
            Console.WriteLine($"\n╔════════════════════════════════════════╗");
            Console.WriteLine($"║  {Name,-38}║");
            Console.WriteLine($"╚════════════════════════════════════════╝");
            Console.WriteLine($"📍 {Description}");
            Console.WriteLine($"⚔️  Recommended Level: {RecommendedLevel}");

            if (Weather != null)
            {
                Console.WriteLine($"🌤️  Weather: {Weather.GetWeatherDescription()}");
            }

            if (Dungeons.Count > 0)
            {
                Console.WriteLine($"\n🏰 Dungeons: {Dungeons.Count}");
                foreach (var dungeon in Dungeons)
                {
                    string completedStatus = dungeon.IsCompleted ? "✓" : "○";
                    Console.WriteLine($"   {completedStatus} {dungeon.Name} (Lv {dungeon.RecommendedLevel}, {dungeon.Floors} floors)");
                }
            }

            if (QuestSpots.Count > 0)
            {
                Console.WriteLine($"\n📜 Quest Spots: {QuestSpots.Count}");
                foreach (var spot in QuestSpots)
                {
                    Console.WriteLine($"   • {spot.Name}");
                }
            }
        }

        public void ExploreArea(List<Character> party, QuestBoard questBoard, BountyBoard bountyBoard,
                              AchievementTracker achievementTracker, Journal journal)
        {
            if (!IsUnlocked)
            {
                Console.WriteLine($"The {Name} is still locked. Complete quests or level up to unlock it!");
                return;
            }

            while (true)
            {
                DisplayAreaInfo();
                Console.WriteLine("\n=== Area Menu ===");
                Console.WriteLine("1) Enter a Dungeon");
                Console.WriteLine("2) Visit Quest Spot");
                Console.WriteLine("3) Rest at Camp");
                Console.WriteLine("0) Return to Map");
                Console.Write("Choose: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        SelectAndExploreDungeon(party, questBoard, bountyBoard, achievementTracker, journal);
                        break;
                    case "2":
                        VisitQuestSpot(party, questBoard, journal);
                        break;
                    case "3":
                        var result = Camping.CampMenu(party, Weather, TimeTracker);
                        if (result != null)
                        {
                            Console.WriteLine("Game loaded from camp. Returning to main menu...");
                            return;
                        }
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private void SelectAndExploreDungeon(List<Character> party, QuestBoard questBoard,
                                            BountyBoard bountyBoard, AchievementTracker achievementTracker, Journal journal)
        {
            if (Dungeons.Count == 0)
            {
                Console.WriteLine("No dungeons available in this area.");
                return;
            }

            Console.WriteLine("\n=== Select a Dungeon ===");
            for (int i = 0; i < Dungeons.Count; i++)
            {
                var d = Dungeons[i];
                string status = d.IsCompleted ? "[✓ Completed]" : "[○ Active]";
                Console.WriteLine($"{i + 1}) {d.Name} {status} - Lv {d.RecommendedLevel}, {d.Floors} floors");
            }
            Console.Write("\nChoose dungeon (or 0 to cancel): ");

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > Dungeons.Count)
            {
                return;
            }

            var selectedDungeon = Dungeons[idx - 1];

            // Show power vs dungeon chance and offer simulation before entering
            bool proceed = false;
            while (true)
            {
                try
                {
                    // Print computed chance and a short simulation
                    int chance = PowerRating.ChanceToComplete(party, selectedDungeon);
                    Console.WriteLine();
                    Console.WriteLine($"Computed chance to complete this dungeon: {chance}%");

                    // Run a 5-attempt simulation (deterministic per session)
                    var simRng = new Random();
                    PowerRating.SimulateDungeonAttempts(party, selectedDungeon, 5, simRng);

                    if (chance < 35)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("⚠️  Warning: This dungeon looks especially dangerous for your current party.");
                        Console.ResetColor();
                    }
                    else if (chance < 60)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️  Caution: This dungeon may be challenging. Consider resting or gearing up.");
                        Console.ResetColor();
                    }
                    // Recommended actions
                    var recs = PowerRating.RecommendActions(party, selectedDungeon);
                    if (recs != null && recs.Count > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Recommended actions:");
                        foreach (var r in recs)
                        {
                            Console.WriteLine($"  - {r}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PowerRating] Could not compute chance/simulation: {ex.Message}");
                }

                Console.WriteLine("\nDo you want to enter this dungeon? (y = enter / s = simulate again / n = cancel)");
                var choice = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
                if (choice == "y") { proceed = true; break; }
                if (choice == "n") { proceed = false; break; }
                if (choice == "s") { continue; }
                Console.WriteLine("Invalid option. Enter 'y', 'n' or 's'.");
            }

            if (!proceed)
            {
                Console.WriteLine("Cancelled. Returning to area menu...");
                return;
            }

            // Travel to dungeon
            if (Weather != null && TimeTracker != null)
            {
                TravelManager.TravelToDungeon(party, Weather, TimeTracker, selectedDungeon.RecommendedLevel);
            }

            Console.WriteLine($"\n🏰 Entering {selectedDungeon.Name}...");
            var dungeon = new Dungeon(selectedDungeon.Floors, selectedDungeon.Seed);
            dungeon.Explore(party, journal, questBoard, bountyBoard, achievementTracker);

            // Mark dungeon as completed
            selectedDungeon.IsCompleted = true;

            // Advance time after dungeon
            var rng = new Random();
            int dungeonHours = rng.Next(2, 5);
            TimeTracker?.AdvanceTime(dungeonHours);

            // Return journey
            if (Weather != null && TimeTracker != null)
            {
                Console.WriteLine("\n🏠 Your party begins the journey back...");
                TravelManager.ReturnToTown(party, Weather, TimeTracker);
            }
        }

        private void VisitQuestSpot(List<Character> party, QuestBoard questBoard, Journal journal)
        {
            if (QuestSpots.Count == 0)
            {
                Console.WriteLine("No quest spots available in this area.");
                return;
            }

            Console.WriteLine("\n=== Quest Spots ===");
            for (int i = 0; i < QuestSpots.Count; i++)
            {
                var q = QuestSpots[i];
                Console.WriteLine($"{i + 1}) {q.Name} - {q.Description}");
            }
            Console.Write("\nVisit which quest spot (or 0 to cancel): ");

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > QuestSpots.Count)
            {
                return;
            }

            var spot = QuestSpots[idx - 1];
            spot.Interact(party, questBoard, journal);

            // Advance time (quest spots take 30 minutes to 1 hour)
            var rng = new Random();
            int minutes = rng.Next(30, 61); // 30 to 60 minutes
            double hours = minutes / 60.0;
            TimeTracker?.AdvanceTime((int)Math.Ceiling(hours));
        }

        #endregion
    }

    #endregion

    #region DungeonLocation Class

    internal class DungeonLocation
    {
        public string Name { get; }
        public int RecommendedLevel { get; }
        public int Floors { get; }
        public bool IsCompleted { get; set; }
        public int Seed { get; }

        public DungeonLocation(string name, int recommendedLevel, int floors, int? seed = null)
        {
            Name = name;
            RecommendedLevel = recommendedLevel;
            Floors = floors;
            IsCompleted = false;
            Seed = seed ?? (name.GetHashCode() ^ recommendedLevel);
        }
    }

    #endregion

    #region QuestSpot Class

    internal class QuestSpot
    {
        public string Name { get; }
        public string Description { get; }
        public string QuestObjective { get; }

        public QuestSpot(string name, string description, string questObjective)
        {
            Name = name;
            Description = description;
            QuestObjective = questObjective;
        }

        public void Interact(List<Character> party, QuestBoard questBoard, Journal journal)
        {
            Console.WriteLine($"\n--- {Name} ---");
            Console.WriteLine(Description);
            Console.WriteLine("\nYou explore the area and make progress on relevant quests...");

            // Update any active quests that match this spot's objective
            questBoard?.UpdateQuestProgressInJournal(journal, QuestObjective, 1);

            // Random event chance
            var rng = new Random();
            if (rng.Next(100) < 30)
            {
                Console.WriteLine("\n✨ You discovered something!");
                var goldFound = rng.Next(10, 50);
                party[0].Inventory.AddGold(goldFound);
                Console.WriteLine($"Found {goldFound} gold!");
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }

    #endregion
}
