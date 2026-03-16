using System;
using System.Collections.Generic;
using System.Linq;
using Night.Characters;

namespace Rpg_Dungeon
{
    #region NPC Type Enum

    internal enum NPCType
    {
        Merchant,
        Questgiver,
        Informant,
        Guard,
        Citizen,
        Traveler,
        Elder
    }

    #endregion

    #region NPC Class

    internal class NPC
    {
        #region Properties

        public string Name { get; }
        public NPCType Type { get; }
        public string Description { get; }
        public List<string> DialogueOptions { get; }
        public List<Quest> AvailableQuests { get; }
        public string LocationName { get; }
        public bool IsStoryNPC { get; }
        public int StoryProgressionRequired { get; }

        #endregion

        #region Constructor

        public NPC(string name, NPCType type, string description, string locationName, bool isStoryNPC = false, int storyProgressionRequired = 0)
        {
            Name = name;
            Type = type;
            Description = description;
            LocationName = locationName;
            DialogueOptions = new List<string>();
            AvailableQuests = new List<Quest>();
            IsStoryNPC = isStoryNPC;
            StoryProgressionRequired = storyProgressionRequired;
            InitializeDialogue();
        }

        #endregion

        #region Initialization

        private void InitializeDialogue()
        {
            switch (Type)
            {
                case NPCType.Merchant:
                    DialogueOptions.Add("What goods do you have today?");
                    DialogueOptions.Add("Tell me about your travels.");
                    DialogueOptions.Add("Any news from other lands?");
                    break;
                case NPCType.Questgiver:
                    DialogueOptions.Add("Do you need any help?");
                    DialogueOptions.Add("What troubles do you face?");
                    DialogueOptions.Add("Tell me about this place.");
                    break;
                case NPCType.Informant:
                    DialogueOptions.Add("What rumors have you heard?");
                    DialogueOptions.Add("Tell me about nearby dangers.");
                    DialogueOptions.Add("Any valuable information?");
                    break;
                case NPCType.Guard:
                    DialogueOptions.Add("What's your duty here?");
                    DialogueOptions.Add("Any trouble in town?");
                    DialogueOptions.Add("Tell me about this area's defenses.");
                    break;
                case NPCType.Citizen:
                    DialogueOptions.Add("How's life in this town?");
                    DialogueOptions.Add("Any local legends?");
                    DialogueOptions.Add("What do you do here?");
                    break;
                case NPCType.Traveler:
                    DialogueOptions.Add("Where have you been?");
                    DialogueOptions.Add("Any advice for an adventurer?");
                    DialogueOptions.Add("What's beyond here?");
                    break;
                case NPCType.Elder:
                    DialogueOptions.Add("Share your wisdom.");
                    DialogueOptions.Add("Tell me about the old days.");
                    DialogueOptions.Add("What secrets do you know?");
                    break;
            }

            DialogueOptions.Add("Farewell.");
        }

        #endregion

        #region Interaction

        public void Interact(List<Character> party, Journal journal, MainStoryline mainStoryline)
        {
            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {GetNPCIcon()} {Name,-59}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"{Description}");

            if (IsStoryNPC && mainStoryline.CurrentChapter >= StoryProgressionRequired)
            {
                Console.WriteLine("\n✨ [STORY NPC] - This person seems important to your quest.");
            }

