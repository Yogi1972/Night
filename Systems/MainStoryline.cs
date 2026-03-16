using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Story Chapter Class

    internal class StoryChapter
    {
        public int ChapterNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ObjectiveDescription { get; set; }
        public bool IsCompleted { get; set; }
        public string RequiredLocation { get; set; }
        public int RequiredLevel { get; set; }
        public string RewardDescription { get; set; }

        public StoryChapter(int chapter, string title, string description, string objective, string requiredLocation, int requiredLevel, string reward)
        {
            ChapterNumber = chapter;
            Title = title;
            Description = description;
            ObjectiveDescription = objective;
            IsCompleted = false;
            RequiredLocation = requiredLocation;
            RequiredLevel = requiredLevel;
            RewardDescription = reward;
        }
    }

    #endregion

    #region Main Storyline Manager

    internal class MainStoryline
    {
        #region Fields

        private readonly List<StoryChapter> _chapters;
        public int CurrentChapter { get; private set; }
        private bool _hasSeenIntro;

        #endregion

        #region Constructor

        public MainStoryline()
        {
            _chapters = new List<StoryChapter>();
            CurrentChapter = 1;
            _hasSeenIntro = false;
            InitializeStoryline();
        }

        #endregion

        #region Initialization

        private void InitializeStoryline()
        {
            // Chapter 1: The Awakening (Havenbrook)
            _chapters.Add(new StoryChapter(
                1,
                "The Awakening",
                "Dark forces are stirring across the realm. Elder Morris in Havenbrook has called for brave adventurers to investigate strange occurrences.",
                "Speak with Elder Morris in Havenbrook",
                "Havenbrook",
                1,
                "Access to new information and 100 gold"
            ));

            // Chapter 2: Forging Alliances (Ironforge Citadel)
            _chapters.Add(new StoryChapter(
                2,
                "Forging Alliances",
                "Master Thorgrim of Ironforge has noticed unusual activity in the mountain mines. The dwarves request aid from skilled adventurers.",
                "Travel to Ironforge Citadel and meet Master Thorgrim",
                "Ironforge Citadel",
                10,
                "Special crafted weapon and 250 gold"
            ));

            // Chapter 3: The Arcane Mystery (Mysthaven)
            _chapters.Add(new StoryChapter(
                3,
                "The Arcane Mystery",
                "Archmage Elara has discovered concerning magical disturbances. The mages of Mysthaven believe it's connected to the growing darkness.",
                "Journey to Mysthaven and consult with Archmage Elara",
                "Mysthaven",
                15,
                "Enchanted accessory and 400 gold"
            ));

            // Chapter 4: Desert Secrets (Sunspire)
            _chapters.Add(new StoryChapter(
                4,
                "Desert Secrets",
                "Ancient tombs in Sunspire hold clues about the dark force. Prince Rashid requests your presence to uncover these secrets.",
                "Reach Sunspire and speak with Prince Rashid",
                "Sunspire",
                20,
                "Ancient artifact and 600 gold"
            ));

            // Chapter 5: Confronting the Shadow (Shadowkeep)
            _chapters.Add(new StoryChapter(
                5,
                "Confronting the Shadow",
                "All paths lead to Shadowkeep. Lord Malachar claims to know the truth about the darkness threatening the realm.",
                "Brave the journey to Shadowkeep and meet Lord Malachar",
                "Shadowkeep",
                25,
                "Legendary equipment and 1000 gold"
            ));

            // Chapter 6: The Final Convergence
            _chapters.Add(new StoryChapter(
                6,
                "The Final Convergence",
                "With knowledge from all corners of the realm, you must now face the source of darkness and save the world.",
                "Defeat the source of darkness in the Corrupted Lands",
                "Blightlands",
                30,
                "Ultimate reward and realm salvation"
            ));
        }

        #endregion

        #region Story Progress

        public void ShowIntroduction()
        {
            if (_hasSeenIntro) return;

            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     ✦  A WHISPER IN THE STORM  ✦                  ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // Short cinematic intro with pacing
            Console.WriteLine("Night falls early this season. A cold wind walks the empty streets, carrying the scent of smoke and old magic.");
            System.Threading.Thread.Sleep(700);
            Console.WriteLine("Farmers whisper of fields gone black at dawn. Soldiers return with empty harvests and haunted eyes.");
            System.Threading.Thread.Sleep(700);
            Console.WriteLine("Caravans pass with fewer wagons; priests speak in hushed tones of a darkness that devours light.");
            System.Threading.Thread.Sleep(900);

            Console.WriteLine();
            Console.WriteLine("As rumors swell into dread, five cities stand between the realm and collapse:");
            System.Threading.Thread.Sleep(500);
            Console.WriteLine();
            Console.WriteLine("  🏰 Havenbrook  — A crossroads where gossip becomes legend.");
            System.Threading.Thread.Sleep(300);
            Console.WriteLine("  ⚒️  Ironforge  — Forges ring out, but the fires seem dimmer.");
            System.Threading.Thread.Sleep(300);
            Console.WriteLine("  🔮 Mysthaven  — Scholars and sorcerers search for answers in old tomes.");
            System.Threading.Thread.Sleep(300);
            Console.WriteLine("  ☀️  Sunspire   — Ancient deserts guard forgotten rites and bones.");
            System.Threading.Thread.Sleep(300);
            Console.WriteLine("  🌑 Shadowkeep — A citadel of shadows — where truths are buried deep.");
            Console.WriteLine();
            System.Threading.Thread.Sleep(700);

            Console.WriteLine("You are called not by fate alone, but by a letter from Elder Morris — a plea for aid tucked into an old oak's knot.");
            System.Threading.Thread.Sleep(700);
            Console.WriteLine("Step lightly, gather allies, and listen to the land. Every clue, every conversation, may turn the tide.");
            Console.WriteLine();

            Console.WriteLine("Your journey will begin in Havenbrook. Prepare your party, sharpen your wits, and keep your courage close.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
            Console.Clear();

            _hasSeenIntro = true;
        }

        public void DisplayCurrentObjective()
        {
            var currentChapterData = _chapters.FirstOrDefault(c => c.ChapterNumber == CurrentChapter);
            if (currentChapterData == null || currentChapterData.IsCompleted)
            {
                if (CurrentChapter > _chapters.Count)
                {
                    Console.WriteLine("\n📖 [STORY] Congratulations! You have completed the main storyline!");
                    return;
                }
                return;
            }

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📖 MAIN QUEST: Chapter {currentChapterData.ChapterNumber} - {currentChapterData.Title,-42}║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"🎯 Current Objective: {currentChapterData.ObjectiveDescription}");
            Console.WriteLine($"📍 Location: {currentChapterData.RequiredLocation}");
            Console.WriteLine($"⭐ Required Level: {currentChapterData.RequiredLevel}");
        }

        public void CheckStoryProgression(string locationName, List<Character> party, Journal? journal)
        {
            var currentChapterData = _chapters.FirstOrDefault(c => c.ChapterNumber == CurrentChapter);
            if (currentChapterData == null || currentChapterData.IsCompleted) return;

            if (locationName == currentChapterData.RequiredLocation)
            {
                var partyLevel = party.Max(p => p.Level);
                if (partyLevel >= currentChapterData.RequiredLevel)
                {
                    AdvanceStory(party);
                }
                else
                {
                    Console.WriteLine($"\n📖 [STORY] You sense this location is important, but you need to be level {currentChapterData.RequiredLevel} to proceed.");
                }
            }
        }

        private void AdvanceStory(List<Character> party)
        {
            var currentChapterData = _chapters.FirstOrDefault(c => c.ChapterNumber == CurrentChapter);
            if (currentChapterData == null) return;

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    📖 STORY PROGRESS                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"\n✨ Chapter {currentChapterData.ChapterNumber}: {currentChapterData.Title} - COMPLETED!");
            Console.WriteLine($"\n{GetChapterCompletionNarrative(currentChapterData.ChapterNumber)}");
            Console.WriteLine($"\n🎁 Reward: {currentChapterData.RewardDescription}");

            // Give rewards
            var goldReward = currentChapterData.ChapterNumber * 100;
            var expReward = currentChapterData.ChapterNumber * 200;

            foreach (var character in party)
            {
                character.Inventory.AddGold(goldReward);
                character.GainExperience(expReward);
            }

            Console.WriteLine($"\n💰 Each party member received {goldReward} gold!");
            Console.WriteLine($"⭐ Each party member gained {expReward} experience!");

            currentChapterData.IsCompleted = true;
            CurrentChapter++;

            if (CurrentChapter <= _chapters.Count)
            {
                var nextChapter = _chapters.FirstOrDefault(c => c.ChapterNumber == CurrentChapter);
                if (nextChapter != null)
                {
                    Console.WriteLine($"\n\n╔════════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine($"║            📖 NEW CHAPTER: {nextChapter.Title,-42}║");
                    Console.WriteLine($"╚════════════════════════════════════════════════════════════════════╝");
                    Console.WriteLine($"\n{nextChapter.Description}");
                    Console.WriteLine($"\n🎯 Objective: {nextChapter.ObjectiveDescription}");
                    Console.WriteLine($"📍 Location: {nextChapter.RequiredLocation}");
                }
            }
            else
            {
                DisplayEnding();
            }

            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey(true);
            Console.Clear();
        }

        private string GetChapterCompletionNarrative(int chapter)
        {
            return chapter switch
            {
                1 => "Elder Morris shared ancient knowledge about the darkness. The corruption started in the east, and the five cities hold the key to stopping it.",
                2 => "Master Thorgrim forged a powerful bond with you. The dwarves' ancient records speak of a time when this darkness appeared before.",
                3 => "Archmage Elara revealed the magical nature of the threat. The corruption feeds on magical energy across the realm.",
                4 => "Prince Rashid showed you ancient texts from the desert tombs. They describe a ritual to seal the darkness forever.",
                5 => "Lord Malachar revealed the shocking truth: the darkness originates from the Blightlands, where an ancient evil was imprisoned long ago.",
                6 => "You have saved the realm! The darkness is sealed, and peace returns to the land. Heroes will sing of your deeds for generations!",
                _ => "Your journey continues..."
            };
        }

        private void DisplayEnding()
        {
            Console.WriteLine("\n\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    🏆 STORYLINE COMPLETE! 🏆                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("You have visited all five great cities, uncovered their secrets,");
            Console.WriteLine("and confronted the darkness threatening the realm.");
            Console.WriteLine();
            Console.WriteLine("The people sing songs of your bravery. Your name will be");
            Console.WriteLine("remembered in history as the hero who saved the world!");
            Console.WriteLine();
            Console.WriteLine("But your adventure doesn't end here...");
            Console.WriteLine("Continue exploring, complete side quests, and become legendary!");
        }

        public void DisplayStoryJournal()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    📖 MAIN STORYLINE                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");

            foreach (var chapter in _chapters)
            {
                string status = chapter.IsCompleted ? "✅" : (chapter.ChapterNumber == CurrentChapter ? "📍" : "🔒");
                Console.WriteLine($"\n{status} Chapter {chapter.ChapterNumber}: {chapter.Title}");

                if (chapter.IsCompleted)
                {
                    Console.WriteLine($"   {chapter.Description}");
                    Console.WriteLine($"   Status: COMPLETED");
                }
                else if (chapter.ChapterNumber == CurrentChapter)
                {
                    Console.WriteLine($"   {chapter.Description}");
                    Console.WriteLine($"   🎯 Objective: {chapter.ObjectiveDescription}");
                    Console.WriteLine($"   📍 Location: {chapter.RequiredLocation} (Level {chapter.RequiredLevel}+)");
                }
                else
                {
                    Console.WriteLine("   [LOCKED - Complete previous chapters]");
                }
            }

            Console.WriteLine("\n\nPress any key to continue...");
            Console.ReadKey(true);
            Console.Clear();
        }

        public bool HasSeenIntro() => _hasSeenIntro;

        public void MarkIntroSeen() => _hasSeenIntro = true;

        #endregion
    }

    #endregion
}
