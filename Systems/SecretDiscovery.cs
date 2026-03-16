using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Manages hidden secrets, easter eggs, and rare discoveries throughout the game
    /// </summary>
    internal static class SecretDiscovery
    {
        private static readonly Random _rng = new Random();
        private static readonly HashSet<string> _discoveredSecrets = new HashSet<string>();

        #region Secret Definitions

        private static readonly List<Secret> _secrets = new List<Secret>
        {
            new Secret(
                "dev_room",
                "Developer's Secret Room",
                "You found the developer's secret room! A note reads: 'Thanks for playing! -Yogi1972'",
                party =>
                {
                    VisualEffects.WriteLineColored("🎮 DEVELOPER ROOM DISCOVERED!", ConsoleColor.Magenta);
                    Console.WriteLine();
                    VisualEffects.TypewriterEffect("'Thank you for exploring every corner of this game!'", 35);
                    Console.WriteLine();

                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        member.Inventory.AddGold(1000);
                        member.GainExperience(1000);
                    }

                    VisualEffects.WriteSuccess("🎁 Everyone received 1000 gold and 1000 XP!\n");
                },
                0.5 // Very rare
            ),

            new Secret(
                "rainbow_chest",
                "Prismatic Chest",
                "A chest glowing with rainbow colors appears!",
                party =>
                {
                    VisualEffects.ShowChestAnimation();
                    VisualEffects.PulseEffect("✨ PRISMATIC CHEST DISCOVERED! ✨",
                        new[] { ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta }, 2);

                    var leader = party.FirstOrDefault(p => p.IsAlive);
                    if (leader != null)
                    {
                        var legendary = LegendaryItemSystem.GetLegendaryForLevel(leader.Level);
                        LegendaryItemSystem.AnnounceItemFound(legendary);
                        leader.Inventory.AddItem(legendary);
                        leader.Inventory.AddGold(500);

                        VisualEffects.WriteSuccess("💰 Also found 500 gold!\n");
                    }
                },
                1.0 // Rare
            ),

            new Secret(
                "time_traveler",
                "Time Traveler Encounter",
                "A strange figure appears, claiming to be from the future...",
                party =>
                {
                    VisualEffects.WriteLineColored("⏰ TIME ANOMALY DETECTED!", ConsoleColor.Cyan);
                    VisualEffects.TypewriterEffect("'I'm from the future. I've seen your destiny...'", 35);
                    Console.WriteLine();

                    Console.WriteLine("The traveler offers you a choice:");
                    Console.WriteLine("1) Accept a powerful item from the future");
                    Console.WriteLine("2) Gain knowledge of future events (+1000 XP)");
                    Console.WriteLine("3) Ask for nothing");
                    Console.Write("Choice: ");

                    var choice = Console.ReadLine();
                    var leader = party.FirstOrDefault(p => p.IsAlive);

                    if (leader != null)
                    {
                        switch (choice)
                        {
                            case "1":
                                var legendary = LegendaryItemSystem.GetLegendaryForLevel(leader.Level + 10);
                                LegendaryItemSystem.AnnounceItemFound(legendary);
                                leader.Inventory.AddItem(legendary);
                                VisualEffects.WriteSuccess("⚡ Received a weapon from the future!\n");
                                break;
                            case "2":
                                leader.GainExperience(1000);
                                VisualEffects.WriteSuccess("🧠 Your mind expands with future knowledge!\n");
                                break;
                            case "3":
                                Console.WriteLine("The traveler smiles and fades away...");
                                Console.WriteLine("Your wisdom is rewarded.");
                                leader.GainExperience(500);
                                break;
                        }
                    }
                },
                0.3 // Very rare
            ),

            new Secret(
                "ancient_library",
                "Lost Library of Alexandria",
                "You discover the legendary lost library!",
                party =>
                {
                    VisualEffects.WriteLineColored("📚 THE LOST LIBRARY OF ALEXANDRIA!", ConsoleColor.Yellow);
                    Console.WriteLine();
                    VisualEffects.TypewriterEffect("Countless tomes of ancient wisdom surround you...", 35);
                    Console.WriteLine();

                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        int xpGain = _rng.Next(800, 1500);
                        member.GainExperience(xpGain);
                        VisualEffects.WriteSuccess($"📖 {member.Name} gained {xpGain} XP from studying!\n");

                        if (member.SkillTree != null)
                        {
                            member.SkillTree.AddSkillPoint();
                            VisualEffects.WriteLineColored($"⭐ {member.Name} also earned a bonus skill point!", ConsoleColor.Cyan);
                        }
                    }
                },
                0.5 // Very rare
            ),

            new Secret(
                "fairy_circle",
                "Fairy Circle",
                "You stumble into a circle of mushrooms where fairies dance!",
                party =>
                {
                    VisualEffects.WriteLineColored("🧚 FAIRY CIRCLE DISCOVERED!", ConsoleColor.Magenta);
                    VisualEffects.TypewriterEffect("Tiny voices whisper: 'Dance with us, mortal!'", 35);
                    Console.WriteLine();
                    Console.Write("Join the fairy dance? (y/n): ");

                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        Console.WriteLine();
                        VisualEffects.PulseEffect("✨ You dance under the moonlight! ✨",
                            new[] { ConsoleColor.Magenta, ConsoleColor.Cyan, ConsoleColor.White }, 3);

                        foreach (var member in party.Where(p => p.IsAlive))
                        {
                            VisualEffects.WriteSuccess($"🧚 {member.Name} was blessed by the fairies!\n");
                            member.Heal(member.MaxHealth);
                            member.RestoreMana(member.MaxMana);
                            member.RestoreStamina(member.MaxStamina);
                            member.Inventory.AddGold(_rng.Next(50, 200));
                        }

                        VisualEffects.WriteLineColored("The fairies gift you with their magic!", ConsoleColor.Cyan);
                    }
                    else
                    {
                        Console.WriteLine("The fairies giggle and disappear.");
                    }
                },
                1.0 // Rare
            ),

            new Secret(
                "ghost_ship",
                "Ghost Ship",
                "A spectral ship appears through the fog...",
                party =>
                {
                    VisualEffects.WriteLineColored("👻 GHOST SHIP SIGHTING!", ConsoleColor.DarkCyan);
                    VisualEffects.TypewriterEffect("The ghostly vessel beckons you aboard...", 35);
                    Console.WriteLine();
                    Console.Write("Board the ghost ship? (y/n): ");

                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        var outcome = _rng.Next(3);
                        switch (outcome)
                        {
                            case 0: // Treasure
                                VisualEffects.ShowChestAnimation();
                                var leader = party.FirstOrDefault(p => p.IsAlive);
                                if (leader != null)
                                {
                                    int gold = _rng.Next(400, 800);
                                    leader.Inventory.AddGold(gold);
                                    VisualEffects.WriteSuccess($"💰 Pirate treasure! Found {gold} gold!\n");
                                }
                                break;
                            case 1: // Cursed
                                VisualEffects.WriteDanger("💀 The ship was cursed!\n");
                                foreach (var member in party.Where(p => p.IsAlive))
                                {
                                    member.ReceiveDamage(_rng.Next(25, 50));
                                }
                                break;
                            case 2: // Ghost captain
                                Console.WriteLine("👻 The ghost captain offers you a trade...");
                                Console.WriteLine("'Take this cursed treasure, and leave!'");
                                var leader2 = party.FirstOrDefault(p => p.IsAlive);
                                if (leader2 != null)
                                {
                                    leader2.Inventory.AddGold(1000);
                                    var legendary = LegendaryItemSystem.GetLegendaryForLevel(leader2.Level);
                                    LegendaryItemSystem.AnnounceItemFound(legendary);
                                    leader2.Inventory.AddItem(legendary);
                                }
                                break;
                        }
                    }
                },
                0.8 // Very rare
            ),

            new Secret(
                "meteor_strike",
                "Meteor Shower",
                "Meteors streak across the sky!",
                party =>
                {
                    VisualEffects.WriteLineColored("☄️  METEOR SHOWER!", ConsoleColor.Yellow);
                    Console.WriteLine("Flaming rocks fall from the heavens!");

                    var outcome = _rng.Next(2);
                    if (outcome == 0) // Find meteor ore
                    {
                        VisualEffects.WriteSuccess("✨ A meteor landed nearby!\n");
                        var leader = party.FirstOrDefault(p => p.IsAlive);
                        if (leader != null)
                        {
                            Console.WriteLine("You collect rare meteor ore!");
                            leader.Inventory.AddGold(300);
                            VisualEffects.WriteSuccess("💰 Meteor ore is worth 300 gold!\n");
                        }
                    }
                    else // Take damage
                    {
                        VisualEffects.WriteDanger("💥 A meteor nearly hits you!\n");
                        var damaged = party.Where(p => p.IsAlive).OrderBy(x => _rng.Next()).First();
                        int damage = _rng.Next(20, 40);
                        damaged.ReceiveDamage(damage);
                        Console.WriteLine($"{damaged.Name} took {damage} damage!");
                    }
                },
                1.5 // Rare
            ),

            new Secret(
                "skill_shrine",
                "Shrine of Mastery",
                "An ancient shrine dedicated to martial and magical mastery!",
                party =>
                {
                    VisualEffects.WriteLineColored("⛪ SHRINE OF MASTERY!", ConsoleColor.Cyan);
                    Console.WriteLine("The shrine offers to enhance your abilities!");
                    Console.WriteLine();

                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        if (member.SkillTree != null)
                        {
                            member.SkillTree.AddSkillPoint();
                            VisualEffects.WriteSuccess($"⭐ {member.Name} gained a skill point!\n");
                        }
                    }
                },
                1.0 // Rare
            )
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Try to discover a secret during exploration
        /// </summary>
        public static void TryDiscoverSecret(List<Character> party)
        {
            // Calculate discovery chance based on party level and composition
            var avgLevel = party.Where(p => p.IsAlive).Average(p => p.Level);
            double baseChance = 2.0; // 2% base chance

            // Rogues increase secret discovery chance
            if (party.Any(p => p is Rogue && p.IsAlive))
            {
                baseChance += 1.5; // +1.5% with a rogue
            }

            if (_rng.Next(1000) < (int)(baseChance * 10))
            {
                // Filter secrets by rarity chance
                var eligibleSecrets = _secrets.Where(s =>
                    !_discoveredSecrets.Contains(s.Id) &&
                    _rng.Next(100) < (100 / s.RarityFactor)
                ).ToList();

                if (eligibleSecrets.Any())
                {
                    var secret = eligibleSecrets[_rng.Next(eligibleSecrets.Count)];
                    DiscoverSecret(secret, party);
                }
            }
        }

        private static void DiscoverSecret(Secret secret, List<Character> party)
        {
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            VisualEffects.WriteLineColored("         🔍 SECRET DISCOVERED! 🔍", ConsoleColor.Magenta);
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine();

            VisualEffects.TypewriterEffect(secret.Description, 30);
            Console.WriteLine();

            secret.Effect(party);

            _discoveredSecrets.Add(secret.Id);

            Console.WriteLine();
            VisualEffects.WriteInfo($"Secrets found: {_discoveredSecrets.Count}/{_secrets.Count}\n");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Check if player entered a secret code
        /// </summary>
        public static bool CheckSecretCode(string input, List<Character> party)
        {
            var codes = new Dictionary<string, Action>
            {
                { "konami", () =>
                {
                    VisualEffects.WriteLineColored("🎮 KONAMI CODE ACTIVATED!", ConsoleColor.Yellow);
                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        member.Inventory.AddGold(500);
                        member.GainExperience(500);
                    }
                    VisualEffects.WriteSuccess("Everyone gained 500 gold and 500 XP!\n");
                }},
                { "legend", () =>
                {
                    VisualEffects.WriteLineColored("✨ LEGENDARY CODE ACTIVATED!", ConsoleColor.Magenta);
                    var leader = party.FirstOrDefault(p => p.IsAlive);
                    if (leader != null)
                    {
                        var legendary = LegendaryItemSystem.GetLegendaryForLevel(leader.Level);
                        LegendaryItemSystem.AnnounceItemFound(legendary);
                        leader.Inventory.AddItem(legendary);
                    }
                }},
                { "skillmaster", () =>
                {
                    VisualEffects.WriteLineColored("⭐ SKILL MASTER CODE!", ConsoleColor.Cyan);
                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        if (member.SkillTree != null)
                        {
                            member.SkillTree.AddSkillPoint();
                            member.SkillTree.AddSkillPoint();
                            member.SkillTree.AddSkillPoint();
                        }
                    }
                    VisualEffects.WriteSuccess("Everyone gained 3 skill points!\n");
                }},
                { "godmode", () =>
                {
                    VisualEffects.FlashText("⚡ DIVINE POWER ACTIVATED! ⚡", ConsoleColor.Yellow, 3);
                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        member.Heal(member.MaxHealth * 2);
                        member.RestoreMana(member.MaxMana * 2);
                        member.RestoreStamina(member.MaxStamina * 2);
                    }
                    Console.WriteLine("All resources doubled!");
                }}
            };

            if (codes.TryGetValue(input.ToLower().Trim(), out var effect))
            {
                Console.WriteLine();
                effect();
                Console.WriteLine();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Show all discovered secrets
        /// </summary>
        public static void ShowDiscoveredSecrets()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            VisualEffects.WriteLineColored("║              SECRETS DISCOVERED                           ║", ConsoleColor.Magenta);
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            if (_discoveredSecrets.Count == 0)
            {
                VisualEffects.WriteInfo("No secrets discovered yet. Keep exploring!\n");
            }
            else
            {
                foreach (var secretId in _discoveredSecrets)
                {
                    var secret = _secrets.FirstOrDefault(s => s.Id == secretId);
                    if (secret != null)
                    {
                        VisualEffects.WriteLineColored($"✓ {secret.Name}", ConsoleColor.Green);
                    }
                }

                Console.WriteLine();
                VisualEffects.WriteInfo($"Progress: {_discoveredSecrets.Count}/{_secrets.Count} secrets found\n");
            }
        }

        #endregion

        #region Helper Class

        private class Secret
        {
            public string Id { get; }
            public string Name { get; }
            public string Description { get; }
            public Action<List<Character>> Effect { get; }
            public double RarityFactor { get; } // Higher = more rare

            public Secret(string id, string name, string description, Action<List<Character>> effect, double rarityFactor)
            {
                Id = id;
                Name = name;
                Description = description;
                Effect = effect;
                RarityFactor = rarityFactor;
            }
        }

        #endregion
    }
}