            while (true)
            {
                Console.WriteLine("\n--- Conversation Options ---");
                for (int i = 0; i < DialogueOptions.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {DialogueOptions[i]}");
                }

                if (AvailableQuests.Count > 0)
                {
                    Console.WriteLine($"Q) View Available Quests ({AvailableQuests.Count})");
                }

                Console.Write("\nYour choice: ");
                var choice = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (choice == "Q" && AvailableQuests.Count > 0)
                {
                    OfferQuests(party, journal);
                }
                else if (int.TryParse(choice, out int dialogueIndex) && dialogueIndex > 0 && dialogueIndex <= DialogueOptions.Count)
                {
                    string selectedDialogue = DialogueOptions[dialogueIndex - 1];

                    if (selectedDialogue == "Farewell.")
                    {
                        Console.WriteLine($"\n{Name}: \"Safe travels, adventurer!\"");
                        return;
                    }

                    RespondToDialogue(selectedDialogue, mainStoryline);
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }

        private void RespondToDialogue(string dialogue, MainStoryline mainStoryline)
        {
            Console.WriteLine($"\n{Name}: ", ConsoleColor.Cyan);
            Console.ResetColor();

            // Generate contextual responses
            var random = new Random();
            string response = dialogue switch
            {
                "What goods do you have today?" => "I have the finest wares from across the realm! Check my shop.",
                "Tell me about your travels." => GenerateTravelStory(),
                "Any news from other lands?" => GenerateNewsResponse(mainStoryline),
                "Do you need any help?" => "Actually, yes! I have a task that might interest you.",
                "What troubles do you face?" => GenerateTroubleResponse(),
                "Tell me about this place." => $"This place has a rich history. {GenerateLocationLore()}",
                "What rumors have you heard?" => GenerateRumorResponse(mainStoryline),
                "Tell me about nearby dangers." => GenerateDangerResponse(),
                "Any valuable information?" => "Information is valuable indeed. " + GenerateInfoResponse(),
                "What's your duty here?" => "I keep the peace and protect the citizens. It's an honorable duty.",
                "Any trouble in town?" => random.Next(0, 3) == 0 ? "Nothing serious, just the usual petty crimes." : "Things are quiet... perhaps too quiet.",
                "Tell me about this area's defenses." => "We're well protected, but one can never be too careful.",
                "How's life in this town?" => "It's peaceful, mostly. We work hard and live simple lives.",
                "Any local legends?" => GenerateLegendResponse(),
                "What do you do here?" => GenerateOccupationResponse(),
                "Where have you been?" => GenerateTravelHistory(),
                "Any advice for an adventurer?" => GenerateAdviceResponse(),
                "What's beyond here?" => "Many dangers and opportunities await beyond our borders.",
                "Share your wisdom." => GenerateWisdomResponse(),
                "Tell me about the old days." => GenerateOldDaysResponse(),
                "What secrets do you know?" => GenerateSecretResponse(mainStoryline),
                _ => "Interesting question..."
            };

            Console.WriteLine(response);
        }

        private string GenerateTravelStory()
        {
            var stories = new[]
            {
                "I once traveled to Ironforge Citadel. The smiths there can forge anything!",
                "The Mysthaven port is beautiful, but the mists make navigation treacherous.",
                "I saw the golden spires of Sunspire once. What a sight to behold!",
                "Shadowkeep is not for the faint of heart. Only the bravest venture there.",
                "The roads between settlements can be dangerous. Travel with companions."
            };
            return stories[new Random().Next(stories.Length)];
        }

        private string GenerateNewsResponse(MainStoryline mainStoryline)
        {
            if (mainStoryline.CurrentChapter >= 2)
            {
                return "Dark forces are stirring in the land. Some say an ancient evil is awakening.";
            }
            return "Trade routes are thriving, but monster attacks are increasing.";
        }

        private string GenerateTroubleResponse()
        {
            var troubles = new[]
            {
                "Bandits have been harassing travelers on the road nearby.",
                "A dangerous creature has made its lair close to our settlement.",
                "We're running low on supplies and need help gathering resources.",
                "Strange occurrences have been happening at night.",
                "Our crops are failing and we don't know why."
            };
            return troubles[new Random().Next(troubles.Length)];
        }

        private string GenerateLocationLore()
        {
            return "Many heroes have passed through here, each leaving their mark on our history.";
        }

        private string GenerateRumorResponse(MainStoryline mainStoryline)
        {
            if (mainStoryline.CurrentChapter >= 3)
            {
                return "They say the Shadowkeep holds the key to defeating the growing darkness.";
            }
            return "I heard there's treasure hidden in the old mountain ruins.";
        }

        private string GenerateDangerResponse()
        {
            var dangers = new[]
            {
                "Goblins have been spotted in the nearby woods. Be careful!",
                "A pack of wolves has been hunting in this area.",
                "Bandits set up ambushes on less-traveled paths.",
                "Some dungeons are best avoided unless you're well prepared.",
                "The weather can be as dangerous as any monster out there."
            };
            return dangers[new Random().Next(dangers.Length)];
        }

        private string GenerateInfoResponse()
        {
            return "Visit all the major towns to gain knowledge and power. Each specializes in something unique.";
        }

        private string GenerateLegendResponse()
        {
            var legends = new[]
            {
                "They say a dragon guards an ancient treasure in the mountains.",
                "Long ago, a great hero sealed away an evil in these lands.",
                "The old ones speak of a time when magic flowed freely everywhere.",
                "There's a legend about a sword that can cut through any darkness."
            };
            return legends[new Random().Next(legends.Length)];
        }

        private string GenerateOccupationResponse()
        {
            var occupations = new[]
            {
                "I'm a farmer. It's honest work.",
                "I maintain the roads and paths around here.",
                "I help at the local inn when they need extra hands.",
                "I craft simple goods for the community.",
                "I watch over the children while parents work."
            };
            return occupations[new Random().Next(occupations.Length)];
        }

        private string GenerateTravelHistory()
        {
            return "I've been all over! Each place has its own character and charm. Some more dangerous than others!";
        }

        private string GenerateAdviceResponse()
        {
            var advice = new[]
            {
                "Always keep your equipment in good condition.",
                "Don't venture into areas above your level - it's suicide.",
                "Make friends in every town. You never know when you'll need help.",
                "Save your gold for important purchases. Quality over quantity.",
                "Train regularly. Skills can save your life."
            };
            return advice[new Random().Next(advice.Length)];
        }

        private string GenerateWisdomResponse()
        {
            return "The path of an adventurer is filled with trials, but also great rewards. Stay true to your purpose.";
        }

        private string GenerateOldDaysResponse()
        {
            return "Back in my day, monsters were fiercer and adventurers were braver. Or perhaps that's just how I remember it.";
        }

        private string GenerateSecretResponse(MainStoryline mainStoryline)
        {
            if (mainStoryline.CurrentChapter >= 4)
            {
                return "I know of an ancient artifact that could help you. Seek it in the corrupted lands.";
            }
            return "Secrets are earned, not given. Prove yourself worthy first.";
        }

        private void OfferQuests(List<Character> party, Journal journal)
        {
            Console.WriteLine($"\n{Name} has the following quests available:");

            for (int i = 0; i < AvailableQuests.Count; i++)
            {
                var quest = AvailableQuests[i];
                Console.WriteLine($"\n{i + 1}) {quest.Name} [{quest.Difficulty}]");
                Console.WriteLine($"   {quest.Description}");
                Console.WriteLine($"   Objective: {quest.ObjectiveName} ({quest.CurrentProgress}/{quest.ObjectiveCount})");
                Console.WriteLine($"   Rewards: {quest.GoldReward} gold, {quest.ExperienceReward} XP");
            }

            Console.Write("\nAccept which quest? (0 to cancel): ");
            var choice = Console.ReadLine()?.Trim() ?? "";

            if (int.TryParse(choice, out int questIndex) && questIndex > 0 && questIndex <= AvailableQuests.Count)
            {
                var selectedQuest = AvailableQuests[questIndex - 1];
                journal.AddActiveQuest(selectedQuest);
                AvailableQuests.RemoveAt(questIndex - 1);
                Console.WriteLine($"\n✅ Quest '{selectedQuest.Name}' accepted!");
            }
        }

        private string GetNPCIcon()
        {
            return Type switch
            {
                NPCType.Merchant => "🛒",
                NPCType.Questgiver => "❗",
                NPCType.Informant => "💬",
                NPCType.Guard => "🛡️",
                NPCType.Citizen => "👤",
                NPCType.Traveler => "🎒",
                NPCType.Elder => "👴",
                _ => "👤"
            };
        }

        #endregion
    }

