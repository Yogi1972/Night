using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    #region World Lore Provider

    /// <summary>
    /// Centralized repository of background lore for the world of Night.
    /// Provides history, legends, and cultural details for towns, settlements, races, and enemy factions.
    /// </summary>
    internal static class WorldLore
    {
        #region Town Lore

        private static readonly Dictionary<string, string[]> TownLoreEntries = new()
        {
            ["Havenbrook"] =
            [
                "Founded three centuries ago by the merchant prince Aldric Haven, Havenbrook began as a humble waystation at the crossroads of the King's Road and the Silverthread Trail.",
                "After the Collapse of the Old Empire, refugees from every corner of the realm converged here, transforming a roadside inn into one of the largest trade cities in the known world.",
                "The city's famous Accord of Open Commerce guarantees safe passage and fair dealing to all races—human, elf, dwarf, and gnome alike—making it a rare haven of cooperation in troubled times.",
                "Legends say the original hearthstone of Aldric's inn still burns beneath the city's Grand Market, and that as long as its flame endures, Havenbrook will never fall to siege."
            ],
            ["Ironforge Citadel"] =
            [
                "Carved into the heart of Mount Anvilar by the Stoneborn dwarven clan over a thousand years ago, Ironforge Citadel is the undisputed capital of metalwork and arms-crafting.",
                "The Great Forge at the city's core burns with dragonfire captured during the Second Dragon War, producing steel of unmatched quality known as 'Anvilar-tempered' across the realm.",
                "During the Orc Siege of the Iron Pass, the citadel held firm for seven years, its defenders sustained by underground springs and mushroom farms that still feed the city today.",
                "The Hammer Council, a group of seven master smiths, governs the citadel. Their decree: 'No blade forged in Ironforge shall be denied to those who defend the innocent.'"
            ],
            ["Mysthaven"] =
            [
                "Mysthaven was discovered—not founded—by a circle of storm mages who followed arcane currents to a coastal bay perpetually shrouded in luminous mist.",
                "The mist itself is believed to be residual magical energy from the Sundering, a cataclysmic event that shattered the continent millennia ago.",
                "The Arcane Academy of Mysthaven is the premier institution for magical study, drawing apprentices from every race. Its library, the Veil Archive, contains texts written in languages no living scholar can fully translate.",
                "Sailors fear the approach to Mysthaven's harbor; without a mage-guided beacon, ships are lost in the enchanted fog, joining the Ghost Fleet that drifts endlessly beneath the waves."
            ],
            ["Sunspire"] =
            [
                "Rising like a golden needle from the heart of the Ember Desert, Sunspire was built atop the ruins of a pre-Sundering civilization whose name has been lost to history.",
                "The city's wealth comes from the Glassbloom Oasis, where rare alchemical plants grow in soil infused with ancient magic, producing ingredients found nowhere else in the world.",
                "Sunspire's ruler, the Sun Regent, is chosen not by bloodline but by a trial called the Walk of Flames—a grueling trek across the desert that tests endurance, wit, and character.",
                "Beneath the city, treasure hunters still explore the Gilded Catacombs, discovering relics of the lost civilization. Many who enter never return, claimed by traps or the guardians that still patrol the depths."
            ],
            ["Shadowkeep"] =
            [
                "Once the fortress of the Dread Lord Morvath, Shadowkeep was liberated in the War of Dawn by an alliance of champions from every race and class.",
                "Rather than destroy the dark fortress, the victors chose to repurpose it as the world's premier training ground for elite warriors, mages, and rogues.",
                "The Keep's infamous Shadow Trials push combatants to their absolute limits, using enchanted constructs and illusion magic to simulate encounters with the world's deadliest threats.",
                "The spirit of Morvath is rumored to still haunt the lowest dungeon. Veterans claim that on moonless nights, his laughter echoes through the halls, testing the courage of even the bravest."
            ],
            ["Crystalshore"] =
            [
                "Crystalshore sits where the Prismatic Reef meets the mainland, a place where the tides wash ashore gemstones polished by centuries of wave action.",
                "The city's architecture incorporates living crystal that grows slowly over decades, giving buildings a shimmering, organic appearance that changes color with the light.",
                "Gnome jewelers first settled here, drawn by the natural gems, and their techniques for crystal-singing—shaping gems with harmonics—remain closely guarded trade secrets.",
                "Every decade, the Crystal Tide brings a surge of rare deep-sea gems to the shore. The event draws merchants, adventurers, and thieves from across the realm."
            ],
            ["Emberpeak"] =
            [
                "Built into the caldera of the semi-active volcano Mount Pyrus, Emberpeak harnesses geothermal vents to power alchemical furnaces of extraordinary temperature.",
                "The Flame Alchemists' Guild, headquartered here, developed techniques for infusing potions with elemental fire, creating concoctions that can heal, harm, or transform in ways conventional alchemy cannot.",
                "The city's founding is attributed to a mad gnome inventor named Sparks Gearbolt, who declared: 'If we can't outrun the lava, we'll make it work for us!'",
                "Emberpeak's greatest danger is the Heartbeat—irregular volcanic tremors that threaten to collapse tunnels. The city employs a permanent corps of earth mages to reinforce its foundations."
            ],
            ["Stormwatch"] =
            [
                "Perched on the Galebreaker Cliffs, Stormwatch was founded by an order of storm mages who believed that lightning was the voice of the gods.",
                "The city's Storm Towers harness lightning strikes to power enchantments, charge magical artifacts, and fuel the great beacon that guides ships through the treacherous Shatter Straits.",
                "Stormwatch's navigators are legendary—their storm-reading abilities allow ships under their guidance to sail routes no ordinary captain would dare attempt.",
                "The Tempest Arena, an open-air colosseum atop the highest cliff, hosts magical duels during thunderstorms. Combatants channel the storm itself, making each battle a spectacle of raw elemental power."
            ],
            ["Frostholm"] =
            [
                "Nestled in the Everwinter Valley, Frostholm endures temperatures that would kill an unprotected traveler in hours. Its people are among the hardiest in the world.",
                "The city's ice-crafters sculpt structures from enchanted permafrost that never melts, creating buildings of breathtaking beauty that shimmer with captured starlight.",
                "Frostholm's founding legend tells of Sigrid Winterborn, a human warrior who wrestled a frost giant for three days to claim the valley for her people.",
                "The annual Festival of the Long Night celebrates the winter solstice with ice sculptures, frost magic displays, and the traditional Boar's Feast that lasts seven days."
            ],
            ["Goldenvale"] =
            [
                "Surrounded by the Golden Fields—vast plains of enchanted wheat that gleam like precious metal in the sun—Goldenvale is the breadbasket of the realm.",
                "The soil here was blessed centuries ago by a grateful earth spirit, and the harvests have never failed, even in the harshest droughts.",
                "Goldenvale's breweries produce the famous Sunlight Ale, a golden brew said to cure homesickness and lift the spirits of even the most battle-weary adventurer.",
                "The Harvest Council, made up of the eldest farmers, governs the city with a gentle hand. Their motto: 'A full belly makes a peaceful heart.'"
            ],
            ["Deepstone"] =
            [
                "Miles beneath the surface, Deepstone is a dwarven metropolis of immense caverns lit by bioluminescent fungi and crystal veins that pulse with a soft inner glow.",
                "The city was founded when miners broke through into the Luminous Caverns, a natural wonder of crystal formations that the dwarves consider sacred.",
                "Deepstone's gem markets are the finest in the world. The legendary Starfall Diamonds, found only in the deepest shafts, are said to contain captured starlight from before the Sundering.",
                "The city observes strict mining codes: for every vein depleted, the dwarves plant crystal seeds—a closely guarded technique that ensures the caverns slowly regenerate over centuries."
            ],
            ["Skyreach"] =
            [
                "Skyreach defies explanation—a city built on massive stone islands that float a thousand feet above the Windswept Plains, connected by rope bridges and wind-current ferries.",
                "The floating islands are remnants of the Sundering, when the world's magical upheaval tore chunks of earth skyward. Wind mages stabilized them and founded the city.",
                "Skyreach's Wind Riders, elite warriors mounted on giant eagles, serve as both the city's defenders and its messengers, carrying news across the realm faster than any ground courier.",
                "The greatest mystery of Skyreach is the Anchor Stone at its core—an artifact of unknown origin that maintains the islands' levitation. If it were ever destroyed, the city would fall."
            ]
        };

        /// <summary>
        /// Gets lore paragraphs for a major town by name.
        /// </summary>
        public static string[] GetTownLore(string townName)
        {
            return TownLoreEntries.TryGetValue(townName, out var lore)
                ? lore
                : [$"{townName} is a place of mystery whose history has yet to be fully uncovered by scholars."];
        }

        /// <summary>
        /// Displays formatted lore for a major town.
        /// </summary>
        public static void DisplayTownLore(string townName)
        {
            var lore = GetTownLore(townName);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 History of {townName,-45}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            foreach (var paragraph in lore)
            {
                Console.WriteLine();
                WriteWrapped(paragraph, 60);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Settlement Lore

        private static readonly Dictionary<string, string[]> SettlementLoreEntries = new()
        {
            ["Willowdale"] =
            [
                "Willowdale was settled by a group of retired soldiers who wanted nothing more than to farm in peace after the War of Dawn.",
                "The great willow tree at the village center is said to have been planted by an elven druid as a blessing, and it has never lost its leaves—even in the deepest winter."
            ],
            ["Crossroads Keep"] =
            [
                "Originally a military outpost guarding the junction of three ancient roads, Crossroads Keep evolved into a settlement when merchants began camping permanently outside its walls.",
                "The Keep's watchtower still bears scorch marks from the Night of Fire, when a band of cultists attempted to seize control of the crossroads to disrupt trade across the realm."
            ],
            ["Pinewood"] =
            [
                "The loggers of Pinewood have an ancient pact with the forest spirits: for every tree felled, two saplings are planted and blessed. The forest has thrived under this agreement.",
                "Deep within the woods near Pinewood, hunters sometimes find carved stones in an unknown script—remnants of a civilization that existed long before humans arrived."
            ],
            ["Riverside"] =
            [
                "Riverside's fisherfolk claim the Great River is home to a benevolent water spirit who guides their nets to the richest catches in exchange for a yearly offering of song and flowers.",
                "The village was nearly destroyed by a flood fifty years ago, but a wandering gnome engineer designed a system of levees and channels that saved it, earning the gratitude of every family."
            ],
            ["Stonebridge"] =
            [
                "The ancient stone bridge that gives this settlement its name predates all known civilizations. Its stones are warm to the touch, and no amount of weathering has eroded its surface.",
                "Scholars believe the bridge was built by the same lost race that created Sunspire's catacombs. Strange runes on its underside glow faintly during a full moon."
            ],
            ["Frosthollow"] =
            [
                "Frosthollow's residents are descendants of Frostholm explorers who chose to settle in the lower mountain passes rather than endure the extreme cold of the Everwinter Valley.",
                "The settlement's hot springs are renowned for their healing properties, drawing travelers from across the mountains to soak in waters warmed by deep volcanic activity."
            ],
            ["Oasis Rest"] =
            [
                "Oasis Rest exists because of a single, seemingly inexhaustible spring that has never run dry—even when surrounding wells turned to dust during the Great Drought.",
                "Desert nomads consider this oasis sacred ground where no blood may be shed. Even bitter enemies observe a truce within its boundaries."
            ],
            ["Moonwell"] =
            [
                "The magical spring at Moonwell's heart glows with pale silver light during the full moon, and water drawn at that time is said to have minor healing and restorative properties.",
                "Elven pilgrims travel great distances to visit Moonwell, believing it to be a place where the barrier between the mortal world and the spirit realm grows thin."
            ],
            ["Thornwall"] =
            [
                "Thornwall earned its name from the massive barrier of enchanted briars that surrounds the settlement—cultivated by a druid who sacrificed his life to protect the frontier villagers from marauding orcs.",
                "Life on the frontier is harsh, and Thornwall's people are fiercely independent. They trust no army to defend them and train every citizen, regardless of age, in basic combat."
            ],
            ["Ghostlight"] =
            [
                "Ghostlight sits on the border of the Haunted Marches, a region blighted by necromantic energy from an ancient battle. At night, pale lights drift through the streets—harmless but unsettling.",
                "The settlement's residents have developed a unique resistance to undead influence, and many of the realm's most skilled ghost hunters and exorcists hail from this eerie village."
            ],
            ["Silvermist"] =
            [
                "Silvermist's perpetual coastal fog is caused by the meeting of warm southern currents and cold northern waters. Sailors call the village 'the Last Beacon' for its guiding lighthouse.",
                "The tavern at Silvermist, the Drowned Sailor, is famous for the tall tales shared by visiting mariners—and for the surprising number of those tales that turn out to be true."
            ],
            ["Copperhill"] =
            [
                "Copperhill's mines produce not only copper but a rare alloy called starcopper, which has natural resistance to magical corrosion and is prized by enchantment smiths.",
                "Dwarven prospectors discovered the veins here, but the settlement is now home to all races. The annual Copper Festival celebrates this diversity with music, food, and crafting competitions."
            ],
            ["Ravencrest"] =
            [
                "Ravencrest was founded by a disgraced knight who built a fortified home on the rocky outcrop as a final stand against the bandits who had driven her from her lands.",
                "The ravens that nest on the settlement's cliffs are unnaturally intelligent. Locals train them as messengers, and some swear the birds understand spoken language."
            ],
            ["Bramblewood"] =
            [
                "Bramblewood is nearly invisible from the outside—its buildings are woven into the thorny hedge that surrounds it, making the entire village look like part of the forest.",
                "The settlement was originally a hideout for refugees during the Orc Wars. Even now, its people maintain the tradition of concealment, and finding Bramblewood without a guide is nearly impossible."
            ],
            ["Saltmarsh"] =
            [
                "Built on stilts above the swamp, Saltmarsh is a settlement of trappers, herbalists, and outcasts who prefer the marshland's solitude to the bustle of civilization.",
                "The rare marsh lotus that grows here is an essential ingredient in high-level healing potions. Apothecaries pay handsomely for fresh specimens."
            ],
            ["Windmill Crossing"] =
            [
                "Windmill Crossing is powered entirely by a series of massive windmills engineered by a brilliant gnome inventor who retired here after a career of adventuring.",
                "The settlement's grain mills produce flour of exceptional quality, and its wind-powered workshops craft everything from rope to furniture, all without a single forge fire."
            ]
        };

        /// <summary>
        /// Gets lore paragraphs for a settlement by name.
        /// </summary>
        public static string[] GetSettlementLore(string settlementName)
        {
            return SettlementLoreEntries.TryGetValue(settlementName, out var lore)
                ? lore
                : [$"Little is known about {settlementName}, though the locals speak of a rich history passed down through oral tradition."];
        }

        /// <summary>
        /// Displays formatted lore for a settlement.
        /// </summary>
        public static void DisplaySettlementLore(string settlementName)
        {
            var lore = GetSettlementLore(settlementName);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 Lore of {settlementName,-48}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            foreach (var paragraph in lore)
            {
                Console.WriteLine();
                WriteWrapped(paragraph, 60);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Race Lore

        private static readonly Dictionary<string, string[]> RaceLoreEntries = new()
        {
            ["Human"] =
            [
                "Humans are the most numerous and adaptable of the civilized races. Though they lack the longevity of elves or the resilience of dwarves, their ambition and versatility have allowed them to thrive in every environment.",
                "The human kingdoms were shattered during the Sundering, but their people rebuilt faster than any other race, founding new cities and forging new alliances within a single generation.",
                "Humans are known for their diplomatic skill and cultural flexibility. The Accord of Open Commerce in Havenbrook—a human creation—is credited with maintaining peace between the races for over a century.",
                "Human warriors, mages, and rogues are found in every corner of the world. What they lack in innate magical talent, they compensate for with determination, ingenuity, and sheer stubbornness."
            ],
            ["Elf"] =
            [
                "The elves are an ancient race whose civilization predates the Sundering by millennia. Their deep connection to the natural world and the arcane currents that flow through it grants them extraordinary magical affinity.",
                "Elven society is centered around the Great Groves—ancient forests where the trees themselves are sentient and serve as living libraries, storing the memories and wisdom of countless generations.",
                "The elves suffered terribly during the Sundering, losing entire cities to the magical cataclysm. This trauma made them cautious and sometimes aloof, though younger elves have grown more willing to engage with other races.",
                "Elven mages are among the most powerful in the world, and their tradition of Moon Singing—casting spells through harmonic vocal patterns—is considered the highest form of magical art."
            ],
            ["Dwarf"] =
            [
                "Born of stone and fire, the dwarves have shaped the underground world for as long as any race can remember. Their cities, carved from living rock, are marvels of engineering that can endure for millennia.",
                "Dwarven culture revolves around craft and clan. Every dwarf is expected to master a trade, and the quality of one's work is the truest measure of worth in dwarven society.",
                "The dwarves were the first to discover the Luminous Caverns deep beneath the earth, and they guard these crystal-filled wonders as sacred spaces. To damage a natural crystal formation is among the gravest dwarven taboos.",
                "In battle, dwarves favor heavy armor and devastating close-range combat. The Shieldwall formation—an impenetrable wall of interlocked dwarven shields—has broken the charge of armies many times its size."
            ],
            ["Gnome"] =
            [
                "Gnomes are the smallest of the civilized races but possess intellects that rival any. Their insatiable curiosity drives them to tinker, experiment, and explore, often with explosive results.",
                "Gnomish society values invention above all else. The annual Gear Summit, held in a different city each year, showcases contraptions ranging from self-stirring soup pots to flying machines (the latter with varying success rates).",
                "Though physically frail, gnomes possess a natural affinity for arcane magic that surpasses even the elves in raw potential, if not in discipline. Gnomish magic tends to be creative, unpredictable, and occasionally catastrophic.",
                "Gnome engineers built the windmills of Windmill Crossing, the levees of Riverside, and the lightning harnesses of Stormwatch. Their motto: 'If it hasn't exploded yet, you haven't pushed it far enough.'"
            ]
        };

        /// <summary>
        /// Gets lore paragraphs for a race by name.
        /// </summary>
        public static string[] GetRaceLore(string raceName)
        {
            return RaceLoreEntries.TryGetValue(raceName, out var lore)
                ? lore
                : [$"The {raceName} are a people of mystery, their origins lost to the ages."];
        }

        /// <summary>
        /// Displays formatted lore for a race.
        /// </summary>
        public static void DisplayRaceLore(string raceName)
        {
            var lore = GetRaceLore(raceName);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 The {raceName,-51}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            foreach (var paragraph in lore)
            {
                Console.WriteLine();
                WriteWrapped(paragraph, 60);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Enemy Faction Lore

        private static readonly Dictionary<EnemyCampType, string[]> FactionLoreEntries = new()
        {
            [EnemyCampType.BanditHideout] =
            [
                "The bandits that plague the roads are not a single organization but a loose network of desperate men and women driven to crime by war, famine, and displacement.",
                "Some bandit chiefs were once soldiers or minor nobles who lost everything in the wars following the Sundering. Their tactical knowledge makes them far more dangerous than common thugs.",
                "The Crimson Coin, a shadowy syndicate rumored to operate from the cities themselves, supplies many bandit groups with intelligence on wealthy caravans in exchange for a cut of the spoils."
            ],
            [EnemyCampType.GoblinWarcamp] =
            [
                "Goblins were once a subterranean race, content to dwell in shallow cave networks. The expansion of dwarven mining operations drove them to the surface, where they became raiders out of necessity.",
                "Goblin warlords rule through cunning rather than strength. Their war-camps are surprisingly well-organized, with scouts, trappers, and alchemists supporting the rank-and-file warriors.",
                "Despite their reputation, goblins possess a rich oral tradition and a complex language. Some scholars believe they were once allied with the gnomes before a betrayal—the details of which differ depending on who tells the tale."
            ],
            [EnemyCampType.OrcStronghold] =
            [
                "The orcs are a proud warrior race whose culture revolves around strength, honor, and clan loyalty. Their strongholds are fortified camps built to withstand siege.",
                "Orcish society is not inherently evil—many orc clans seek only to defend their territory. However, the rise of the Bloodfang faction, which preaches conquest and domination, has turned many clans aggressive.",
                "The greatest orc warriors enter a battle trance called the Wrath, a state of heightened strength and fearlessness. Controlling this state is the mark of a true chieftain."
            ],
            [EnemyCampType.UndeadGraveyard] =
            [
                "The undead that haunt the graveyards and battlefields of the world are remnants of the Sundering's darkest legacy—a wave of necromantic energy that seeped into the earth itself.",
                "Not all undead are mindless. Liches—powerful mages who chose undeath to continue their research—command vast armies of lesser undead, pursuing agendas that span centuries.",
                "The Pallid Order, a secretive group of necromancers, deliberately creates undead outposts to 'reclaim' territory they believe was promised to them by the death god Morvath before his imprisonment."
            ],
            [EnemyCampType.BeastDen] =
            [
                "The wild beasts that threaten travelers are often mundane animals driven mad by proximity to corrupted magical sites—a lingering effect of the Sundering.",
                "Alpha beasts that lead packs near magical corruption zones grow larger and more aggressive than their natural counterparts, developing thick hides and, in some cases, rudimentary magical abilities.",
                "Rangers and druids have long warned that the increasing aggression of wildlife is a symptom of deeper magical imbalance. Until the corruption is cleansed, the beasts will only grow more dangerous."
            ],
            [EnemyCampType.CultistShrine] =
            [
                "The dark cults that operate across the realm worship entities from beyond the mortal plane—beings of immense power whose true nature defies comprehension.",
                "Cultist shrines are built on ley line intersections where the barrier between worlds is weakest. The rituals performed there slowly erode reality, creating zones of magical instability.",
                "Many cultists were once ordinary people—scholars, priests, or mages—seduced by promises of forbidden knowledge. Their leaders, the High Priests, have surrendered their humanity entirely in exchange for dark power."
            ],
            [EnemyCampType.DragonLair] =
            [
                "Dragons are among the oldest and most powerful beings in the world, predating even the elves. Each dragon's lair is both a fortress and a treasure vault accumulated over centuries of hoarding.",
                "The Second Dragon War, fought three hundred years ago, ended in a fragile truce. Most dragons retreated to remote lairs, but younger dragons—restless and ambitious—increasingly violate the accord.",
                "Dragon lairs are saturated with draconic magic that warps the surrounding environment. Plants grow with crystalline bark, animals develop scales, and the very stones radiate warmth."
            ],
            [EnemyCampType.DemonPortal] =
            [
                "Demon portals are tears in the fabric of reality, created when dark magic accumulates beyond what the mortal plane can contain. Through these rifts, demonic entities seep into the world.",
                "The demons that emerge are not a unified force but refugees and conquerors from a realm of eternal conflict. Some seek dominion, others simply destruction, and a rare few seek asylum.",
                "Closing a demon portal requires immense magical power and precise ritual knowledge. The Arcane Academy of Mysthaven maintains a specialized order, the Voidwardens, dedicated to sealing these breaches."
            ],
            [EnemyCampType.ElementalNexus] =
            [
                "Elemental nexus points are locations where the raw forces of creation—fire, water, earth, and air—clash in perpetual conflict, spawning elemental creatures from the turbulent energy.",
                "These nexus points are natural phenomena, not inherently malevolent, but the creatures they spawn are driven by primal instinct and pose a severe threat to anything nearby.",
                "Some mages seek to harness nexus energy for enchantment and artifice. The Emberpeak alchemists have had particular success, though the dangers of working with unstable elemental forces cannot be overstated."
            ],
            [EnemyCampType.SpiderNest] =
            [
                "The giant spiders that infest caves, forests, and ruins are descended from arachnids exposed to concentrated Sundering magic. They have grown to monstrous proportions over generations.",
                "Spider nests are organized around a Brood Mother—a spider of enormous size and cunning intelligence who directs her offspring with pheromone signals and, some scholars claim, a rudimentary telepathy.",
                "Spider silk harvested from cleared nests is extraordinarily valuable. Weavers can craft armor from it that is lighter than leather yet stronger than chain mail, though the gathering process is perilous."
            ]
        };

        /// <summary>
        /// Gets lore paragraphs for an enemy faction by camp type.
        /// </summary>
        public static string[] GetFactionLore(EnemyCampType campType)
        {
            return FactionLoreEntries.TryGetValue(campType, out var lore)
                ? lore
                : ["Little is known about this faction. Their origins remain a mystery."];
        }

        /// <summary>
        /// Displays formatted lore for an enemy faction.
        /// </summary>
        public static void DisplayFactionLore(EnemyCampType campType)
        {
            var lore = GetFactionLore(campType);
            string factionName = GetFactionName(campType);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 {factionName,-55}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            foreach (var paragraph in lore)
            {
                Console.WriteLine();
                WriteWrapped(paragraph, 60);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static string GetFactionName(EnemyCampType campType)
        {
            return campType switch
            {
                EnemyCampType.BanditHideout => "The Bandit Underworld",
                EnemyCampType.GoblinWarcamp => "The Goblin Horde",
                EnemyCampType.OrcStronghold => "The Orc Clans",
                EnemyCampType.UndeadGraveyard => "The Restless Dead",
                EnemyCampType.BeastDen => "The Corrupted Wilds",
                EnemyCampType.CultistShrine => "The Dark Cults",
                EnemyCampType.DragonLair => "The Dragon Accord",
                EnemyCampType.DemonPortal => "The Demonic Incursion",
                EnemyCampType.ElementalNexus => "The Elemental Storms",
                EnemyCampType.SpiderNest => "The Brood",
                _ => "Unknown Faction"
            };
        }

        #endregion

        #region Region Lore

        private static readonly Dictionary<string, string[]> RegionLoreEntries = new()
        {
            ["Whispering Woods"] =
            [
                "The Whispering Woods earned their name from the soft voices that travelers hear drifting between the ancient oaks—fragments of conversations from centuries past, trapped in the bark by residual Sundering magic.",
                "Despite its eerie reputation, the woods are the safest wilderness near Havenbrook. Novice adventurers cut their teeth here, hunting boars and clearing goblin scouts from the underbrush.",
                "Druids maintain hidden shrines throughout the forest, and those who treat the woods with respect sometimes find healing herbs left on their path—gifts from unseen guardians."
            ],
            ["Shadowfen Marsh"] =
            [
                "The Shadowfen is a vast wetland where the water runs black with dissolved peat and the air hangs heavy with the smell of decay. Navigation without a guide is nearly impossible; the trails shift with every tide.",
                "Necromantic energy from an ancient battlefield seeps upward through the mud, animating the remains of soldiers who fell here during the Orc Siege. Their restless patrols make the marsh treacherous after dark.",
                "Herbalists brave the Shadowfen for its bounty of rare fungi and marsh lotus, ingredients that cannot be cultivated anywhere else. The reward is great, but so is the risk."
            ],
            ["Frostpeak Mountains"] =
            [
                "The Frostpeaks are a jagged wall of granite and ice that divides the northern reaches from the temperate lowlands. Dwarven mining outposts dot the lower slopes, but above the snowline, only the hardiest creatures survive.",
                "Ancient ruins cling to the mountainsides—watchtowers and temples from the Old Empire, half-buried in glacial ice. Scholars believe a network of underground highways once connected them, though most passages have collapsed.",
                "The peaks are home to frost wyrms, lesser cousins of true dragons, who nest in the highest caves and hunt mountain goats with terrifying speed."
            ],
            ["Scorching Sands"] =
            [
                "The Scorching Sands stretch endlessly south of Sunspire, a sea of golden dunes hiding the ruins of a civilization that predates the Sundering by untold centuries.",
                "Sandstorms here are not merely weather—they are manifestations of elemental fury, conjured by nexus points buried beneath the desert. Travelers caught in one may find themselves deposited miles from where they started.",
                "Desert nomads speak of the Glass City, a mirage that appears at dusk: a shimmering metropolis of crystal spires. Those who follow it vanish, though some return years later with no memory of where they have been."
            ],
            ["Deathwhisper Graveyard"] =
            [
                "Once the Grand Cemetery of the Old Empire's capital, the Deathwhisper Graveyard now spans miles of crumbling mausoleums and toppled headstones. The ground itself hums with necrotic resonance.",
                "The Pallid Order maintains a hidden sanctum here, raising the ancient dead to serve as an army they believe will 'reclaim the world for the deserving.' Their rituals grow bolder with each passing season.",
                "Despite its horrors, the graveyard holds relics of immense value—weapons and armor interred with fallen heroes, enchantments still intact after centuries in the earth."
            ],
            ["Ashfall Crater"] =
            [
                "The Ashfall Crater is what remains of a volcano that erupted during the Sundering, its caldera now a hellscape of lava rivers, obsidian fields, and geysers of superheated steam.",
                "Fire elementals roam the crater in packs, drawn to the nexus of elemental energy at its heart. The Emberpeak alchemists harvest crystallized fire from the crater's rim, but venturing deeper is suicidal without magical protection.",
                "Legends claim that a phoenix nests at the crater's deepest point, reborn in the lava every century. Its feathers, if recovered, are said to grant immunity to flame."
            ],
            ["Moonlight Glade"] =
            [
                "The Moonlight Glade exists in a state of perpetual twilight—neither day nor night—where the trees glow with bioluminescent sap and time moves differently than in the outside world.",
                "Hours spent in the Glade may translate to days or mere minutes in the wider world. Elven scholars believe the Glade occupies a fold in reality created during the Sundering.",
                "Fey creatures inhabit the Glade, and their intentions are unpredictable. A traveler may receive a priceless gift or a terrible curse, depending on the fey's inscrutable whims."
            ],
            ["Stormbreaker Shores"] =
            [
                "The Stormbreaker Shores are a jagged coastline of black basalt cliffs battered by perpetual storms. Lightning strikes the sea dozens of times per hour, and the thunder never ceases.",
                "Shipwrecks litter the shore, driven onto the rocks by the enchanted storms. Scavengers pick through the wreckage for cargo, but the Ghost Fleet—spectral ships crewed by drowned sailors—attacks anyone who lingers too long.",
                "Stormwatch's navigators train here, learning to read the lightning patterns that reveal safe passages through the tempest."
            ],
            ["Blightlands"] =
            [
                "The Blightlands are a corrupted wasteland where nothing natural grows. The soil is ashen grey, the water runs with a sickly luminescence, and the sky is perpetually overcast with clouds that seem to absorb light.",
                "This is where the ancient evil was imprisoned after the War of Dawn—sealed beneath the earth by a coalition of the realm's greatest mages. The corruption that seeps from its prison warps everything it touches.",
                "Creatures born in the Blightlands are twisted parodies of natural life: wolves with too many eyes, birds that fly in spirals until they die, trees that uproot themselves and shamble toward sound."
            ],
            ["Luminous Depths"] =
            [
                "Miles beneath the surface, beyond even Deepstone's lowest levels, the Luminous Depths are a labyrinth of crystal caverns lit by veins of raw magical ore that pulse like a heartbeat.",
                "The crystals here resonate with arcane energy, and skilled mages can 'listen' to them to hear echoes of spells cast centuries ago. Some researchers have gone mad from the whispers.",
                "The Depths are home to creatures found nowhere else: eyeless fish that navigate by magical sonar, crystal golems that form spontaneously from the walls, and the mythical Deep Wurm, a beast of legend no living adventurer has confirmed seeing."
            ],
            ["Fields of Valor"] =
            [
                "The Fields of Valor mark the site of the final battle of the War of Dawn, where the allied armies broke the Dread Lord Morvath's legions and drove him back to Shadowkeep.",
                "The ground is scarred with trenches and craters that have never fully healed. Weapons and armor from the fallen still surface after heavy rains, and some carry enchantments that activate at the touch of the living.",
                "Every year on the anniversary of the battle, ghostly armies re-enact the conflict across the fields. Watching this spectral replay is a rite of passage for young soldiers across the realm."
            ],
            ["Starfall Peak"] =
            [
                "Starfall Peak is the tallest mountain in the known world, so high that its summit pierces the cloud layer and extends into a realm of crystalline sky where the stars are close enough to touch.",
                "Fragments of fallen stars—meteorites infused with cosmic magic—litter the slopes above the snowline. Skyreach's smiths forge these fragments into weapons of extraordinary power.",
                "The ascent to Starfall Peak is itself a trial. The thin air, extreme cold, and predatory rocs that nest on the cliffs have claimed countless lives. Only the most experienced adventurers attempt the climb."
            ],
            ["Thornwood Wilds"] =
            [
                "The Thornwood is a dense forest of ironbark trees and razor-thorned brambles that can shred leather armor in seconds. Navigation requires a machete—or a druid's blessing.",
                "The forest is home to a tribe of wood elves who have rejected contact with the outside world. They tolerate travelers who pass quickly but attack those who linger or damage the trees.",
                "Carnivorous plants of enormous size grow in the Thornwood's deepest groves, luring prey with sweet scents and bioluminescent blooms before snapping shut with crushing force."
            ],
            ["Crimson Canyon"] =
            [
                "The Crimson Canyon is a labyrinth of red sandstone pillars and narrow passages carved by an ancient river that has long since dried up. Its walls are stained rust-red by iron deposits.",
                "Bandits have claimed the canyon as their stronghold for generations, using its maze-like passages to ambush caravans and evade pursuit. The Crimson Coin syndicate operates its largest base here.",
                "Deep within the canyon, petroglyphs from the pre-Sundering era depict scenes of a civilization that could control the weather. The images show storms being called and dismissed with gestures."
            ],
            ["Mistral Plateau"] =
            [
                "The Mistral Plateau is a windswept highland where ancient monoliths stand in geometric patterns that no scholar has been able to decipher. The wind between the stones produces an eerie harmonic tone.",
                "The monoliths predate every known civilization—even the elves have no record of who erected them. During celestial events, the stones glow with an inner light and the harmonic tone shifts to what some describe as speech.",
                "Wind elementals are drawn to the plateau in great numbers, and their constant presence makes the winds unpredictable and dangerously powerful. Travelers have been swept off the plateau's edge without warning."
            ]
        };

        /// <summary>
        /// Gets lore paragraphs for an exploration region by name.
        /// </summary>
        public static string[] GetRegionLore(string regionName)
        {
            return RegionLoreEntries.TryGetValue(regionName, out var lore)
                ? lore
                : [$"The region known as {regionName} remains largely uncharted. Mapmakers mark it only with the old warning: 'Here be danger.'"];
        }

        /// <summary>
        /// Displays formatted lore for an exploration region.
        /// </summary>
        public static void DisplayRegionLore(string regionName)
        {
            var lore = GetRegionLore(regionName);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 Region: {regionName,-48}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            foreach (var paragraph in lore)
            {
                Console.WriteLine();
                WriteWrapped(paragraph, 60);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Champion Class Lore

        private static readonly Dictionary<string, string[]> ChampionLoreEntries = new()
        {
            ["Paladin"] =
            [
                "The Paladins trace their lineage to the Order of the Silver Dawn, a brotherhood of holy warriors founded during the War of Dawn to combat Morvath's undead legions.",
                "A Paladin's power flows from an oath—a binding covenant sworn before the gods. Breaking this oath strips a Paladin of their divine gifts and brands their soul with a mark visible to all clerics.",
                "The Trial of Radiance, the final test of Paladin ascension, requires the candidate to walk through a corridor of holy fire. Those found worthy emerge unscathed; those found wanting are consumed."
            ],
            ["Berserker"] =
            [
                "Berserkers descend from the Winterborn clans of Frostholm, warriors who learned to channel primal rage into devastating combat prowess through a ritual involving the blood of frost giants.",
                "The Berserker's Fury is not mere anger—it is a controlled descent into a primal state where pain is ignored, fear is impossible, and the body pushes beyond mortal limits at the cost of terrible exhaustion.",
                "Elders warn that the Fury, if used too often, begins to erode the Berserker's humanity. Legendary Berserkers of the past became indistinguishable from the beasts they hunted."
            ],
            ["Archmage"] =
            [
                "The title of Archmage is bestowed by the Veil Archive of Mysthaven upon mages who demonstrate mastery over at least three schools of magic simultaneously—a feat that requires decades of study.",
                "Archmages perceive magic as a living tapestry woven through all things. They do not merely cast spells; they reshape the fabric of reality, pulling threads of energy from the world itself.",
                "The greatest Archmage in history, Seraphina Voidwalker, once stopped a volcanic eruption by unraveling the elemental fury at its core. She vanished afterward, and her staff still hums with power in the Archive's deepest vault."
            ],
            ["Shadowblade"] =
            [
                "Shadowblades are assassins who have learned to step between the folds of shadow, existing partially in the mortal world and partially in a dark reflection known as the Umbral Veil.",
                "The training of a Shadowblade involves three days of total sensory deprivation in a lightless chamber beneath Shadowkeep. Those who emerge can see in absolute darkness and move without sound.",
                "The Shadowblade's Code demands that no innocent blood be shed. Despite their terrifying abilities, true Shadowblades are instruments of justice—silent protectors who eliminate threats before they materialize."
            ],
            ["Guardian"] =
            [
                "Guardians are warriors who have bonded with the living stone of the earth itself, gaining the ability to channel geological forces through their shields and armor.",
                "The Bond of Stone is forged in the Luminous Depths, where the candidate meditates for a week beside a crystal vein until the earth accepts them. Those rejected are simply expelled to the surface; those accepted gain skin that can harden to granite.",
                "Guardians are the last line of defense in any battle. Their oath: 'None shall fall while I stand.' A Guardian who loses a comrade carries that weight as a physical burden—their stone bond grows heavier with each failure."
            ],
            ["Ranger"] =
            [
                "Rangers are the eyes and ears of civilization, solitary scouts who walk the wild places and keep watch over the borders between the settled lands and the untamed wilderness.",
                "Every Ranger undergoes the Long Walk—a year-long solo journey through the most dangerous regions of the realm. They begin with nothing but a knife and must survive by skill alone.",
                "Rangers share a deep bond with the natural world, and many form lifelong partnerships with animal companions. The bond is mystical in nature; Ranger and beast share senses and, some say, thoughts."
            ],
            ["Templar"] =
            [
                "Templars are warrior-priests who serve as the militant arm of the realm's temples, combining martial discipline with divine magic to protect the faithful and punish the wicked.",
                "The Templar's Judgment is a fearsome ability—a burst of holy energy that sears the guilty and heals the innocent. It requires absolute moral conviction; doubt weakens the effect and can turn it inward.",
                "Templars played a pivotal role in the War of Dawn, forming the vanguard that breached Shadowkeep's gates. Their battle hymn, 'Light Endures,' is still sung in temples across the realm."
            ],
            ["Assassin"] =
            [
                "The Assassin's Guild operates from the shadows of every major city, a network so secretive that even its members know only their direct contacts. The guild's origins are lost, but its influence is undeniable.",
                "An Assassin's blade is coated in a unique poison crafted specifically for their hand. The formula is committed to memory and never written down; it dies with its maker.",
                "Despite their lethal reputation, Assassins follow the Rule of the Contract: no kill is made without a sanctioned agreement, and betrayal of a contract is punished by the guild with swift finality."
            ],
            ["Elementalist"] =
            [
                "Elementalists are mages who have forsaken broad magical study to achieve total mastery over one of the four primal elements: fire, water, earth, or air.",
                "The Elemental Pact requires the mage to survive direct exposure to their chosen element in its purest form—standing in the heart of a bonfire, submerging in a whirlpool, being buried alive, or weathering a hurricane.",
                "At the height of their power, Elementalists can transform their bodies into their chosen element entirely. A fire Elementalist becomes a walking inferno; an earth Elementalist, a moving mountain."
            ],
            ["Necromancer"] =
            [
                "Necromancers walk a path reviled by most but understood by few. Their magic draws on the boundary between life and death—a boundary that the Sundering weakened irrevocably.",
                "Not all Necromancers serve darkness. The Order of the Grey Veil uses necromantic knowledge to lay the restless dead to peace, seal cursed burial sites, and counter the Pallid Order's machinations.",
                "The price of necromancy is steep: practitioners age faster than their peers, and their connection to the living world slowly erodes. The most powerful Necromancers appear as walking corpses themselves."
            ],
            ["Oracle"] =
            [
                "Oracles are priests who have been touched by the Sight—a rare and uncontrollable gift that grants visions of possible futures at the cost of immense physical and mental strain.",
                "The Sight cannot be learned or taught; it chooses its bearer. Most Oracles manifest the gift in childhood, experiencing prophetic dreams that grow more vivid and demanding with age.",
                "An Oracle's predictions are never certain—they see branching possibilities, not fixed destinies. The greatest challenge of the Sight is determining which branch is most likely and communicating it before the moment passes."
            ],
            ["Druid"] =
            [
                "Druids are the custodians of the natural world, drawing power from the Great Groves—ancient sentient forests that serve as repositories of the world's ecological memory.",
                "The Rite of Roots, which transforms a priest into a Druid, involves planting a sapling with one's own blood and guarding it for a full year. The sapling's survival determines the Druid's acceptance by the Green.",
                "Druids can shapeshift into animals, command plants to grow or wither, and commune with the spirits of the natural world. Their deepest taboo is the use of metal weapons, which they believe sever the connection to the earth."
            ]
        };

        /// <summary>
        /// Gets lore paragraphs for a champion class by name.
        /// </summary>
        public static string[] GetChampionLore(string className)
        {
            return ChampionLoreEntries.TryGetValue(className, out var lore)
                ? lore
                : [$"The path of the {className} is shrouded in mystery, its secrets passed only from master to apprentice."];
        }

        /// <summary>
        /// Displays formatted lore for a champion class.
        /// </summary>
        public static void DisplayChampionLore(string className)
        {
            var lore = GetChampionLore(className);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 The Path of the {className,-39}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            foreach (var paragraph in lore)
            {
                Console.WriteLine();
                WriteWrapped(paragraph, 60);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Legendary Artifact Lore

        private static readonly Dictionary<string, string[]> ArtifactLoreEntries = new()
        {
            ["The Hearthstone of Aldric"] =
            [
                "The original hearthstone that Aldric Haven placed in his roadside inn three centuries ago still burns beneath Havenbrook's Grand Market. No fuel feeds it; no wind can extinguish it.",
                "Scholars believe the stone is a fragment of a pre-Sundering power source, accidentally activated by Aldric's campfire. Its warmth sustains the city's wards against siege and pestilence.",
                "Legend says that if the hearthstone is ever removed from Havenbrook, the city will fall within a fortnight. No ruler has dared test this prophecy."
            ],
            ["The Anchor Stone of Skyreach"] =
            [
                "The Anchor Stone is a sphere of unknown metal that hovers at the exact center of Skyreach's largest floating island. It emits a low hum that can be felt in the bones rather than heard.",
                "No tool has ever scratched its surface. No spell has penetrated its shell. The Wind Riders guard it at all times, for if the Anchor Stone were destroyed, every island would plummet to the plains below.",
                "Once per generation, the Anchor Stone flickers—a brief dimming that causes the islands to shudder. During the last flickering, an entire district collapsed into the sky before stabilizing."
            ],
            ["Morvath's Crown"] =
            [
                "The iron crown of the Dread Lord Morvath was not destroyed after the War of Dawn—it could not be. Every attempt to melt, shatter, or unmake it failed, the crown reforming within hours.",
                "It was sealed in the deepest vault of Shadowkeep, guarded by enchantments layered by mages from every city. The crown whispers to those who draw near, promising power beyond mortal comprehension.",
                "Some believe the crown is not merely an artifact but a vessel—that a fragment of Morvath's consciousness survives within it, patiently waiting for a hand weak enough to lift it."
            ],
            ["The Veil Archive's First Tome"] =
            [
                "The First Tome of the Veil Archive is a book that writes itself. Its pages fill with text in a language that shifts depending on the reader, always presenting knowledge the reader desperately needs.",
                "No one knows who created the Tome or when it appeared in Mysthaven's library. The earliest records mention it as already ancient when the Academy was founded.",
                "Reading the Tome exacts a price: for every secret gained, the reader forgets something of equal value. Archmages consult it only in the direst emergencies, and always with a scribe to record what they learn before they forget."
            ],
            ["The Dragonfire Heart"] =
            [
                "The Great Forge of Ironforge Citadel burns with dragonfire captured during the Second Dragon War—a flame torn from the chest of the dragon Kael'thorax as he fell in battle above Mount Anvilar.",
                "The Dragonfire Heart, as the dwarves call it, burns hotter than any mundane flame and imbues steel with extraordinary resilience. Weapons forged in its heat never dull and armor never rusts.",
                "Kael'thorax's kin have demanded the Heart's return for three centuries. The dwarves' refusal is the primary source of tension between Ironforge and the remaining dragons."
            ],
            ["The Sunderer's Staff"] =
            [
                "The Sunderer's Staff is a myth—or so most believe. It is said to be the instrument that caused the Sundering itself, a conduit of such power that its activation tore the continent apart.",
                "Fragments of the Staff appear in legends across every culture. The elves call it Aen'lithir, the Worldbreaker. The dwarves know it as the Fault Hammer. Humans simply call it the End.",
                "If the Staff still exists, finding it would present an impossible dilemma: it could seal the Blightlands' corruption forever, or it could trigger a second Sundering that would destroy what remains of the world."
            ]
        };

        /// <summary>
        /// Gets lore paragraphs for a legendary artifact by name.
        /// </summary>
        public static string[] GetArtifactLore(string artifactName)
        {
            return ArtifactLoreEntries.TryGetValue(artifactName, out var lore)
                ? lore
                : [$"The artifact known as {artifactName} is mentioned in fragments across a dozen ancient texts, but no complete account of its origin survives."];
        }

        /// <summary>
        /// Displays formatted lore for a legendary artifact.
        /// </summary>
        public static void DisplayArtifactLore(string artifactName)
        {
            var lore = GetArtifactLore(artifactName);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 Artifact: {artifactName,-46}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            foreach (var paragraph in lore)
            {
                Console.WriteLine();
                WriteWrapped(paragraph, 60);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Mythic Title Lore

        private static readonly Dictionary<string, string[]> MythicTitleLoreEntries = new()
        {
            ["The Undying"] =
            [
                "The title of 'The Undying' was first bestowed upon Kara Ashveil, a human warrior who fell in battle against the Dread Lord's vanguard—and rose again, held together by sheer will and the prayers of her comrades.",
                "Those who bear this title have passed through death's threshold and returned. Their bodies carry a faint chill, and wounds that would kill a lesser being close with unnatural speed.",
                "The Undying are both revered and feared. Some see them as blessed by the gods; others whisper that they have cheated death, and that death remembers."
            ],
            ["The Wrathful"] =
            [
                "The title of 'The Wrathful' honors the memory of Grok Bloodfist, an orc chieftain who turned against the Dread Lord's corruption and fought with such fury that his enemies fled before his shadow alone.",
                "The Wrathful channel raw emotion—anger, grief, determination—into physical devastation. Their strikes carry the weight of every battle they have survived, every ally they have lost.",
                "Scholars at Mysthaven theorize that the Wrathful unconsciously tap into elemental fury, channeling the same primal forces that drive storms and earthquakes through their weapons."
            ],
            ["The Sage"] =
            [
                "The title of 'The Sage' has been carried by only a handful of individuals across recorded history, each possessing an understanding of magic so profound that reality itself bends to accommodate their will.",
                "Seraphina Voidwalker, the greatest Archmage in history, was the most famous Sage. Her ability to unmake spells mid-cast and reweave enchantments on the fly remains unmatched centuries after her disappearance.",
                "The Sage's power comes not from brute magical force but from efficiency—a perfect economy of arcane energy that makes every spell twice as potent at half the cost."
            ],
            ["The Ironclad"] =
            [
                "The title of 'The Ironclad' was first claimed by Thane Bouldershield, a dwarven Guardian who stood alone at the Gates of Deepstone and held back a cave troll assault for three days without sleep.",
                "Those who bear this title embody the concept of the immovable object. Their defensive instincts are so honed that they reflexively absorb punishment meant for their allies.",
                "An Ironclad's presence on the battlefield is said to change the flow of combat itself. Enemies instinctively focus their attacks on the Ironclad, drawn by a gravitational pull of sheer defiance."
            ],
            ["The Swift"] =
            [
                "The title of 'The Swift' was earned by Lyra Windstep, an elven Ranger who crossed the entire realm in seven days to deliver warning of the Dread Lord's advance—a journey that normally takes months.",
                "The Swift exist in a state of accelerated perception. To them, the world moves slowly—raindrops hang in the air, arrows drift lazily, and opponents telegraph their attacks with painful obviousness.",
                "Some believe the Swift have thinned the barrier between themselves and the flow of time, borrowing seconds from the future. Whether this debt must someday be repaid remains a subject of nervous speculation."
            ],
            ["The Eternal"] =
            [
                "The title of 'The Eternal' is the rarest and most mysterious of all Mythic Titles. It is said to be bestowed not by mortal recognition but by the world itself—the ley lines, the stones, the wind.",
                "Those who bear this title exhibit a perfect balance of all abilities—strength, speed, wisdom, and resilience—that defies the normal limits of specialization. They are generalists elevated to a level that surpasses most specialists.",
                "The Eternal regenerate slowly but constantly—wounds close, fatigue lifts, and magical reserves refill as if the world itself is sustaining them. Whether this is a gift or a binding remains unclear."
            ]
        };

        /// <summary>
        /// Gets lore paragraphs for a mythic title by name.
        /// </summary>
        public static string[] GetMythicTitleLore(string titleName)
        {
            return MythicTitleLoreEntries.TryGetValue(titleName, out var lore)
                ? lore
                : [$"The Mythic Title '{titleName}' is spoken of in hushed tones, its full history known only to those who bear it."];
        }

        /// <summary>
        /// Displays formatted lore for a mythic title.
        /// </summary>
        public static void DisplayMythicTitleLore(string titleName)
        {
            var lore = GetMythicTitleLore(titleName);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 Mythic Title: {titleName,-42}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            foreach (var paragraph in lore)
            {
                Console.WriteLine();
                WriteWrapped(paragraph, 60);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Historical Eras

        private static readonly (string Era, string Period, string[] Details)[] HistoricalEras =
        [
            ("The Age of Unity", "Before the Sundering",
            [
                "In the time before memory, the world was one—a single continent united under a civilization whose mastery of magic and engineering surpassed anything that exists today.",
                "The Builders, as they are called by modern scholars, raised cities of crystal and living stone, powered by a network of ley lines that channeled the world's magical energy like rivers of light.",
                "Their greatest achievement was the Harmony—a state of perfect balance between the mortal world and the elemental planes. Under the Harmony, the seasons were gentle, the harvests abundant, and disease was unknown."
            ]),
            ("The Sundering", "Approximately 3,000 years ago",
            [
                "The cause of the Sundering remains the most debated question in scholarly history. What is known is that a single catastrophic event shattered the continent, reshaped the oceans, and unleashed raw magical energy across the world.",
                "The elves, the only modern race old enough to retain fragments of pre-Sundering memory, speak of 'the Mistake'—a ritual of unimaginable ambition that went catastrophically wrong.",
                "In the aftermath, the ley line network collapsed. Magic became wild and unpredictable. Entire species were transformed by the uncontrolled energy, giving rise to the monsters and magical beasts that plague the world today.",
                "The Builders vanished entirely. Whether they were destroyed, fled to another plane, or were transformed beyond recognition remains unknown. Their ruins—Sunspire's catacombs, Stonebridge's arch, the Mistral monoliths—are all that remain."
            ]),
            ("The Age of Rebuilding", "3,000 - 1,000 years ago",
            [
                "The surviving races emerged from the Sundering's devastation slowly. The elves retreated to their Great Groves to mourn. The dwarves delved deeper underground, seeking stability in the stone. Humans and gnomes scattered across the changed landscape.",
                "It took centuries for new civilizations to form. The first post-Sundering city was Deepstone, founded when dwarven miners broke through into the Luminous Caverns and found a sanctuary of crystal light.",
                "During this era, the races had little contact with one another. Each struggled independently against the newly hostile world, developing the distinct cultures and traditions that define them today."
            ]),
            ("The Old Empire", "1,000 - 200 years ago",
            [
                "The Old Empire was a human-dominated realm that united most of the known world under a single crown through a combination of military conquest, diplomatic marriage, and trade agreements.",
                "At its height, the Empire maintained order across thousands of miles, built the King's Road network that still serves as the realm's primary trade routes, and established the first inter-racial courts of law.",
                "Corruption rotted the Empire from within. A succession of weak emperors, ambitious generals, and a bloated bureaucracy led to civil war. The Empire fractured into squabbling kingdoms, and the roads became hunting grounds for bandits."
            ]),
            ("The War of Dawn", "Approximately 100 years ago",
            [
                "The Dread Lord Morvath rose from the ruins of the Old Empire, a sorcerer of terrifying power who had unlocked the secrets of the Pallid Order's darkest rituals. He raised an army of undead that swept across the realm.",
                "For the first time since the Sundering, the four races united. Dwarven shieldwalls held the mountain passes. Elven archers rained enchanted arrows from the tree lines. Gnome engineers built siege weapons of devastating ingenuity. Human knights led the charge.",
                "The final battle was fought on the Fields of Valor, where Morvath's legions were broken. The Dread Lord retreated to his fortress, where he was defeated in single combat by a coalition of champions. His fortress was repurposed as Shadowkeep.",
                "Morvath was not killed—he could not be. His essence was bound and imprisoned beneath the Blightlands, sealed by the combined magic of every Archmage in the realm. The seal holds, but it weakens with each passing decade."
            ]),
            ("The Current Age", "Present day",
            [
                "The century since the War of Dawn has been one of cautious rebuilding. The Accord of Open Commerce, signed in Havenbrook, maintains peace between the races, though tensions simmer beneath the surface.",
                "New threats emerge with increasing frequency. Demon portals open where the Sundering's residual magic is strongest. Orc clans, driven by the militant Bloodfang faction, grow more aggressive. The Blightlands' corruption spreads slowly but steadily.",
                "Heroes are needed more than ever. The old alliances are fraying, the ancient seals are weakening, and the world stands once again at a crossroads between salvation and catastrophe.",
                "It is into this world that you step—armed with courage, bound by purpose, and burdened with a destiny that may decide the fate of all."
            ])
        ];

        /// <summary>
        /// Gets the details for a specific historical era by name.
        /// </summary>
        public static string[] GetEraLore(string eraName)
        {
            foreach (var era in HistoricalEras)
            {
                if (string.Equals(era.Era, eraName, StringComparison.OrdinalIgnoreCase))
                    return era.Details;
            }
            return [$"The era known as '{eraName}' is referenced in only the most obscure texts, its details lost to the passage of time."];
        }

        /// <summary>
        /// Displays a full interactive timeline of the world's history.
        /// </summary>
        public static void DisplayTimeline()
        {
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  📜 Timeline of the Known World                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

            for (int i = 0; i < HistoricalEras.Length; i++)
            {
                var era = HistoricalEras[i];
                Console.WriteLine($"\n  [{i + 1}] {era.Era} — {era.Period}");
            }

            Console.WriteLine($"\n  [0] Return");
            Console.Write("\nSelect an era to learn more: ");

            var input = Console.ReadLine()?.Trim() ?? "";
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= HistoricalEras.Length)
            {
                var selected = HistoricalEras[choice - 1];
                Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
                Console.WriteLine($"║  📜 {selected.Era,-55}║");
                Console.WriteLine($"║     {selected.Period,-55}║");
                Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

                foreach (var paragraph in selected.Details)
                {
                    Console.WriteLine();
                    WriteWrapped(paragraph, 60);
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        #endregion

        #region Lore Codex

        /// <summary>
        /// Displays the full Lore Codex menu, giving players access to all lore categories.
        /// </summary>
        public static void DisplayLoreCodex()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║  📚 THE LORE CODEX                                          ║");
                Console.WriteLine("║     A compendium of the world's history and secrets          ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("  [1] 🌍 World History");
                Console.WriteLine("  [2] ⏳ Historical Timeline");
                Console.WriteLine("  [3] 🏰 Major Town Lore");
                Console.WriteLine("  [4] 🏘️  Settlement Lore");
                Console.WriteLine("  [5] 🗺️  Region Lore");
                Console.WriteLine("  [6] 👤 Race Lore");
                Console.WriteLine("  [7] ⚔️  Enemy Faction Lore");
                Console.WriteLine("  [8] 🏆 Champion Class Lore");
                Console.WriteLine("  [9] ✨ Legendary Artifact Lore");
                Console.WriteLine("  [10] 🌟 Mythic Title Lore");
                Console.WriteLine("  [0] Return");
                Console.Write("\nChoose a category: ");

                var input = Console.ReadLine()?.Trim() ?? "";
                switch (input)
                {
                    case "1":
                        DisplayWorldHistory();
                        break;
                    case "2":
                        DisplayTimeline();
                        break;
                    case "3":
                        DisplayLoreList("Major Towns", TownLoreEntries, DisplayTownLore);
                        break;
                    case "4":
                        DisplayLoreList("Settlements", SettlementLoreEntries, DisplaySettlementLore);
                        break;
                    case "5":
                        DisplayLoreList("Regions", RegionLoreEntries, DisplayRegionLore);
                        break;
                    case "6":
                        DisplayLoreList("Races", RaceLoreEntries, DisplayRaceLore);
                        break;
                    case "7":
                        DisplayFactionLoreList();
                        break;
                    case "8":
                        DisplayLoreList("Champion Classes", ChampionLoreEntries, DisplayChampionLore);
                        break;
                    case "9":
                        DisplayLoreList("Legendary Artifacts", ArtifactLoreEntries, DisplayArtifactLore);
                        break;
                    case "10":
                        DisplayLoreList("Mythic Titles", MythicTitleLoreEntries, DisplayMythicTitleLore);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void DisplayLoreList(string categoryName, Dictionary<string, string[]> entries, Action<string> displayAction)
        {
            var keys = new List<string>(entries.Keys);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 {categoryName,-55}║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            for (int i = 0; i < keys.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {keys[i]}");
            }
            Console.WriteLine($"  [0] Return");
            Console.Write("\nChoose an entry: ");

            var input = Console.ReadLine()?.Trim() ?? "";
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= keys.Count)
            {
                displayAction(keys[choice - 1]);
            }
        }

        private static void DisplayFactionLoreList()
        {
            var factions = (EnemyCampType[])Enum.GetValues(typeof(EnemyCampType));

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  📜 Enemy Factions                                           ║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");

            for (int i = 0; i < factions.Length; i++)
            {
                Console.WriteLine($"  [{i + 1}] {GetFactionName(factions[i])}");
            }
            Console.WriteLine($"  [0] Return");
            Console.Write("\nChoose a faction: ");

            var input = Console.ReadLine()?.Trim() ?? "";
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= factions.Length)
            {
                DisplayFactionLore(factions[choice - 1]);
            }
        }

        #endregion

        #region World History

        /// <summary>
        /// Displays the overarching world history / creation myth.
        /// </summary>
        public static void DisplayWorldHistory()
        {
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  📜 The History of the Known World                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

            string[] history =
            [
                "In the age before memory, the world was whole—a single vast continent united under a civilization so advanced that their works still defy understanding.",
                "Then came the Sundering: a cataclysm of magical energy that shattered the continent, reshaped the coastlines, and unleashed forces that would haunt the world for millennia.",
                "The cause of the Sundering remains debated. Some blame a failed ritual of unimaginable scale. Others point to a war between gods. The elves, who remember fragments of that time, speak only of 'the Mistake.'",
                "In the aftermath, the surviving races—humans, elves, dwarves, and gnomes—rebuilt from the ruins. New kingdoms rose and fell. The Old Empire united much of the known world before it, too, collapsed under the weight of corruption and civil war.",
                "The War of Dawn, fought a century ago against the Dread Lord Morvath, forged the current alliance between the races. Though Morvath was defeated and his fortress repurposed as Shadowkeep, his influence lingers in the undead that still plague the land.",
                "Now, the world stands at a crossroads. New threats emerge—demon portals open with increasing frequency, ancient beasts stir in forgotten lairs, and the Sundering's residual magic continues to warp the land. Heroes are needed more than ever."
            ];

            foreach (var paragraph in history)
            {
                Console.WriteLine();
                WriteWrapped(paragraph, 60);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        #endregion

        #region Utility

        private static void WriteWrapped(string text, int width)
        {
            var words = text.Split(' ');
            int currentLineLength = 0;
            string indent = "   ";

            Console.Write(indent);
            currentLineLength = indent.Length;

            foreach (var word in words)
            {
                if (currentLineLength + word.Length + 1 > width + indent.Length && currentLineLength > indent.Length)
                {
                    Console.WriteLine();
                    Console.Write(indent);
                    currentLineLength = indent.Length;
                }

                Console.Write(word + " ");
                currentLineLength += word.Length + 1;
            }

            Console.WriteLine();
        }

        #endregion
    }

    #endregion
}
