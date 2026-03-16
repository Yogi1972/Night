using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    #region Camp Class

    internal class Camp : Location
    {
        #region Properties

        public CampType CampType { get; }

        #endregion

        #region Constructor

        public Camp(string name, string description, CampType campType)
            : base(name, description, LocationCategory.Camp, 1)
        {
            CampType = campType;
        }

        #endregion

        #region Methods

        public override void Enter(List<Character> party, GameState gameState)
        {
            if (!IsDiscovered)
            {
                IsDiscovered = true;
                Console.WriteLine($"\n✨ New camp discovered: {Name}!");

                // Discover on fog of war map
                gameState.FogOfWarMap?.DiscoverLocation(Name);
            }

            gameState.FogOfWarMap?.SetCurrentLocation(Name);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║        {Name,-37}║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝");
                Console.WriteLine($"⛺ {Description}");
                Console.WriteLine($"   Camp Type: {CampType}");

                // Show NPC if present
                var npcs = gameState.NPCManager?.GetNPCsAtLocation(Name) ?? new List<NPC>();
                if (npcs.Count > 0)
                {
                    Console.WriteLine($"   👤 Someone is at this camp");
                }
                Console.WriteLine();

                Console.WriteLine("--- Camp Options ---");
                Console.WriteLine("1) Rest by the Fire (Free) - Restore 25% HP/Stamina");
                Console.WriteLine("2) Forage for Food - Restore some HP");
                Console.WriteLine("3) Check Surroundings");
                if (npcs.Count > 0)
                {
                    Console.WriteLine("4) Talk to Person at Camp 👤");
                }
                Console.WriteLine("0) Leave Camp");
                Console.Write("Choice: ");

                var choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        RestByFire(party);
                        break;
                    case "2":
                        Forage(party);
                        break;
                    case "3":
                        CheckSurroundings();
                        break;
                    case "4" when npcs.Count > 0:
                        TalkToNPCs(party, gameState);
                        break;
                    case "0":
                        Console.WriteLine($"\nYou leave the camp behind.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void RestByFire(List<Character> party)
        {
            Console.WriteLine("\n🔥 The party gathers around the campfire...");
            Console.WriteLine("The warmth is comforting, but it's not a proper rest.");

            foreach (var member in party)
            {
                int hpRestore = member.GetTotalMaxHP() / 4;
                int staminaRestore = member.GetTotalMaxStamina() / 4;

                member.Heal(hpRestore);
                member.RestoreStamina(staminaRestore);

                Console.WriteLine($"{member.Name} recovered {hpRestore} HP and {staminaRestore} Stamina.");
            }

            Console.WriteLine("\n✨ The party feels slightly refreshed.");
            Console.ReadKey();
        }

        private void Forage(List<Character> party)
        {
            var rng = new Random();
            int success = rng.Next(100);

            Console.WriteLine("\n🌿 You search the area for edible plants and berries...");

            if (success < 60)
            {
                int healAmount = rng.Next(10, 30);
                var player = party[0];
                player.Heal(healAmount);
                Console.WriteLine($"✓ Found some berries! Restored {healAmount} HP.");
            }
            else if (success < 90)
            {
                Console.WriteLine("✗ Found nothing edible.");
            }
            else
            {
                Console.WriteLine("✗ You found some mushrooms, but they look poisonous. Better not risk it.");
            }

            Console.ReadKey();
        }

        private void CheckSurroundings()
        {
            Console.WriteLine($"\n👁️ You survey the area around {Name}...");

            string[] observations = CampType switch
            {
                CampType.Roadside => new[] {
                    "Wagon tracks indicate this is a well-traveled route.",
                    "You see old campfire rings from previous travelers.",
                    "A signpost points to distant settlements."
                },
                CampType.Forest => new[] {
                    "Birds chirp in the trees above.",
                    "Animal tracks crisscross the forest floor.",
                    "A deer watches you from a distance before bounding away."
                },
                CampType.Mountain => new[] {
                    "The wind howls through the rocky peaks.",
                    "Snow caps the distant mountains.",
                    "The air is thin and cold up here."
                },
                CampType.Desert => new[] {
                    "Sand dunes stretch endlessly in all directions.",
                    "A scorpion scuttles across the hot sand.",
                    "Heat waves shimmer on the horizon."
                },
                CampType.Riverside => new[] {
                    "The sound of flowing water is peaceful.",
                    "Fish jump in the river nearby.",
                    "Reeds sway gently along the bank."
                },
                CampType.Ruins => new[] {
                    "Ancient stone walls crumble around you.",
                    "Faded carvings hint at a forgotten civilization.",
                    "This place has an eerie, abandoned feeling."
                },
                _ => new[] { "Nothing particularly interesting catches your eye." }
            };

            var rng = new Random();
            Console.WriteLine(observations[rng.Next(observations.Length)]);
            Console.ReadKey();
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"   Camp Type: {CampType}");
            Console.WriteLine($"   Facilities: Basic rest and foraging");
        }

        private void TalkToNPCs(List<Character> party, GameState gameState)
        {
            if (gameState.NPCManager == null || gameState.Journal == null || gameState.MainStoryline == null)
            {
                Console.WriteLine("NPC system not available.");
                return;
            }

            var npcs = gameState.NPCManager.GetNPCsAtLocation(Name);

            if (npcs.Count == 0)
            {
                Console.WriteLine("\nThere's no one at this camp.");
                Console.ReadKey();
                return;
            }

            // Only one NPC at camps
            var npc = npcs[0];
            string questIndicator = npc.AvailableQuests.Count > 0 ? " (Has Quest)" : "";
            Console.WriteLine($"\n👤 You see: {npc.Name}{questIndicator}");
            Console.Write("Talk to them? (y/n): ");

            if (Console.ReadLine()?.Trim().ToLower() == "y")
            {
                npc.Interact(party, gameState.Journal, gameState.MainStoryline);
            }
        }

        #endregion
    }

    #endregion

    #region Camp Type Enum

    internal enum CampType
    {
        Roadside,   // Along main roads
        Forest,     // In wooded areas
        Mountain,   // Rocky, elevated areas
        Desert,     // Sandy, hot areas
        Riverside,  // Near water
        Ruins       // Among ancient structures
    }

    #endregion
}