    #endregion

    #region NPC Manager

    internal class NPCManager
    {
        #region Fields

        private readonly Dictionary<string, List<NPC>> _npcsByLocation;
        private readonly Random _random;

        #endregion

        #region Constructor

        public NPCManager()
        {
            _npcsByLocation = new Dictionary<string, List<NPC>>();
            _random = new Random();
            InitializeNPCs();
        }

        #endregion

        #region Initialization

        private void InitializeNPCs()
        {
            // Main Town - GreyWolf (10+ NPCs)
            InitializeGreyWolfNPCs();

            // Major Towns - 5-8 NPCs each
            InitializeHavenbrookNPCs();
            InitializeIronforgeNPCs();
            InitializeMysthavenNPCs();
            InitializeSunspireNPCs();
            InitializeShadowkeepNPCs();

            // Settlements - 3-5 NPCs each (expanded)
            InitializeSettlementNPCs();

            // Camps - 1 NPC each
            InitializeCampNPCs();
        }

        private void InitializeGreyWolfNPCs()
        {
            var npcs = new List<NPC>();

            // Central Square NPCs
            var townCrier = new NPC("Harold the Town Crier", NPCType.Informant,
                "A loud voice announcing news and events throughout the town.",
                "GreyWolf Town");
            npcs.Add(townCrier);

            var fountainKeeper = new NPC("Old Tom", NPCType.Elder,
                "An elderly man who maintains the town fountain and shares stories of the past.",
                "GreyWolf Town");
            npcs.Add(fountainKeeper);

            var streetMerchant = new NPC("Rosie the Flower Seller", NPCType.Merchant,
                "A cheerful woman selling flowers and small trinkets in the square.",
                "GreyWolf Town");
            npcs.Add(streetMerchant);

            // Crafters District (North Side) NPCs
            var masterBlacksmith = new NPC("Bjorn Ironhammer", NPCType.Questgiver,
                "The master blacksmith at The Rusty Anvil, always needing rare materials.",
                "GreyWolf Town");
            masterBlacksmith.AvailableQuests.Add(new Quest(
                "Forge Master's Request",
                "I need 5 iron ingots to create masterwork weapons. Collect them for me.",
                QuestType.Collect,
                QuestDifficulty.Medium,
                "Iron Ingot",
                5,
                100,
                150
            ));
            npcs.Add(masterBlacksmith);

            var leatherWorker = new NPC("Talia Softhand", NPCType.Merchant,
                "An expert leather worker at Tanner's Workshop, known for quality armor.",
                "GreyWolf Town");
            npcs.Add(leatherWorker);

            var jeweler = new NPC("Gemma Goldweaver", NPCType.Merchant,
                "A skilled jeweler at Golden Trinkets, with an eye for precious stones.",
                "GreyWolf Town");
            npcs.Add(jeweler);

            var craftApprentice = new NPC("Young Marcus", NPCType.Citizen,
                "An eager apprentice running errands between the craft shops.",
                "GreyWolf Town");
            npcs.Add(craftApprentice);

            // Service Quarter (East Side) NPCs
            var innkeeper = new NPC("Martha Warmheart", NPCType.Citizen,
                "The kind innkeeper of The Cozy Inn, always ready with a warm meal.",
                "GreyWolf Town");
            npcs.Add(innkeeper);

            var banker = new NPC("Cornelius Goldvault", NPCType.Merchant,
                "The strict but fair banker who manages GreyWolf Bank & Storage.",
                "GreyWolf Town");
            npcs.Add(banker);

            var guildMaster = new NPC("Guild Master Varen", NPCType.Questgiver,
                "The experienced guild master at the Adventurer's Guild.",
                "GreyWolf Town");
            guildMaster.AvailableQuests.Add(new Quest(
                "Guild Initiation",
                "Prove yourself by defeating 10 monsters in the wilderness.",
                QuestType.Kill,
                QuestDifficulty.Medium,
                "Monster",
                10,
                120,
                200
            ));
            npcs.Add(guildMaster);

            var bountyClerk = new NPC("Scarlett Redmark", NPCType.Informant,
                "A sharp-eyed woman who tracks bounties at the Hunter's Lodge.",
                "GreyWolf Town");
            npcs.Add(bountyClerk);

            var trainer = new NPC("Master Swordsman Drake", NPCType.Questgiver,
                "A legendary warrior who trains adventurers at the Training Hall.",
                "GreyWolf Town");
            trainer.AvailableQuests.Add(new Quest(
                "Warrior's Trial",
                "Complete training by exploring 3 dangerous areas.",
                QuestType.Explore,
                QuestDifficulty.Medium,
                "Dungeon Level",
                3,
                90,
                140
            ));
            npcs.Add(trainer);

            // Mystic Quarter (South Side) NPCs
            var arcaneShopkeeper = new NPC("Wizard Aldous", NPCType.Merchant,
                "An eccentric wizard running the Arcane Emporium with rare magical items.",
                "GreyWolf Town");
            npcs.Add(arcaneShopkeeper);

            var apothecary = new NPC("Healer Lyanna", NPCType.Merchant,
                "A gentle healer at Healing Hands, skilled in herbal remedies.",
                "GreyWolf Town");
            npcs.Add(apothecary);

            var achievementKeeper = new NPC("Historian Eldric", NPCType.Elder,
                "The keeper of the Hall of Fame, recording legendary deeds.",
                "GreyWolf Town");
            npcs.Add(achievementKeeper);

            var ascensionMaster = new NPC("Champion Seraphina", NPCType.Elder,
                "A legendary champion who oversees the path to ascension.",
                "GreyWolf Town", true, 25);
            npcs.Add(ascensionMaster);

            // Entertainment District (West Side) NPCs
            var gambler = new NPC("Lucky Fingers", NPCType.Merchant,
                "A charismatic host of The Lucky Dragon gambling den.",
                "GreyWolf Town");
            npcs.Add(gambler);

            var beastTamer = new NPC("Fauna Wildwhisper", NPCType.Merchant,
                "A kind beast tamer at the Pet Stable with many animal companions.",
                "GreyWolf Town");
            npcs.Add(beastTamer);

            var tailor = new NPC("Silk Silverthimble", NPCType.Merchant,
                "An elegant elven tailor at Fine Threads, creating beautiful garments.",
                "GreyWolf Town");
            npcs.Add(tailor);

            // General Town NPCs
            var guardCaptain = new NPC("Captain Ironshield", NPCType.Guard,
                "The captain of the GreyWolf town guard, maintaining order and safety.",
                "GreyWolf Town");
            guardCaptain.AvailableQuests.Add(new Quest(
                "Town Defense",
                "Bandits have been spotted nearby. Eliminate 6 threats to the town.",
                QuestType.Kill,
                QuestDifficulty.Easy,
                "Bandit",
                6,
                80,
                120
            ));
            npcs.Add(guardCaptain);

            var townPatrol = new NPC("Guard Brennan", NPCType.Guard,
                "A friendly town guard patrolling the streets.",
                "GreyWolf Town");
            npcs.Add(townPatrol);

            var wanderer = new NPC("Mysterious Wanderer", NPCType.Traveler,
                "A hooded figure who has seen many lands and knows many secrets.",
                "GreyWolf Town");
            npcs.Add(wanderer);

            var baker = new NPC("Betty Sweetbread", NPCType.Citizen,
                "A plump baker whose shop fills the air with delicious scents.",
                "GreyWolf Town");
            npcs.Add(baker);

            var librarian = new NPC("Scholar Percival", NPCType.Informant,
                "A bookish man who runs the town library and knows local lore.",
                "GreyWolf Town");
            npcs.Add(librarian);

            var stableHand = new NPC("Jack Haymaker", NPCType.Citizen,
                "A hard-working stable hand who cares for horses and mounts.",
                "GreyWolf Town");
            npcs.Add(stableHand);

            var childNPC = new NPC("Little Timmy", NPCType.Citizen,
                "A curious child who loves hearing about adventures.",
                "GreyWolf Town");
            npcs.Add(childNPC);

            _npcsByLocation["GreyWolf Town"] = npcs;
        }

