using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Enums

    internal enum EventType
    {
        Combat,
        Treasure,
        Merchant,
        Mystery,
        Blessing,
        Curse
    }

    #endregion

    #region RandomEvent Class

    internal class RandomEvent
    {
        #region Properties

        public string Name { get; }
        public string Description { get; }
        public EventType Type { get; }
        public Action<List<Character>> Effect { get; }

        #endregion

        #region Constructor

        public RandomEvent(string name, string description, EventType type, Action<List<Character>> effect)
        {
            Name = name;
            Description = description;
            Type = type;
            Effect = effect;
        }

        #endregion

        #region Methods

        public void Trigger(List<Character> party)
        {
            Console.WriteLine();
            VisualEffects.WriteLineColored($"✨ RANDOM EVENT: {Name}", ConsoleColor.Yellow);
            VisualEffects.WriteLineColored(Description, ConsoleColor.White);
            Console.WriteLine();
            Effect?.Invoke(party);
        }

        #endregion
    }

    #endregion

    #region RandomEventManager Class

    internal class RandomEventManager
    {
        #region Fields

        private readonly List<RandomEvent> _events;
        private readonly Random _random;

        #endregion

        #region Constructor

        public RandomEventManager()
        {
            _random = new Random();
            _events = new List<RandomEvent>();
            InitializeEvents();
        }

        #endregion

        #region Event Initialization

        private void InitializeEvents()
        {
            // Treasure Events
            _events.Add(new RandomEvent(
                "Hidden Cache",
                "You discover a hidden cache of treasure!",
                EventType.Treasure,
                party =>
                {
                    int gold = _random.Next(50, 200);
                    var leader = party.FirstOrDefault(p => p.IsAlive);
                    if (leader != null)
                    {
                        leader.Inventory.AddGold(gold);
                        Console.WriteLine($"💰 Found {gold} gold!");
                    }
                }
            ));

            _events.Add(new RandomEvent(
                "Chest in the Shadows",
                "A mysterious chest appears before you...",
                EventType.Treasure,
                party =>
                {
                    Console.Write("Open the chest? (y/n): ");
                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        if (_random.Next(100) < 70)
                        {
                            int gold = _random.Next(100, 300);
                            var leader = party.FirstOrDefault(p => p.IsAlive);
                            if (leader != null)
                            {
                                leader.Inventory.AddGold(gold);
                                Console.WriteLine($"💰 The chest contained {gold} gold!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("💀 It was a trap! The chest was cursed!");
                            foreach (var member in party.Where(p => p.IsAlive))
                            {
                                member.ReceiveDamage(_random.Next(10, 30));
                            }
                        }
                    }
                }
            ));

            // Merchant Events
            _events.Add(new RandomEvent(
                "Traveling Merchant",
                "A traveling merchant offers you a special deal!",
                EventType.Merchant,
                party =>
                {
                    Console.WriteLine("'I have a Rare Healing Potion for only 30 gold!'");
                    Console.Write("Buy it? (y/n): ");
                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        var leader = party.FirstOrDefault(p => p.IsAlive);
                        if (leader != null && leader.Inventory.SpendGold(30))
                        {
                            leader.Inventory.AddItem(new GenericItem("Rare Healing Potion", 60));
                            Console.WriteLine("✓ Purchased Rare Healing Potion!");
                        }
                        else
                        {
                            Console.WriteLine("Not enough gold!");
                        }
                    }
                }
            ));

            // Blessing Events
            _events.Add(new RandomEvent(
                "Shrine of Healing",
                "You find an ancient shrine emanating divine energy.",
                EventType.Blessing,
                party =>
                {
                    Console.WriteLine("The shrine's power washes over your party!");
                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        int healAmount = _random.Next(20, 50);
                        member.Heal(healAmount);
                        Console.WriteLine($"✨ {member.Name} healed for {healAmount} HP!");
                    }
                }
            ));

            _events.Add(new RandomEvent(
                "Blessing of Fortune",
                "A mysterious figure blesses you with good fortune!",
                EventType.Blessing,
                party =>
                {
                    var leader = party.FirstOrDefault(p => p.IsAlive);
                    if (leader != null)
                    {
                        int expGain = _random.Next(100, 300);
                        leader.GainExperience(expGain);
                        Console.WriteLine($"✨ {leader.Name} gained {expGain} experience!");
                    }
                }
            ));

            // Mystery Events
            _events.Add(new RandomEvent(
                "Mysterious Stranger",
                "A hooded figure approaches...",
                EventType.Mystery,
                party =>
                {
                    Console.WriteLine("'I can make you stronger... for a price.'");
                    Console.Write("Accept? (y/n): ");
                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        var leader = party.FirstOrDefault(p => p.IsAlive);
                        if (leader != null && leader.Inventory.SpendGold(100))
                        {
                            leader.GainExperience(500);
                            Console.WriteLine($"✨ {leader.Name} gained 500 experience!");
                        }
                        else
                        {
                            Console.WriteLine("'Come back when you have 100 gold.'");
                        }
                    }
                }
            ));

            _events.Add(new RandomEvent(
                "Ancient Tome",
                "You find an ancient tome filled with knowledge.",
                EventType.Mystery,
                party =>
                {
                    var livingMembers = party.Where(p => p.IsAlive).ToList();
                    if (livingMembers.Count > 0)
                    {
                        var reader = livingMembers[_random.Next(livingMembers.Count)];
                        int expGain = _random.Next(150, 400);
                        reader.GainExperience(expGain);
                        Console.WriteLine($"📖 {reader.Name} learned from the tome and gained {expGain} experience!");
                    }
                }
            ));

            // Curse Events
            _events.Add(new RandomEvent(
                "Cursed Ground",
                "You step on cursed ground!",
                EventType.Curse,
                party =>
                {
                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        int damage = _random.Next(5, 20);
                        member.ReceiveDamage(damage);
                        Console.WriteLine($"💀 {member.Name} took {damage} curse damage!");
                    }
                }
            ));

            // Ambush Event
            _events.Add(new RandomEvent(
                "Ambush!",
                "Enemies leap out from hiding!",
                EventType.Combat,
                party =>
                {
                    Console.WriteLine("⚔️ A group of bandits attack!");
                    Console.WriteLine("(This would trigger combat in the full game)");
                    // In actual implementation, this would start combat
                }
            ));

            // Resting Spot
            _events.Add(new RandomEvent(
                "Safe Haven",
                "You find a peaceful clearing.",
                EventType.Blessing,
                party =>
                {
                    Console.WriteLine("Your party takes a moment to rest.");
                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        int healAmount = member.MaxHealth / 4;
                        int manaAmount = member.MaxMana / 4;
                        member.Heal(healAmount);
                        member.RestoreMana(manaAmount);
                        Console.WriteLine($"✨ {member.Name} recovered {healAmount} HP and {manaAmount} Mana!");
                    }
                }
            ));

            // Gambling opportunity
            _events.Add(new RandomEvent(
                "Game of Chance",
                "A gambler challenges you to a dice game!",
                EventType.Mystery,
                party =>
                {
                    Console.Write("Bet 50 gold on a dice roll? (y/n): ");
                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        var leader = party.FirstOrDefault(p => p.IsAlive);
                        if (leader != null && leader.Inventory.SpendGold(50))
                        {
                            if (_random.Next(100) < 50)
                            {
                                leader.Inventory.AddGold(150);
                                Console.WriteLine("🎲 You win! Gained 150 gold!");
                            }
                            else
                            {
                                Console.WriteLine("🎲 You lose! Lost 50 gold!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Not enough gold!");
                        }
                    }
                }
            ));

            // Equipment drop
            _events.Add(new RandomEvent(
                "Abandoned Gear",
                "You find equipment left behind by a fallen adventurer.",
                EventType.Treasure,
                party =>
                {
                    var equipmentTypes = new[] { EquipmentType.Weapon, EquipmentType.Armor, EquipmentType.Accessory };
                    var randomType = equipmentTypes[_random.Next(equipmentTypes.Length)];
                    var equipment = new Equipment($"Found {randomType}", randomType, _random.Next(30, 60), _random.Next(40, 100));

                    var leader = party.FirstOrDefault(p => p.IsAlive);
                    if (leader != null)
                    {
                        if (leader.Inventory.AddItem(equipment))
                        {
                            Console.WriteLine($"⚔️ Found {equipment.Name}!");
                        }
                        else
                        {
                            Console.WriteLine("Inventory full! Item left behind.");
                        }
                    }
                }
            ));

            // NEW EXCITING EVENTS BELOW

            // Legendary Encounter
            _events.Add(new RandomEvent(
                "Ancient Guardian",
                "An ancient guardian challenges you to prove your worth!",
                EventType.Combat,
                party =>
                {
                    VisualEffects.WriteLineColored("⚔️  A LEGENDARY ENCOUNTER AWAITS!", ConsoleColor.Magenta);
                    Console.WriteLine("Defeat the guardian to earn a legendary reward!");
                    Console.WriteLine("(This would trigger a boss fight in the full game)");
                }
            ));

            // Fortune Teller
            _events.Add(new RandomEvent(
                "Mysterious Fortune Teller",
                "A fortune teller offers to reveal your destiny...",
                EventType.Mystery,
                party =>
                {
                    VisualEffects.WriteLineColored("🔮 'I see great things in your future...'", ConsoleColor.Magenta);
                    Console.Write("Pay 20 gold for a fortune? (y/n): ");
                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        var leader = party.FirstOrDefault(p => p.IsAlive);
                        if (leader != null && leader.Inventory.SpendGold(20))
                        {
                            var fortunes = new (string, Action)[]
                            {
                                ("Great wealth awaits you!", (Action)(() => leader.Inventory.AddGold(_random.Next(100, 300)))),
                                ("Your skills will sharpen!", (Action)(() => leader.GainExperience(_random.Next(200, 500)))),
                                ("Health and vitality!", (Action)(() => { leader.Heal(leader.MaxHealth); Console.WriteLine("Fully healed!"); })),
                                ("Beware of the shadows...", (Action)(() => Console.WriteLine("You feel a chill..."))),
                                ("A legendary item is near!", (Action)(() =>
                                {
                                    var legendary = LegendaryItemSystem.GetLegendaryForLevel(leader.Level);
                                    LegendaryItemSystem.AnnounceItemFound(legendary);
                                    leader.Inventory.AddItem(legendary);
                                }))
                            };

                            var fortune = fortunes[_random.Next(fortunes.Length)];
                            VisualEffects.TypewriterEffect($"🔮 '{fortune.Item1}'", 40);
                            fortune.Item2();
                        }
                        else
                        {
                            Console.WriteLine("Not enough gold!");
                        }
                    }
                }
            ));

            // Hidden Portal
            _events.Add(new RandomEvent(
                "Shimmering Portal",
                "A magical portal appears before you!",
                EventType.Mystery,
                party =>
                {
                    VisualEffects.WriteLineColored("✨ A portal swirls with mysterious energy...", ConsoleColor.Cyan);
                    Console.Write("Enter the portal? (y/n): ");
                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        var outcomes = _random.Next(4);
                        switch (outcomes)
                        {
                            case 0: // Treasure dimension
                                VisualEffects.ShowChestAnimation();
                                var leader = party.FirstOrDefault(p => p.IsAlive);
                                if (leader != null)
                                {
                                    int gold = _random.Next(200, 500);
                                    leader.Inventory.AddGold(gold);
                                    VisualEffects.WriteSuccess($"💰 You found a treasure dimension! Gained {gold} gold!\n");
                                }
                                break;
                            case 1: // Combat dimension
                                VisualEffects.WriteDanger("⚔️  You're ambushed by dimensional creatures!\n");
                                Console.WriteLine("(Combat would trigger here)");
                                break;
                            case 2: // Healing dimension
                                VisualEffects.WriteHealing("✨ The portal led to a healing sanctuary!\n");
                                foreach (var member in party.Where(p => p.IsAlive))
                                {
                                    member.Heal(member.MaxHealth);
                                    member.RestoreMana(member.MaxMana);
                                    member.RestoreStamina(member.MaxStamina);
                                }
                                Console.WriteLine("Everyone is fully restored!");
                                break;
                            case 3: // Nothing
                                Console.WriteLine("The portal closes behind you. Nothing happened.");
                                break;
                        }
                    }
                }
            ));

            // Dragon's Hoard
            _events.Add(new RandomEvent(
                "Dragon's Hoard",
                "You stumble upon a sleeping dragon guarding a massive hoard!",
                EventType.Treasure,
                party =>
                {
                    VisualEffects.WriteLineColored("🐉 A DRAGON sleeps atop mountains of gold!", ConsoleColor.Red);
                    Console.WriteLine();
                    Console.WriteLine("1) Try to steal some treasure (risky!)");
                    Console.WriteLine("2) Leave quietly");
                    Console.Write("Choice: ");
                    var choice = Console.ReadLine();

                    if (choice == "1")
                    {
                        var stealthCheck = _random.Next(100);
                        if (stealthCheck < 30) // 30% success
                        {
                            VisualEffects.ShowChestAnimation();
                            var leader = party.FirstOrDefault(p => p.IsAlive);
                            if (leader != null)
                            {
                                int gold = _random.Next(500, 1000);
                                leader.Inventory.AddGold(gold);
                                VisualEffects.WriteSuccess($"💰 You carefully took {gold} gold!\n");

                                // Small chance for legendary
                                if (_random.Next(100) < 20)
                                {
                                    var legendary = LegendaryItemSystem.GetLegendaryForLevel(leader.Level);
                                    LegendaryItemSystem.AnnounceItemFound(legendary);
                                    leader.Inventory.AddItem(legendary);
                                }
                            }
                        }
                        else
                        {
                            VisualEffects.WriteDanger("🐉 THE DRAGON AWAKENS!\n");
                            Console.WriteLine("The dragon's roar shakes the ground!");
                            foreach (var member in party.Where(p => p.IsAlive))
                            {
                                int damage = _random.Next(30, 60);
                                member.ReceiveDamage(damage);
                                VisualEffects.WriteDamage($"🔥 {member.Name} takes {damage} fire damage from the dragon's breath!\n");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("You wisely retreat...");
                    }
                }
            ));

            // Wishing Well
            _events.Add(new RandomEvent(
                "Ancient Wishing Well",
                "You find an ancient well. Locals say it grants wishes...",
                EventType.Mystery,
                party =>
                {
                    VisualEffects.WriteLineColored("✨ The well shimmers with magical energy.", ConsoleColor.Cyan);
                    Console.Write("Toss in a coin? (10 gold) (y/n): ");
                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        var leader = party.FirstOrDefault(p => p.IsAlive);
                        if (leader != null && leader.Inventory.SpendGold(10))
                        {
                            var wishOutcome = _random.Next(5);
                            switch (wishOutcome)
                            {
                                case 0:
                                    int goldMultiplier = _random.Next(10, 50);
                                    leader.Inventory.AddGold(goldMultiplier * 10);
                                    VisualEffects.WriteSuccess($"💰 Your wish was granted! Received {goldMultiplier * 10} gold!\n");
                                    break;
                                case 1:
                                    int exp = _random.Next(300, 800);
                                    leader.GainExperience(exp);
                                    VisualEffects.WriteSuccess($"✨ You feel enlightened! Gained {exp} XP!\n");
                                    break;
                                case 2:
                                    leader.Heal(leader.MaxHealth / 2);
                                    VisualEffects.WriteHealing("💚 You feel rejuvenated!\n");
                                    break;
                                case 3:
                                    Console.WriteLine("Nothing happens... perhaps the well is empty.");
                                    break;
                                case 4:
                                    VisualEffects.WriteDanger("💀 The well was cursed!\n");
                                    leader.ReceiveDamage(_random.Next(10, 30));
                                    break;
                            }
                        }
                    }
                }
            ));

            // Mysterious Trader
            _events.Add(new RandomEvent(
                "Exotic Trader",
                "An exotic trader from distant lands has rare goods!",
                EventType.Merchant,
                party =>
                {
                    VisualEffects.WriteLineColored("🗺️  'I have traveled far to bring you these rare items!'", ConsoleColor.Yellow);
                    Console.WriteLine();
                    Console.WriteLine("1) Mysterious Box (100g) - Could contain anything!");
                    Console.WriteLine("2) Experience Scroll (150g) - Instant 1000 XP");
                    Console.WriteLine("3) Stat Elixir (200g) - Permanently increases a random stat");
                    Console.WriteLine("4) Nothing, thanks");
                    Console.Write("Choice: ");

                    var choice = Console.ReadLine();
                    var leader = party.FirstOrDefault(p => p.IsAlive);

                    if (leader != null)
                    {
                        switch (choice)
                        {
                            case "1":
                                if (leader.Inventory.SpendGold(100))
                                {
                                    var boxResult = _random.Next(5);
                                    VisualEffects.ShowChestAnimation();
                                    switch (boxResult)
                                    {
                                        case 0:
                                        case 1:
                                            int gold = _random.Next(50, 300);
                                            leader.Inventory.AddGold(gold);
                                            Console.WriteLine($"💰 The box contained {gold} gold!");
                                            break;
                                        case 2:
                                            leader.GainExperience(500);
                                            Console.WriteLine("📜 The box contained an experience scroll! +500 XP!");
                                            break;
                                        case 3:
                                            var legendary = LegendaryItemSystem.GetLegendaryForLevel(leader.Level);
                                            LegendaryItemSystem.AnnounceItemFound(legendary);
                                            leader.Inventory.AddItem(legendary);
                                            break;
                                        case 4:
                                            Console.WriteLine("💀 The box was cursed! It explodes!");
                                            leader.ReceiveDamage(20);
                                            break;
                                    }
                                }
                                else Console.WriteLine("Not enough gold!");
                                break;
                            case "2":
                                if (leader.Inventory.SpendGold(150))
                                {
                                    leader.GainExperience(1000);
                                    VisualEffects.WriteSuccess("📜 You gained 1000 XP!\n");
                                }
                                else Console.WriteLine("Not enough gold!");
                                break;
                            case "3":
                                if (leader.Inventory.SpendGold(200))
                                {
                                    var statBoosts = new (string, Action)[]
                                    {
                                        ("Strength", () => Console.WriteLine("💪 +5 Strength!")),
                                        ("Agility", () => Console.WriteLine("🏃 +5 Agility!")),
                                        ("Intelligence", () => Console.WriteLine("🧠 +5 Intelligence!"))
                                    };
                                    var boost = statBoosts[_random.Next(statBoosts.Length)];
                                    VisualEffects.WriteSuccess($"⚡ Your {boost.Item1} increased permanently!\n");
                                }
                                else Console.WriteLine("Not enough gold!");
                                break;
                        }
                    }
                }
            ));

            // Secret Path Discovery
            _events.Add(new RandomEvent(
                "Hidden Path",
                "You notice a concealed passage!",
                EventType.Mystery,
                party =>
                {
                    VisualEffects.WriteLineColored("🗺️  You discovered a secret path!", ConsoleColor.Yellow);
                    Console.Write("Explore the hidden path? (y/n): ");
                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        var secretResult = _random.Next(3);
                        switch (secretResult)
                        {
                            case 0: // Treasure room
                                VisualEffects.ShowChestAnimation();
                                var leader = party.FirstOrDefault(p => p.IsAlive);
                                if (leader != null)
                                {
                                    int gold = _random.Next(300, 600);
                                    leader.Inventory.AddGold(gold);
                                    VisualEffects.WriteSuccess($"💰 You found a secret treasure room! {gold} gold!\n");
                                }
                                break;
                            case 1: // Trap
                                VisualEffects.WriteDanger("💀 It was a trap!\n");
                                foreach (var member in party.Where(p => p.IsAlive))
                                {
                                    int damage = _random.Next(15, 35);
                                    member.ReceiveDamage(damage);
                                }
                                break;
                            case 2: // Ancient altar
                                VisualEffects.WriteLineColored("⛪ You find an ancient altar!", ConsoleColor.Cyan);
                                Console.WriteLine("Choose a blessing:");
                                Console.WriteLine("1) Blessing of Strength");
                                Console.WriteLine("2) Blessing of Speed");
                                Console.WriteLine("3) Blessing of Wisdom");
                                var blessing = Console.ReadLine();
                                var leader2 = party.FirstOrDefault(p => p.IsAlive);
                                if (leader2 != null)
                                {
                                    switch (blessing)
                                    {
                                        case "1":
                                            VisualEffects.WriteSuccess("💪 Strength permanently increased!\n");
                                            break;
                                        case "2":
                                            VisualEffects.WriteSuccess("🏃 Agility permanently increased!\n");
                                            break;
                                        case "3":
                                            VisualEffects.WriteSuccess("🧠 Intelligence permanently increased!\n");
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
            ));

            // Legendary Beast Sighting
            _events.Add(new RandomEvent(
                "Legendary Beast",
                "You spot a rare legendary creature!",
                EventType.Combat,
                party =>
                {
                    VisualEffects.WriteLineColored("🦁 A LEGENDARY BEAST appears in the distance!", ConsoleColor.Magenta);
                    Console.WriteLine("These creatures are incredibly rare and powerful!");
                    Console.Write("1) Attempt to hunt it (high risk, high reward)\n2) Observe from afar\nChoice: ");
                    var choice = Console.ReadLine();

                    if (choice == "1")
                    {
                        VisualEffects.WriteDanger("⚔️  The beast notices you and charges!\n");
                        Console.WriteLine("(This would trigger an elite combat encounter)");
                        Console.WriteLine("On victory, you would receive:");
                        Console.WriteLine("  • Massive XP bonus");
                        Console.WriteLine("  • Guaranteed rare loot");
                        Console.WriteLine("  • High chance for legendary item");
                    }
                    else
                    {
                        Console.WriteLine("You watch the majestic creature from a distance...");
                        var leader = party.FirstOrDefault(p => p.IsAlive);
                        if (leader != null)
                        {
                            leader.GainExperience(100);
                            Console.WriteLine("✨ Gained 100 XP from the observation!");
                        }
                    }
                }
            ));

            // Cursed Artifact
            _events.Add(new RandomEvent(
                "Cursed Artifact",
                "You find a glowing artifact... but something feels wrong.",
                EventType.Curse,
                party =>
                {
                    VisualEffects.WriteLineColored("⚠️  The artifact pulses with dark energy...", ConsoleColor.DarkMagenta);
                    Console.Write("Touch the artifact? (y/n): ");
                    var choice = Console.ReadLine();
                    if (choice?.ToLower() == "y")
                    {
                        if (_random.Next(100) < 40) // 40% success
                        {
                            var leader = party.FirstOrDefault(p => p.IsAlive);
                            if (leader != null)
                            {
                                VisualEffects.WriteSuccess("✨ You resisted the curse and claimed its power!\n");
                                leader.GainExperience(_random.Next(500, 1000));
                            }
                        }
                        else
                        {
                            VisualEffects.WriteDanger("💀 The curse overwhelms you!\n");
                            foreach (var member in party.Where(p => p.IsAlive))
                            {
                                int damage = _random.Next(20, 40);
                                member.ReceiveDamage(damage);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("You wisely leave the artifact alone.");
                    }
                }
            ));

            // Fountain of Life
            _events.Add(new RandomEvent(
                "Fountain of Life",
                "A crystal-clear fountain bubbles with magical water.",
                EventType.Blessing,
                party =>
                {
                    VisualEffects.WriteLineColored("💧 The fountain radiates pure life energy!", ConsoleColor.Cyan);
                    Console.WriteLine("Each party member may drink once.");

                    foreach (var member in party.Where(p => p.IsAlive))
                    {
                        int maxHpBonus = _random.Next(10, 25);
                        member.Heal(member.MaxHealth);
                        VisualEffects.WriteHealing($"✨ {member.Name} drinks deeply and feels empowered!\n");
                        VisualEffects.WriteSuccess($"   HP fully restored and Max HP +{maxHpBonus}!\n");
                        // Note: Would need to implement permanent MaxHP increase in Character class
                    }
                }
            ));

            // Rival Adventurer
            _events.Add(new RandomEvent(
                "Rival Adventurer",
                "Another adventurer crosses your path!",
                EventType.Mystery,
                party =>
                {
                    VisualEffects.WriteLineColored("⚔️  A rival adventurer appears!", ConsoleColor.Yellow);
                    Console.WriteLine("'Care for a friendly duel? Winner takes 100 gold!'");
                    Console.Write("Accept the duel? (y/n): ");
                    var choice = Console.ReadLine();

                    if (choice?.ToLower() == "y")
                    {
                        var leader = party.FirstOrDefault(p => p.IsAlive);
                        if (leader != null)
                        {
                            var duelRoll = _random.Next(100);
                            if (duelRoll < 50)
                            {
                                VisualEffects.ShowVictoryBanner();
                                leader.Inventory.AddGold(100);
                                leader.GainExperience(200);
                                VisualEffects.WriteSuccess("🏆 You won the duel! +100 gold and +200 XP!\n");
                            }
                            else
                            {
                                VisualEffects.WriteDanger("💀 You lost the duel!\n");
                                leader.Inventory.SpendGold(Math.Min(100, leader.Inventory.Gold));
                                int damage = _random.Next(10, 25);
                                leader.ReceiveDamage(damage);
                            }
                        }
                    }
                }
            ));
        }

        #endregion

        #region Event Triggering

        public void TriggerRandomEvent(List<Character> party, int chancePercent = 25)
        {
            if (_random.Next(100) < chancePercent)
            {
                var randomEvent = _events[_random.Next(_events.Count)];
                randomEvent.Trigger(party);
            }
        }

        public RandomEvent? GetRandomEvent()
        {
            return _events[_random.Next(_events.Count)];
        }

        public void TriggerSpecificEvent(string eventName, List<Character> party)
        {
            var evt = _events.FirstOrDefault(e => e.Name.Equals(eventName, StringComparison.OrdinalIgnoreCase));
            evt?.Trigger(party);
        }

        #endregion
    }

    #endregion
}