        private void InitializeHavenbrookNPCs()
        {
            var npcs = new List<NPC>();

            var elderMorris = new NPC("Elder Morris", NPCType.Elder,
                "The wise elder of Havenbrook, keeper of ancient knowledge.",
                "Havenbrook", true, 0);
            elderMorris.DialogueOptions.Add("Tell me about the ancient prophecy.");
            npcs.Add(elderMorris);

            var guardCaptain = new NPC("Captain Aldric", NPCType.Guard,
                "The captain of the town guard, vigilant and experienced.",
                "Havenbrook");
            npcs.Add(guardCaptain);

            var merchantSara = new NPC("Sara the Trader", NPCType.Merchant,
                "A friendly merchant with goods from distant lands.",
                "Havenbrook");
            npcs.Add(merchantSara);

            var questgiverTom = new NPC("Tom Brightwood", NPCType.Questgiver,
                "A local farmer who often needs help with problems.",
                "Havenbrook");
            questgiverTom.AvailableQuests.Add(new Quest(
                "Farmland Protection",
                "Wolves have been attacking my livestock. Please deal with 3 wolves.",
                QuestType.Kill,
                QuestDifficulty.Easy,
                "Wolf",
                3,
                40,
                75
            ));
            npcs.Add(questgiverTom);

            var informant = new NPC("Mysterious Hooded Figure", NPCType.Informant,
                "A shadowy figure who seems to know things others don't.",
                "Havenbrook");
            npcs.Add(informant);

            var traveler = new NPC("Wandering Bard", NPCType.Traveler,
                "A traveling bard with tales from across the realm.",
                "Havenbrook");
            npcs.Add(traveler);

            var citizen1 = new NPC("Mary Cooper", NPCType.Citizen,
                "A cheerful baker who runs the local bakery.",
                "Havenbrook");
            npcs.Add(citizen1);

            var citizen2 = new NPC("Blacksmith's Son", NPCType.Citizen,
                "A young apprentice learning the smithing trade.",
                "Havenbrook");
            npcs.Add(citizen2);

            _npcsByLocation["Havenbrook"] = npcs;
        }

        private void InitializeIronforgeNPCs()
        {
            var npcs = new List<NPC>();

            var masterSmith = new NPC("Master Thorgrim", NPCType.Elder,
                "The legendary master smith of Ironforge, gruff but fair.",
                "Ironforge Citadel", true, 2);
            masterSmith.AvailableQuests.Add(new Quest(
                "Rare Ore Collection",
                "I need rare ore from the deep mines. Bring me 5 pieces.",
                QuestType.Collect,
                QuestDifficulty.Medium,
                "Rare Ore",
                5,
                150,
                200,
                new Equipment("Masterwork Hammer", EquipmentType.Weapon, 60, 150)
            ));
            npcs.Add(masterSmith);

            var guardDwarf = new NPC("Guard Ironbeard", NPCType.Guard,
                "A stout dwarf guard protecting the forges.",
                "Ironforge Citadel");
            npcs.Add(guardDwarf);

            var merchant = new NPC("Greta Ironheart", NPCType.Merchant,
                "A dwarven merchant specializing in metal goods.",
                "Ironforge Citadel");
            npcs.Add(merchant);

            var questgiver = new NPC("Engineer Cogsworth", NPCType.Questgiver,
                "An inventor working on various mechanical projects.",
                "Ironforge Citadel");
            questgiver.AvailableQuests.Add(new Quest(
                "Gear Recovery",
                "My prototype fell into the mines! Explore 3 dungeon levels to find it.",
                QuestType.Explore,
                QuestDifficulty.Medium,
                "Dungeon Level",
                3,
                100,
                150
            ));
            npcs.Add(questgiver);

            var traveler = new NPC("Mountain Guide", NPCType.Traveler,
                "An experienced guide who knows every mountain path.",
                "Ironforge Citadel");
            npcs.Add(traveler);

            var citizen = new NPC("Forge Apprentice", NPCType.Citizen,
                "A young dwarf learning the ancient art of smithing.",
                "Ironforge Citadel");
            npcs.Add(citizen);

            _npcsByLocation["Ironforge Citadel"] = npcs;
        }

        private void InitializeMysthavenNPCs()
        {
            var npcs = new List<NPC>();

            var archmage = new NPC("Archmage Elara", NPCType.Elder,
                "The wise archmage who leads the magical academy.",
                "Mysthaven", true, 3);
            archmage.AvailableQuests.Add(new Quest(
                "Mana Crystal Collection",
                "I need mana crystals for my research. Collect 5 of them.",
                QuestType.Collect,
                QuestDifficulty.Medium,
                "Mana Crystal",
                5,
                200,
                300,
                new Equipment("Arcane Staff", EquipmentType.Weapon, 70, 200)
            ));
            npcs.Add(archmage);

            var guard = new NPC("Spell Guardian", NPCType.Guard,
                "A mage-guard who protects the city with magic.",
                "Mysthaven");
            npcs.Add(guard);

            var merchant = new NPC("Mystic Trader", NPCType.Merchant,
                "A merchant dealing in enchanted items and reagents.",
                "Mysthaven");
            npcs.Add(merchant);

            var questgiver = new NPC("Scholar Finn", NPCType.Questgiver,
                "A young scholar researching ancient magical texts.",
                "Mysthaven");
            questgiver.AvailableQuests.Add(new Quest(
                "Ancient Tome Recovery",
                "An ancient tome was stolen by bandits. Defeat 5 enemies to recover it.",
                QuestType.Kill,
                QuestDifficulty.Medium,
                "Bandit",
                5,
                120,
                180
            ));
            npcs.Add(questgiver);

            var informant = new NPC("Oracle Mystica", NPCType.Informant,
                "A mysterious oracle who sees glimpses of the future.",
                "Mysthaven");
            npcs.Add(informant);

            var traveler = new NPC("Sea Captain", NPCType.Traveler,
                "A grizzled captain who has sailed the seven seas.",
                "Mysthaven");
            npcs.Add(traveler);

            var citizen = new NPC("Apprentice Mage", NPCType.Citizen,
                "A young magic student studying at the academy.",
                "Mysthaven");
            npcs.Add(citizen);

            _npcsByLocation["Mysthaven"] = npcs;
        }

        private void InitializeSunspireNPCs()
        {
            var npcs = new List<NPC>();

            var desertPrince = new NPC("Prince Rashid", NPCType.Elder,
                "The noble prince of Sunspire, guardian of desert secrets.",
                "Sunspire", true, 4);
            desertPrince.AvailableQuests.Add(new Quest(
                "Tomb Raider's Challenge",
                "Explore the ancient tombs and clear 5 dungeon levels.",
                QuestType.Explore,
                QuestDifficulty.Hard,
                "Dungeon Level",
                5,
                300,
                400,
                new Equipment("Desert King's Crown", EquipmentType.Accessory, 80, 300)
            ));
            npcs.Add(desertPrince);

            var guard = new NPC("Desert Sentinel", NPCType.Guard,
                "A warrior clad in golden armor protecting the city.",
                "Sunspire");
            npcs.Add(guard);

            var merchant = new NPC("Exotic Trader", NPCType.Merchant,
                "A merchant with rare and exotic goods from the desert.",
                "Sunspire");
            npcs.Add(merchant);

            var questgiver = new NPC("Tomb Scholar", NPCType.Questgiver,
                "An archaeologist seeking artifacts from ancient tombs.",
                "Sunspire");
            questgiver.AvailableQuests.Add(new Quest(
                "Scarab Hunt",
                "Ancient scarabs guard the tombs. Defeat 8 of them.",
                QuestType.Kill,
                QuestDifficulty.Hard,
                "Scarab",
                8,
                250,
                350
            ));
            npcs.Add(questgiver);

            var informant = new NPC("Desert Nomad", NPCType.Informant,
                "A nomad with knowledge of hidden oases and shortcuts.",
                "Sunspire");
            npcs.Add(informant);

            var citizen = new NPC("Carpet Merchant", NPCType.Citizen,
                "A friendly merchant selling beautiful carpets.",
                "Sunspire");
            npcs.Add(citizen);

            _npcsByLocation["Sunspire"] = npcs;
        }

        private void InitializeShadowkeepNPCs()
        {
            var npcs = new List<NPC>();

            var darkLord = new NPC("Lord Malachar", NPCType.Elder,
                "The enigmatic ruler of Shadowkeep, master of shadow magic.",
                "Shadowkeep", true, 5);
            darkLord.AvailableQuests.Add(new Quest(
                "Shadow Mastery",
                "Prove your worth by defeating the darkness. Kill 10 shadow creatures.",
                QuestType.Kill,
                QuestDifficulty.Elite,
                "Shadow Beast",
                10,
                500,
                800,
                new Equipment("Shadowblade", EquipmentType.Weapon, 100, 500)
            ));
            npcs.Add(darkLord);

            var guard = new NPC("Shadow Knight", NPCType.Guard,
                "An elite knight sworn to protect Shadowkeep.",
                "Shadowkeep");
            npcs.Add(guard);

            var merchant = new NPC("Dark Merchant", NPCType.Merchant,
                "A mysterious merchant dealing in rare dark artifacts.",
                "Shadowkeep");
            npcs.Add(merchant);

            var questgiver = new NPC("Shadowmancer", NPCType.Questgiver,
                "A dark mage seeking powerful reagents.",
                "Shadowkeep");
            questgiver.AvailableQuests.Add(new Quest(
                "Dark Essence Collection",
                "Collect dark essence from fallen enemies. Find 7 essence.",
                QuestType.Collect,
                QuestDifficulty.Hard,
                "Dark Essence",
                7,
                350,
                500
            ));
            npcs.Add(questgiver);

            var informant = new NPC("Spymaster", NPCType.Informant,
                "The keeper of secrets and information in Shadowkeep.",
                "Shadowkeep", true, 5);
            npcs.Add(informant);

            var citizen = new NPC("Dark Scholar", NPCType.Citizen,
                "A scholar studying forbidden knowledge.",
                "Shadowkeep");
            npcs.Add(citizen);

            _npcsByLocation["Shadowkeep"] = npcs;
        }

        private void InitializeSettlementNPCs()
        {
            var settlementNames = new[]
            {
                "Willowdale", "Crossroads Keep", "Pinewood", "Riverside", "Stonebridge",
                "Frosthollow", "Oasis Rest", "Moonwell", "Thornwall", "Ghostlight"
            };

            foreach (var settlementName in settlementNames)
            {
                var npcs = new List<NPC>();

                // Every settlement has an innkeeper (guaranteed)
                var innkeeper = new NPC(GetSettlementInnkeeperName(settlementName), NPCType.Citizen,
                    $"The welcoming innkeeper of {settlementName}.",
                    settlementName);
                npcs.Add(innkeeper);

                // Every settlement has a guard (guaranteed)
                var guard = new NPC(GetSettlementGuardName(settlementName), NPCType.Guard,
                    $"A vigilant guard protecting {settlementName} from threats.",
                    settlementName);
                npcs.Add(guard);

                // Every settlement has a merchant (guaranteed)
                var merchant = new NPC(GetSettlementMerchantName(settlementName), NPCType.Merchant,
                    $"A traveling merchant who regularly stops at {settlementName}.",
                    settlementName);
                npcs.Add(merchant);

                // Most settlements have a local questgiver (80% chance)
                if (_random.Next(0, 10) < 8)
                {
                    var questgiver = new NPC(GetSettlementQuestgiverName(settlementName), NPCType.Questgiver,
                        $"A local resident of {settlementName} who needs assistance.",
                        settlementName);
                    questgiver.AvailableQuests.Add(GenerateRandomSettlementQuest());
                    npcs.Add(questgiver);
                }

                // Additional citizens for atmosphere (60% chance)
                if (_random.Next(0, 10) < 6)
                {
                    var citizen1 = new NPC(GetSettlementCitizenName(settlementName), NPCType.Citizen,
                        $"A local resident of {settlementName} going about their day.",
                        settlementName);
                    npcs.Add(citizen1);
                }

                // Occasional travelers passing through (40% chance)
                if (_random.Next(0, 10) < 4)
                {
                    var traveler = new NPC(GetSettlementTravelerName(), NPCType.Traveler,
                        $"A weary traveler resting at {settlementName} before continuing their journey.",
                        settlementName);
                    npcs.Add(traveler);
                }

                // Rare informants with useful information (30% chance)
                if (_random.Next(0, 10) < 3)
                {
                    var informant = new NPC(GetSettlementInformantName(), NPCType.Informant,
                        $"A mysterious person who seems to know the area around {settlementName} well.",
                        settlementName);
                    npcs.Add(informant);
                }

                _npcsByLocation[settlementName] = npcs;
            }
        }

        private void InitializeCampNPCs()
        {
            var campNames = new[]
            {
                "Traveler's Rest", "Wagon Circle", "Milestone Camp", "Crossroads Camp", "Guard Post",
                "Hunter's Clearing", "Woodcutter's Site", "Druid Circle", "Ranger Outpost",
                "Eagle's Nest", "Cave Shelter", "Mountain Pass Camp",
                "Dune Hollow", "Nomad Circle", "Rock Shelter",
                "Fisher's Camp", "Ferry Landing", "Beaver Dam",
                "Temple Steps", "Old Fort"
            };

            foreach (var campName in campNames)
            {
                var npcs = new List<NPC>();

                // Each camp has one random NPC
                var npcType = (NPCType)_random.Next(0, 4); // Traveler, Citizen, Guard, or Questgiver
                var npc = new NPC(GenerateCampNPCName(npcType), npcType,
                    GenerateCampNPCDescription(npcType, campName),
                    campName);

                if (npcType == NPCType.Questgiver && _random.Next(0, 2) == 0)
                {
                    npc.AvailableQuests.Add(GenerateRandomCampQuest());
                }

                npcs.Add(npc);
                _npcsByLocation[campName] = npcs;
            }
        }

        private string GenerateCampNPCName(NPCType type)
        {
            return type switch
            {
                NPCType.Traveler => "Weary Traveler",
                NPCType.Citizen => "Camp Keeper",
                NPCType.Guard => "Patrol Guard",
                NPCType.Questgiver => "Fellow Adventurer",
                _ => "Stranger"
            };
        }

        private string GenerateCampNPCDescription(NPCType type, string campName)
        {
            return type switch
            {
                NPCType.Traveler => $"A tired traveler resting at {campName}.",
                NPCType.Citizen => $"Someone maintaining the {campName} camp.",
                NPCType.Guard => $"A guard patrolling near {campName}.",
                NPCType.Questgiver => $"An adventurer seeking help at {campName}.",
                _ => "A mysterious person at the camp."
            };
        }

        private Quest GenerateRandomSettlementQuest()
        {
            var questTemplates = new[]
            {
                ("Pest Control", "Local creatures are causing trouble. Eliminate 4 of them.", "Pest", 4, 60, 100),
                ("Supply Run", "We need supplies from the wilderness. Collect 3 items.", "Supplies", 3, 70, 90),
                ("Lost Item", "I lost something important nearby. Help me search for it.", "Lost Item", 1, 50, 80),
                ("Guard Duty", "Help patrol the area and defeat any threats you find.", "Enemy", 3, 80, 120)
            };

            var template = questTemplates[_random.Next(questTemplates.Length)];
            return new Quest(
                template.Item1,
                template.Item2,
                _random.Next(0, 2) == 0 ? QuestType.Kill : QuestType.Collect,
                QuestDifficulty.Easy,
                template.Item3,
                template.Item4,
                template.Item5,
                template.Item6
            );
        }

        private Quest GenerateRandomCampQuest()
        {
            var questTemplates = new[]
            {
                ("Help Needed", "I could use some help. Can you collect 2 healing herbs?", "Healing Herb", 2, 30, 50),
                ("Monster Threat", "A monster is nearby. Please defeat 2 of them.", "Monster", 2, 40, 60),
                ("Quick Task", "I need someone to explore nearby. Complete 1 dungeon level.", "Dungeon Level", 1, 35, 55)
            };

            var template = questTemplates[_random.Next(questTemplates.Length)];
            return new Quest(
                template.Item1,
                template.Item2,
                _random.Next(0, 2) == 0 ? QuestType.Kill : QuestType.Collect,
                QuestDifficulty.Easy,
                template.Item3,
                template.Item4,
                template.Item5,
                template.Item6
            );
        }

        #endregion

        #region NPC Access

        public List<NPC> GetNPCsAtLocation(string locationName)
        {
            if (_npcsByLocation.TryGetValue(locationName, out var npcs))
            {
                return npcs;
            }
            return new List<NPC>();
        }

        // Find a quest by name across all NPCs. Returns the Quest instance if found, otherwise null.
        public Quest? FindQuestByName(string questName)
        {
            if (string.IsNullOrWhiteSpace(questName)) return null;
            foreach (var kv in _npcsByLocation)
            {
                foreach (var npc in kv.Value)
                {
                    var q = npc.AvailableQuests.FirstOrDefault(x => string.Equals(x.Name, questName, StringComparison.OrdinalIgnoreCase));
                    if (q != null) return q;
                }
            }
            return null;
        }

        // Remove a quest from any NPC that offers it (used after a player accepts a quest)
        public bool RemoveQuestByName(string questName)
        {
            if (string.IsNullOrWhiteSpace(questName)) return false;
            foreach (var kv in _npcsByLocation)
            {
                for (int i = kv.Value.Count - 1; i >= 0; i--)
                {
                    var npc = kv.Value[i];
                    for (int j = npc.AvailableQuests.Count - 1; j >= 0; j--)
                    {
                        if (string.Equals(npc.AvailableQuests[j].Name, questName, StringComparison.OrdinalIgnoreCase))
                        {
                            npc.AvailableQuests.RemoveAt(j);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void SpawnRandomNPC(string locationName, LocationCategory locationType)
        {
            if (!_npcsByLocation.ContainsKey(locationName))
            {
                _npcsByLocation[locationName] = new List<NPC>();
            }

            var existingCount = _npcsByLocation[locationName].Count;

            // Spawn rate based on location type
            int maxNPCs = locationType switch
            {
                LocationCategory.Town => 8,
                LocationCategory.Settlement => 3,
                LocationCategory.Camp => 1,
                _ => 0
            };

            if (existingCount >= maxNPCs) return;

            // Random chance to spawn
            if (_random.Next(0, 100) < 30) // 30% chance
            {
                var npcType = (NPCType)_random.Next(0, 7);
                var npc = new NPC(
                    GenerateRandomNPCName(npcType),
                    npcType,
                    GenerateRandomNPCDescription(npcType),
                    locationName
                );

                if ((npcType == NPCType.Questgiver || npcType == NPCType.Elder) && _random.Next(0, 2) == 0)
                {
                    npc.AvailableQuests.Add(GenerateRandomSettlementQuest());
                }

                _npcsByLocation[locationName].Add(npc);
                Console.WriteLine($"\n👤 A new person has arrived at {locationName}!");
            }
        }

        private string GenerateRandomNPCName(NPCType type)
        {
            var firstNames = new[] { "John", "Sarah", "Marcus", "Elena", "Thomas", "Lisa", "Robert", "Anna" };
            var lastNames = new[] { "Smith", "Ironwood", "Brightstone", "Shadowend", "Goldleaf", "Stormwind" };

            return type switch
            {
                NPCType.Elder => $"Elder {lastNames[_random.Next(lastNames.Length)]}",
                NPCType.Guard => "Town Guard",
                NPCType.Merchant => $"Merchant {firstNames[_random.Next(firstNames.Length)]}",
                NPCType.Traveler => "Passing Traveler",
                NPCType.Informant => "Mysterious Stranger",
                _ => $"{firstNames[_random.Next(firstNames.Length)]} {lastNames[_random.Next(lastNames.Length)]}"
            };
        }

        private string GenerateRandomNPCDescription(NPCType type)
        {
            return type switch
            {
                NPCType.Elder => "A wise elder with years of experience.",
                NPCType.Guard => "A vigilant guard keeping the peace.",
                NPCType.Merchant => "A trader with various goods for sale.",
                NPCType.Traveler => "A traveler from distant lands.",
                NPCType.Informant => "Someone who seems to know more than they let on.",
                NPCType.Questgiver => "Someone who looks like they need help.",
                _ => "A local resident going about their day."
            };
        }

        private string GetSettlementInnkeeperName(string settlementName)
        {
            return settlementName switch
            {
                "Willowdale" => "Innkeeper Margaret",
                "Crossroads Keep" => "Innkeeper Roland",
                "Pinewood" => "Innkeeper Pine",
                "Riverside" => "Innkeeper Marina",
                "Stonebridge" => "Innkeeper Mason",
                "Frosthollow" => "Innkeeper Frost",
                "Oasis Rest" => "Innkeeper Sahara",
                "Moonwell" => "Innkeeper Luna",
                "Thornwall" => "Innkeeper Thorn",
                "Ghostlight" => "Innkeeper Shade",
                _ => "Local Innkeeper"
            };
        }

        private string GetSettlementGuardName(string settlementName)
        {
            return settlementName switch
            {
                "Willowdale" => "Guard Willow",
                "Crossroads Keep" => "Sergeant Cross",
                "Pinewood" => "Ranger Oakley",
                "Riverside" => "Patrol Officer Wade",
                "Stonebridge" => "Guard Rockheart",
                "Frosthollow" => "Sentinel Icewind",
                "Oasis Rest" => "Desert Warden",
                "Moonwell" => "Night Watch",
                "Thornwall" => "Wall Guardian",
                "Ghostlight" => "Spectral Guard",
                _ => "Settlement Guard"
            };
        }

        private string GetSettlementMerchantName(string settlementName)
        {
            return settlementName switch
            {
                "Willowdale" => "Merchant Greenleaf",
                "Crossroads Keep" => "Trader Pathfinder",
                "Pinewood" => "Merchant Woodsworth",
                "Riverside" => "Fisher's Friend",
                "Stonebridge" => "Trader Bridgeman",
                "Frosthollow" => "Winter Merchant",
                "Oasis Rest" => "Desert Trader Kalim",
                "Moonwell" => "Merchant Starlight",
                "Thornwall" => "Trader Brambleton",
                "Ghostlight" => "Wandering Peddler",
                _ => "Traveling Merchant"
            };
        }

        private string GetSettlementQuestgiverName(string settlementName)
        {
            var names = new[] { "Elder", "Chief", "Mayor", "Headman", "Leader", "Wise One" };
            var settlementShortName = settlementName.Replace(" ", "").Split(' ')[0];
            return $"{names[_random.Next(names.Length)]} of {settlementShortName}";
        }

        private string GetSettlementCitizenName(string settlementName)
        {
            var firstNames = new[] { "Emma", "Jack", "Olivia", "Henry", "Sophia", "William", "Ava", "James", "Isabella", "Oliver" };
            var occupations = new[] { "the Farmer", "the Baker", "the Woodcutter", "the Fisher", "the Weaver", "the Carpenter" };
            return $"{firstNames[_random.Next(firstNames.Length)]} {occupations[_random.Next(occupations.Length)]}";
        }

        private string GetSettlementTravelerName()
        {
            var descriptions = new[] { "Weary Pilgrim", "Road Scholar", "Merchant Caravan", "Wandering Minstrel", "Lost Traveler", "Foreign Visitor" };
            return descriptions[_random.Next(descriptions.Length)];
        }

        private string GetSettlementInformantName()
        {
            var names = new[] { "Mysterious Stranger", "Hooded Figure", "Old Hermit", "Wandering Sage", "Cloaked Visitor", "Silent Observer" };
            return names[_random.Next(names.Length)];
        }

        #endregion
    }

    #endregion
}
