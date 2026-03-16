using Night.Characters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rpg_Dungeon
{
    internal class Options
    {
        #region Fields

        // Tracks whether the party has set up camp during this session.
        private static bool _hasCamped = false;

        #endregion

        #region Options Menu

        // Show the options menu. The menu enforces that the party must set up camp before saving or exiting.
        // Returns a loaded party if the player chooses to load a save; otherwise returns null.
        public static List<Character>? ShowOptions(List<Character> party, bool isMultiplayer = false)
        {
            if (party == null) party = new List<Character>();

            while (true)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1) Set up camp");
                Console.WriteLine("2) Save game (requires camp)");
                Console.WriteLine("3) Load game");
                Console.WriteLine("4) Exit game (requires camp)");
                Console.WriteLine("5) Return");
                Console.Write("Choose an option: ");
                var input = Console.ReadLine() ?? string.Empty;

                switch (input.Trim())
                {
                    case "1":
                        Camping.CampMenu(party, null, null);
                        _hasCamped = true;
                        break;
                    case "2":
                        if (!_hasCamped)
                        {
                            Console.WriteLine("You must set up camp before saving the game.");
                        }
                        else
                        {
                            SaveGameJson(party, isMultiplayer);
                        }
                        break;
                    case "3":
                        var loaded = LoadGameJson();
                        if (loaded != null)
                        {
                            Console.WriteLine("Game loaded.");
                            return loaded;
                        }
                        break;
                    case "4":
                        if (!_hasCamped)
                        {
                            Console.WriteLine("You must set up camp before exiting safely. Do you want to set up camp now? (y/n)");
                            var ans = Console.ReadLine() ?? string.Empty;
                            if (ans.Trim().ToLowerInvariant() == "y")
                            {
                                Camping.CampMenu(party, null, null);
                                _hasCamped = true;
                                // fall through to prompt exit next loop
                            }
                            else
                            {
                                Console.WriteLine("Exit cancelled. You may return to options.");
                            }
                        }
                        else
                        {
                            if (ConfirmExit()) Environment.Exit(0);
                        }
                        break;
                    case "5":
                        return null;
                    default:
                        Console.WriteLine("Invalid selection. Enter 1-5.");
                        break;
                }
            }
        }

        #endregion

        #region Helper Methods

        private static bool ConfirmExit()
        {
            Console.Write("Are you sure you want to exit the game? (y/n): ");
            var ans = Console.ReadLine() ?? string.Empty;
            return ans.Trim().ToLowerInvariant() == "y";
        }

        // Allow external callers to mark that the party has camped (so save/exit become available)
        public static void MarkCamped()
        {
            _hasCamped = true;
        }

        #endregion

        #region Save System

        private static void ManageSaveLimit(bool isMultiplayer)
        {
            try
            {
                var filePattern = isMultiplayer ? "mpsave_*.json" : "save_*.json";
                var saveFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), filePattern)
                    .OrderBy(f => File.GetLastWriteTime(f))
                    .ToArray();

                if (saveFiles.Length >= 10)
                {
                    int toDelete = saveFiles.Length - 9;
                    Console.WriteLine($"\n⚠️  Save limit reached! Removing {toDelete} oldest save(s)...");

                    for (int i = 0; i < toDelete; i++)
                    {
                        var oldFile = saveFiles[i];
                        try
                        {
                            var json = File.ReadAllText(oldFile, Encoding.UTF8);
                            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var save = JsonSerializer.Deserialize<SaveFile>(json, opts);
                            var saveName = save?.SaveName ?? Path.GetFileName(oldFile);
                            File.Delete(oldFile);
                            Console.WriteLine($"   🗑️  Deleted: '{saveName}'");
                        }
                        catch
                        {
                            File.Delete(oldFile);
                            Console.WriteLine($"   🗑️  Deleted: {Path.GetFileName(oldFile)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Warning: Could not manage save limit: {ex.Message}");
            }
        }

        // Very simple save: writes party summary to a timestamped file in the current directory.
        private static void SaveGameJson(List<Character> party, bool isMultiplayer)
        {
            try
            {
                Console.WriteLine("\n💾 SAVE GAME");
                Console.WriteLine("══════════════════════════════════════════════════════════════════");
                Console.Write("\nEnter a name for this save (leave empty for auto-name): ");
                var saveName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(saveName))
                {
                    saveName = $"Save {DateTime.Now:yyyy-MM-dd HH:mm}";
                }

                ManageSaveLimit(isMultiplayer);

                var filePrefix = isMultiplayer ? "mpsave" : "save";
                var fileName = $"{filePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                var save = new SaveFile
                {
                    SaveName = saveName,
                    IsMultiplayer = isMultiplayer,
                    Created = DateTime.Now,
                    Party = party.Select(p => new CharacterData
                    {
                        Class = p.GetType().Name,
                        Name = p.Name,
                        Level = p.Level,
                        Experience = p.Experience,
                        Health = p.Health,
                        MaxHealth = p.MaxHealth,
                        Mana = p.Mana,
                        MaxMana = p.MaxMana,
                        Stamina = p.Stamina,
                        MaxStamina = p.MaxStamina,
                        Strength = p.Strength,
                        Agility = p.Agility,
                        Intelligence = p.Intelligence,
                        ChampionClass = p.ChampionClass,
                        Pet = p.Pet == null ? null : new PetData
                        {
                            Name = p.Pet.Name,
                            Type = p.Pet.Type.ToString(),
                            Ability = p.Pet.Ability.ToString(),
                            Level = p.Pet.Level,
                            Experience = p.Pet.Experience,
                            Loyalty = p.Pet.Loyalty
                        },
                        SkillTree = p.SkillTree == null ? null : new SkillTreeData
                        {
                            SkillPoints = p.SkillTree.SkillPoints,
                            LearnedSkills = p.SkillTree.LearnedSkills.Select(kvp => new LearnedSkillData
                            {
                                SkillName = kvp.Key,
                                CurrentRank = kvp.Value.CurrentRank
                            }).ToList()
                        },
                        Inventory = new InventoryData
                        {
                            Gold = p.Inventory.Gold,
                            ExtraSlots = p.Inventory.TotalSlots - p.Inventory.BaseSlots,
                            EquippedWeapon = p.Inventory.EquippedWeapon == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedWeapon.Name,
                                Price = p.Inventory.EquippedWeapon.Price,
                                EquipmentType = p.Inventory.EquippedWeapon.Type.ToString(),
                                Durability = p.Inventory.EquippedWeapon.Durability,
                                MaxDurability = p.Inventory.EquippedWeapon.MaxDurability,
                                StrengthBonus = p.Inventory.EquippedWeapon.StrengthBonus,
                                AgilityBonus = p.Inventory.EquippedWeapon.AgilityBonus,
                                IntelligenceBonus = p.Inventory.EquippedWeapon.IntelligenceBonus,
                                MaxHPBonus = p.Inventory.EquippedWeapon.MaxHPBonus,
                                MaxManaBonus = p.Inventory.EquippedWeapon.MaxManaBonus,
                                MaxStaminaBonus = p.Inventory.EquippedWeapon.MaxStaminaBonus,
                                ArmorBonus = p.Inventory.EquippedWeapon.ArmorBonus
                            },
                            EquippedArmor = p.Inventory.EquippedArmor == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedArmor.Name,
                                Price = p.Inventory.EquippedArmor.Price,
                                EquipmentType = p.Inventory.EquippedArmor.Type.ToString(),
                                Durability = p.Inventory.EquippedArmor.Durability,
                                MaxDurability = p.Inventory.EquippedArmor.MaxDurability,
                                StrengthBonus = p.Inventory.EquippedArmor.StrengthBonus,
                                AgilityBonus = p.Inventory.EquippedArmor.AgilityBonus,
                                IntelligenceBonus = p.Inventory.EquippedArmor.IntelligenceBonus,
                                MaxHPBonus = p.Inventory.EquippedArmor.MaxHPBonus,
                                MaxManaBonus = p.Inventory.EquippedArmor.MaxManaBonus,
                                MaxStaminaBonus = p.Inventory.EquippedArmor.MaxStaminaBonus,
                                ArmorBonus = p.Inventory.EquippedArmor.ArmorBonus
                            },
                            EquippedAccessory = p.Inventory.EquippedAccessory == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedAccessory.Name,
                                Price = p.Inventory.EquippedAccessory.Price,
                                EquipmentType = p.Inventory.EquippedAccessory.Type.ToString(),
                                Durability = p.Inventory.EquippedAccessory.Durability,
                                MaxDurability = p.Inventory.EquippedAccessory.MaxDurability,
                                StrengthBonus = p.Inventory.EquippedAccessory.StrengthBonus,
                                AgilityBonus = p.Inventory.EquippedAccessory.AgilityBonus,
                                IntelligenceBonus = p.Inventory.EquippedAccessory.IntelligenceBonus,
                                MaxHPBonus = p.Inventory.EquippedAccessory.MaxHPBonus,
                                MaxManaBonus = p.Inventory.EquippedAccessory.MaxManaBonus,
                                MaxStaminaBonus = p.Inventory.EquippedAccessory.MaxStaminaBonus,
                                ArmorBonus = p.Inventory.EquippedAccessory.ArmorBonus
                            },
                            EquippedNecklace = p.Inventory.EquippedNecklace == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedNecklace.Name,
                                Price = p.Inventory.EquippedNecklace.Price,
                                EquipmentType = p.Inventory.EquippedNecklace.Type.ToString(),
                                Durability = p.Inventory.EquippedNecklace.Durability,
                                MaxDurability = p.Inventory.EquippedNecklace.MaxDurability,
                                StrengthBonus = p.Inventory.EquippedNecklace.StrengthBonus,
                                AgilityBonus = p.Inventory.EquippedNecklace.AgilityBonus,
                                IntelligenceBonus = p.Inventory.EquippedNecklace.IntelligenceBonus,
                                MaxHPBonus = p.Inventory.EquippedNecklace.MaxHPBonus,
                                MaxManaBonus = p.Inventory.EquippedNecklace.MaxManaBonus,
                                MaxStaminaBonus = p.Inventory.EquippedNecklace.MaxStaminaBonus,
                                ArmorBonus = p.Inventory.EquippedNecklace.ArmorBonus
                            },
                            EquippedRing1 = p.Inventory.EquippedRing1 == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedRing1.Name,
                                Price = p.Inventory.EquippedRing1.Price,
                                EquipmentType = p.Inventory.EquippedRing1.Type.ToString(),
                                Durability = p.Inventory.EquippedRing1.Durability,
                                MaxDurability = p.Inventory.EquippedRing1.MaxDurability,
                                StrengthBonus = p.Inventory.EquippedRing1.StrengthBonus,
                                AgilityBonus = p.Inventory.EquippedRing1.AgilityBonus,
                                IntelligenceBonus = p.Inventory.EquippedRing1.IntelligenceBonus,
                                MaxHPBonus = p.Inventory.EquippedRing1.MaxHPBonus,
                                MaxManaBonus = p.Inventory.EquippedRing1.MaxManaBonus,
                                MaxStaminaBonus = p.Inventory.EquippedRing1.MaxStaminaBonus,
                                ArmorBonus = p.Inventory.EquippedRing1.ArmorBonus
                            },
                            EquippedRing2 = p.Inventory.EquippedRing2 == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedRing2.Name,
                                Price = p.Inventory.EquippedRing2.Price,
                                EquipmentType = p.Inventory.EquippedRing2.Type.ToString(),
                                Durability = p.Inventory.EquippedRing2.Durability,
                                MaxDurability = p.Inventory.EquippedRing2.MaxDurability,
                                StrengthBonus = p.Inventory.EquippedRing2.StrengthBonus,
                                AgilityBonus = p.Inventory.EquippedRing2.AgilityBonus,
                                IntelligenceBonus = p.Inventory.EquippedRing2.IntelligenceBonus,
                                MaxHPBonus = p.Inventory.EquippedRing2.MaxHPBonus,
                                MaxManaBonus = p.Inventory.EquippedRing2.MaxManaBonus,
                                MaxStaminaBonus = p.Inventory.EquippedRing2.MaxStaminaBonus,
                                ArmorBonus = p.Inventory.EquippedRing2.ArmorBonus
                            },
                            EquippedOffHand = p.Inventory.EquippedOffHand == null ? null : new ItemData
                            {
                                Type = p.Inventory.EquippedOffHand is Torch ? "Torch" : "Equipment",
                                Name = p.Inventory.EquippedOffHand.Name,
                                Price = p.Inventory.EquippedOffHand.Price,
                                EquipmentType = p.Inventory.EquippedOffHand.Type.ToString(),
                                Durability = p.Inventory.EquippedOffHand.Durability,
                                MaxDurability = p.Inventory.EquippedOffHand.MaxDurability,
                                IsLit = p.Inventory.EquippedOffHand is Torch t && t.IsLit,
                                BurnTimeHours = p.Inventory.EquippedOffHand is Torch torch ? torch.BurnTimeHours : 0,
                                StrengthBonus = p.Inventory.EquippedOffHand.StrengthBonus,
                                AgilityBonus = p.Inventory.EquippedOffHand.AgilityBonus,
                                IntelligenceBonus = p.Inventory.EquippedOffHand.IntelligenceBonus,
                                MaxHPBonus = p.Inventory.EquippedOffHand.MaxHPBonus,
                                MaxManaBonus = p.Inventory.EquippedOffHand.MaxManaBonus,
                                MaxStaminaBonus = p.Inventory.EquippedOffHand.MaxStaminaBonus,
                                ArmorBonus = p.Inventory.EquippedOffHand.ArmorBonus
                            },
                            Slots = p.Inventory.Slots.Select(it => it == null ? null : new ItemData
                            {
                                Type = it is Torch ? "Torch" : it is Backpack ? "Backpack" : it is Equipment ? "Equipment" : "Generic",
                                Name = it.Name,
                                Price = it.Price,
                                Slots = it is Backpack b ? b.Slots : 0,
                                EquipmentType = it is Equipment eq ? eq.Type.ToString() : "",
                                Durability = it is Equipment e ? e.Durability : 0,
                                MaxDurability = it is Equipment e2 ? e2.MaxDurability : 0,
                                IsLit = it is Torch torch && torch.IsLit,
                                BurnTimeHours = it is Torch t ? t.BurnTimeHours : 0,
                                StrengthBonus = it is Equipment ex ? ex.StrengthBonus : 0,
                                AgilityBonus = it is Equipment ex2 ? ex2.AgilityBonus : 0,
                                IntelligenceBonus = it is Equipment ex3 ? ex3.IntelligenceBonus : 0,
                                MaxHPBonus = it is Equipment ex4 ? ex4.MaxHPBonus : 0,
                                MaxManaBonus = it is Equipment ex5 ? ex5.MaxManaBonus : 0,
                                MaxStaminaBonus = it is Equipment ex6 ? ex6.MaxStaminaBonus : 0,
                                ArmorBonus = it is Equipment ex7 ? ex7.ArmorBonus : 0
                            }).ToList()
                        }
                    }).ToList()
                };

                var opts = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(save, opts);
                File.WriteAllText(fileName, json, Encoding.UTF8);
                Console.WriteLine($"\n✅ Game saved: '{saveName}'");
                Console.WriteLine($"   File: {fileName}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save game: {ex.Message}");
            }
        }

        public static void SaveGameAutomatic(List<Character> party, bool isMultiplayer = false)
        {
            try
            {
                ManageSaveLimit(isMultiplayer);

                var filePrefix = isMultiplayer ? "mpsave" : "save";
                var fileName = $"{filePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var saveName = $"Auto-Save {DateTime.Now:yyyy-MM-dd HH:mm}";

                var save = new SaveFile
                {
                    SaveName = saveName,
                    IsMultiplayer = isMultiplayer,
                    Created = DateTime.Now,
                    Party = party.Select(p => new CharacterData
                    {
                        Class = p.GetType().Name,
                        Name = p.Name,
                        Level = p.Level,
                        Experience = p.Experience,
                        Health = p.Health,
                        MaxHealth = p.MaxHealth,
                        Mana = p.Mana,
                        MaxMana = p.MaxMana,
                        Stamina = p.Stamina,
                        MaxStamina = p.MaxStamina,
                        Strength = p.Strength,
                        Agility = p.Agility,
                        Intelligence = p.Intelligence,
                        ChampionClass = p.ChampionClass,
                        Pet = p.Pet == null ? null : new PetData
                        {
                            Name = p.Pet.Name,
                            Type = p.Pet.Type.ToString(),
                            Ability = p.Pet.Ability.ToString(),
                            Level = p.Pet.Level,
                            Experience = p.Pet.Experience,
                            Loyalty = p.Pet.Loyalty
                        },
                        SkillTree = p.SkillTree == null ? null : new SkillTreeData
                        {
                            SkillPoints = p.SkillTree.SkillPoints,
                            LearnedSkills = p.SkillTree.LearnedSkills.Select(kvp => new LearnedSkillData
                            {
                                SkillName = kvp.Key,
                                CurrentRank = kvp.Value.CurrentRank
                            }).ToList()
                        },
                        Inventory = new InventoryData
                        {
                            Gold = p.Inventory.Gold,
                            ExtraSlots = p.Inventory.TotalSlots - p.Inventory.BaseSlots,
                            EquippedWeapon = p.Inventory.EquippedWeapon == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedWeapon.Name,
                                Price = p.Inventory.EquippedWeapon.Price,
                                EquipmentType = p.Inventory.EquippedWeapon.Type.ToString(),
                                Durability = p.Inventory.EquippedWeapon.Durability,
                                MaxDurability = p.Inventory.EquippedWeapon.MaxDurability
                            },
                            EquippedArmor = p.Inventory.EquippedArmor == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedArmor.Name,
                                Price = p.Inventory.EquippedArmor.Price,
                                EquipmentType = p.Inventory.EquippedArmor.Type.ToString(),
                                Durability = p.Inventory.EquippedArmor.Durability,
                                MaxDurability = p.Inventory.EquippedArmor.MaxDurability
                            },
                            EquippedAccessory = p.Inventory.EquippedAccessory == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedAccessory.Name,
                                Price = p.Inventory.EquippedAccessory.Price,
                                EquipmentType = p.Inventory.EquippedAccessory.Type.ToString(),
                                Durability = p.Inventory.EquippedAccessory.Durability,
                                MaxDurability = p.Inventory.EquippedAccessory.MaxDurability
                            },
                            EquippedNecklace = p.Inventory.EquippedNecklace == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedNecklace.Name,
                                Price = p.Inventory.EquippedNecklace.Price,
                                EquipmentType = p.Inventory.EquippedNecklace.Type.ToString(),
                                Durability = p.Inventory.EquippedNecklace.Durability,
                                MaxDurability = p.Inventory.EquippedNecklace.MaxDurability
                            },
                            EquippedRing1 = p.Inventory.EquippedRing1 == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedRing1.Name,
                                Price = p.Inventory.EquippedRing1.Price,
                                EquipmentType = p.Inventory.EquippedRing1.Type.ToString(),
                                Durability = p.Inventory.EquippedRing1.Durability,
                                MaxDurability = p.Inventory.EquippedRing1.MaxDurability
                            },
                            EquippedRing2 = p.Inventory.EquippedRing2 == null ? null : new ItemData
                            {
                                Type = "Equipment",
                                Name = p.Inventory.EquippedRing2.Name,
                                Price = p.Inventory.EquippedRing2.Price,
                                EquipmentType = p.Inventory.EquippedRing2.Type.ToString(),
                                Durability = p.Inventory.EquippedRing2.Durability,
                                MaxDurability = p.Inventory.EquippedRing2.MaxDurability
                            },
                            EquippedOffHand = p.Inventory.EquippedOffHand == null ? null : new ItemData
                            {
                                Type = p.Inventory.EquippedOffHand is Torch ? "Torch" : "Equipment",
                                Name = p.Inventory.EquippedOffHand.Name,
                                Price = p.Inventory.EquippedOffHand.Price,
                                EquipmentType = p.Inventory.EquippedOffHand.Type.ToString(),
                                Durability = p.Inventory.EquippedOffHand.Durability,
                                MaxDurability = p.Inventory.EquippedOffHand.MaxDurability,
                                IsLit = p.Inventory.EquippedOffHand is Torch t && t.IsLit,
                                BurnTimeHours = p.Inventory.EquippedOffHand is Torch torch ? torch.BurnTimeHours : 0
                            },
                            Slots = p.Inventory.Slots.Select(it => it == null ? null : new ItemData
                            {
                                Type = it is Torch ? "Torch" : it is Backpack ? "Backpack" : it is Equipment ? "Equipment" : "Generic",
                                Name = it.Name,
                                Price = it.Price,
                                Slots = it is Backpack b ? b.Slots : 0,
                                EquipmentType = it is Equipment eq ? eq.Type.ToString() : "",
                                Durability = it is Equipment e ? e.Durability : 0,
                                MaxDurability = it is Equipment e2 ? e2.MaxDurability : 0,
                                IsLit = it is Torch torch && torch.IsLit,
                                BurnTimeHours = it is Torch t ? t.BurnTimeHours : 0
                            }).ToList()
                        }
                    }).ToList()
                };

                var opts = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(save, opts);
                File.WriteAllText(fileName, json, Encoding.UTF8);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Auto-save failed: {ex.Message}");
            }
        }

        #endregion

        #region Load System

        private static List<Character>? LoadGameJson()
        {
            try
            {
                Console.WriteLine("Enter save filename to load (or press enter to list saves):");
                var input = Console.ReadLine() ?? string.Empty;
                string file = input;
                if (string.IsNullOrWhiteSpace(file))
                {
                    var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "save_*.json");
                    if (files.Length == 0)
                    {
                        Console.WriteLine("No save files found.");
                        return null;
                    }
                    for (int i = 0; i < files.Length; i++) Console.WriteLine($"{i + 1}) {Path.GetFileName(files[i])}");
                    Console.WriteLine("Choose save number to load:");
                    var sel = Console.ReadLine() ?? string.Empty;
                    if (!int.TryParse(sel, out var idx) || idx < 1 || idx > files.Length) { Console.WriteLine("Invalid."); return null; }
                    file = files[idx - 1];
                }

                if (!File.Exists(file))
                {
                    Console.WriteLine("File not found.");
                    return null;
                }

                var json = File.ReadAllText(file, Encoding.UTF8);
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var save = JsonSerializer.Deserialize<SaveFile>(json, opts);
                if (save == null) return null;

                var party = new List<Character>();
                foreach (var cd in save.Party)
                {
                    Character ch = cd.Class switch
                    {
                        "Warrior" => new Warrior(cd.Name),
                        "Mage" => new Mage(cd.Name),
                        "Rogue" => new Rogue(cd.Name),
                        "Priest" => new Priest(cd.Name),
                        _ => new Warrior(cd.Name)
                    };

                    // Apply race bonuses if present in save
                    try
                    {
                        if (!string.IsNullOrEmpty(cd.Race))
                        {
                            // Map race name to Race object
                            Race race = cd.Race switch
                            {
                                "Elf" => new Elf(),
                                "Dwarf" => new Dwarf(),
                                "Gnome" => new Gnome(),
                                _ => new Human()
                            };
                            ch.ApplyRaceBonuses(race);
                            ch.RaceName = race.Name;
                        }
                    }
                    catch { }

                    // restore stats
                    ch.RestoreProgress(cd.Level, cd.Experience, cd.Health, cd.MaxHealth, cd.Mana, cd.MaxMana, cd.Stamina, cd.MaxStamina, cd.Strength, cd.Agility, cd.Intelligence);

                    // Restore champion class if present
                    if (!string.IsNullOrEmpty(cd.ChampionClass))
                    {
                        ch.ChampionClass = cd.ChampionClass;
                    }

                    // Restore pet if present
                    if (cd.Pet != null)
                    {
                        var petType = Enum.TryParse<PetType>(cd.Pet.Type, out var type) ? type : PetType.Wolf;
                        var petAbility = Enum.TryParse<PetAbility>(cd.Pet.Ability, out var ability) ? ability : PetAbility.DamageBoost;
                        var pet = new Pet(cd.Pet.Name, petType, petAbility);

                        // Restore pet progress
                        for (int i = 1; i < cd.Pet.Level; i++)
                        {
                            pet.GainExperience(pet.ExperienceToLevel);
                        }
                        pet.GainExperience(cd.Pet.Experience);
                        pet.IncreaseLoyalty(cd.Pet.Loyalty - 50); // Base loyalty is 50

                        ch.Pet = pet;
                    }

                    // Restore skill tree if present
                    if (cd.SkillTree != null && ch.SkillTree != null)
                    {
                        // Restore skill points
                        for (int i = 0; i < cd.SkillTree.SkillPoints; i++)
                        {
                            ch.SkillTree.AddSkillPoint();
                        }

                        // Restore learned skills
                        foreach (var learnedSkill in cd.SkillTree.LearnedSkills)
                        {
                            if (ch.SkillTree.AvailableSkills.TryGetValue(learnedSkill.SkillName, out var skill))
                            {
                                // Increase rank to match saved rank
                                for (int r = 0; r < learnedSkill.CurrentRank; r++)
                                {
                                    skill.IncreaseRank();
                                }

                                // Add to learned skills if not already there
                                if (!ch.SkillTree.LearnedSkills.ContainsKey(learnedSkill.SkillName))
                                {
                                    ch.SkillTree.LearnedSkills.Add(learnedSkill.SkillName, skill);
                                }
                            }
                        }
                    }

                    // restore inventory
                    if (cd.Inventory != null)
                    {
                        ch.Inventory.AddGold(cd.Inventory.Gold);

                        // Restore inventory items
                        if (cd.Inventory.Slots != null)
                        {
                            foreach (var it in cd.Inventory.Slots)
                            {
                                if (it == null) continue;
                                Item item;
                                if (it.Type == "Backpack")
                                    item = new Backpack(it.Name, it.Slots, it.Price);
                                else if (it.Type == "Torch")
                                {
                                    var torch = new Torch(it.Name, it.MaxDurability, it.Price);
                                    if (it.Durability < torch.MaxBurnTimeHours)
                                        torch.Burn(torch.MaxBurnTimeHours - it.BurnTimeHours);
                                    if (it.IsLit)
                                        torch.Light();
                                    item = torch;
                                }
                                else if (it.Type == "Equipment")
                                {
                                    var eqType = Enum.TryParse<EquipmentType>(it.EquipmentType, out var type) ? type : EquipmentType.Weapon;
                                    var eq = new Equipment(it.Name, eqType, it.MaxDurability, it.Price);
                                    if (it.Durability < eq.MaxDurability)
                                        eq.Damage(eq.MaxDurability - it.Durability);
                                    item = eq;
                                }
                                else
                                    item = new GenericItem(it.Name, it.Price);
                                ch.Inventory.AddItem(item);
                            }
                        }

                        // Restore equipped items
                        if (cd.Inventory.EquippedWeapon != null)
                        {
                            var eqType = Enum.TryParse<EquipmentType>(cd.Inventory.EquippedWeapon.EquipmentType, out var type) ? type : EquipmentType.Weapon;
                            var weapon = new Equipment(cd.Inventory.EquippedWeapon.Name, eqType, cd.Inventory.EquippedWeapon.MaxDurability, cd.Inventory.EquippedWeapon.Price);
                            if (cd.Inventory.EquippedWeapon.Durability < weapon.MaxDurability)
                                weapon.Damage(weapon.MaxDurability - cd.Inventory.EquippedWeapon.Durability);
                            ch.Inventory.EquipItem(weapon, EquipmentSlot.Weapon);
                        }

                        if (cd.Inventory.EquippedArmor != null)
                        {
                            var eqType = Enum.TryParse<EquipmentType>(cd.Inventory.EquippedArmor.EquipmentType, out var type) ? type : EquipmentType.Armor;
                            var armor = new Equipment(cd.Inventory.EquippedArmor.Name, eqType, cd.Inventory.EquippedArmor.MaxDurability, cd.Inventory.EquippedArmor.Price);
                            if (cd.Inventory.EquippedArmor.Durability < armor.MaxDurability)
                                armor.Damage(armor.MaxDurability - cd.Inventory.EquippedArmor.Durability);
                            ch.Inventory.EquipItem(armor, EquipmentSlot.Armor);
                        }

                        if (cd.Inventory.EquippedAccessory != null)
                        {
                            var eqType = Enum.TryParse<EquipmentType>(cd.Inventory.EquippedAccessory.EquipmentType, out var type) ? type : EquipmentType.Accessory;
                            var accessory = new Equipment(cd.Inventory.EquippedAccessory.Name, eqType, cd.Inventory.EquippedAccessory.MaxDurability, cd.Inventory.EquippedAccessory.Price);
                            if (cd.Inventory.EquippedAccessory.Durability < accessory.MaxDurability)
                                accessory.Damage(accessory.MaxDurability - cd.Inventory.EquippedAccessory.Durability);
                            ch.Inventory.EquipItem(accessory, EquipmentSlot.Accessory);
                        }

                        if (cd.Inventory.EquippedNecklace != null)
                        {
                            var eqType = Enum.TryParse<EquipmentType>(cd.Inventory.EquippedNecklace.EquipmentType, out var type) ? type : EquipmentType.Necklace;
                            var necklace = new Equipment(cd.Inventory.EquippedNecklace.Name, eqType, cd.Inventory.EquippedNecklace.MaxDurability, cd.Inventory.EquippedNecklace.Price);
                            if (cd.Inventory.EquippedNecklace.Durability < necklace.MaxDurability)
                                necklace.Damage(necklace.MaxDurability - cd.Inventory.EquippedNecklace.Durability);
                            ch.Inventory.EquipItem(necklace, EquipmentSlot.Necklace);
                        }

                        if (cd.Inventory.EquippedRing1 != null)
                        {
                            var eqType = Enum.TryParse<EquipmentType>(cd.Inventory.EquippedRing1.EquipmentType, out var type) ? type : EquipmentType.Ring;
                            var ring1 = new Equipment(cd.Inventory.EquippedRing1.Name, eqType, cd.Inventory.EquippedRing1.MaxDurability, cd.Inventory.EquippedRing1.Price);
                            if (cd.Inventory.EquippedRing1.Durability < ring1.MaxDurability)
                                ring1.Damage(ring1.MaxDurability - cd.Inventory.EquippedRing1.Durability);
                            ch.Inventory.EquipItem(ring1, EquipmentSlot.Ring1);
                        }

                        if (cd.Inventory.EquippedRing2 != null)
                        {
                            var eqType = Enum.TryParse<EquipmentType>(cd.Inventory.EquippedRing2.EquipmentType, out var type) ? type : EquipmentType.Ring;
                            var ring2 = new Equipment(cd.Inventory.EquippedRing2.Name, eqType, cd.Inventory.EquippedRing2.MaxDurability, cd.Inventory.EquippedRing2.Price);
                            if (cd.Inventory.EquippedRing2.Durability < ring2.MaxDurability)
                                ring2.Damage(ring2.MaxDurability - cd.Inventory.EquippedRing2.Durability);
                            ch.Inventory.EquipItem(ring2, EquipmentSlot.Ring2);
                        }

                        if (cd.Inventory.EquippedOffHand != null)
                        {
                            Equipment offHand;
                            if (cd.Inventory.EquippedOffHand.Type == "Torch")
                            {
                                var torch = new Torch(cd.Inventory.EquippedOffHand.Name, cd.Inventory.EquippedOffHand.MaxDurability, cd.Inventory.EquippedOffHand.Price);
                                if (cd.Inventory.EquippedOffHand.BurnTimeHours < torch.MaxBurnTimeHours)
                                    torch.Burn(torch.MaxBurnTimeHours - cd.Inventory.EquippedOffHand.BurnTimeHours);
                                if (cd.Inventory.EquippedOffHand.IsLit)
                                    torch.Light();
                                offHand = torch;
                            }
                            else
                            {
                                var eqType = Enum.TryParse<EquipmentType>(cd.Inventory.EquippedOffHand.EquipmentType, out var type) ? type : EquipmentType.OffHand;
                                offHand = new Equipment(cd.Inventory.EquippedOffHand.Name, eqType, cd.Inventory.EquippedOffHand.MaxDurability, cd.Inventory.EquippedOffHand.Price);
                                if (cd.Inventory.EquippedOffHand.Durability < offHand.MaxDurability)
                                    offHand.Damage(offHand.MaxDurability - cd.Inventory.EquippedOffHand.Durability);
                            }
                            ch.Inventory.EquipItem(offHand, EquipmentSlot.OffHand);
                        }

                        if (cd.Inventory.ExtraSlots > 0)
                        {
                            // Equip a dummy backpack to set extra slots
                            ch.Inventory.EquipBackpack(new Backpack("Loaded Backpack", cd.Inventory.ExtraSlots, 0));
                        }
                    }

                    party.Add(ch);
                }

                return party;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load game: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Save Data Classes

        // Data transfer objects for save/load
        internal class SaveFile
        {
            public string SaveName { get; set; } = string.Empty;
            public bool IsMultiplayer { get; set; }
            public DateTime Created { get; set; }
            public List<CharacterData> Party { get; set; } = new List<CharacterData>();
        }

        internal class CharacterData
        {
            public string Class { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Race { get; set; } = "Human";
            public int Level { get; set; }
            public int Experience { get; set; }
            public int Health { get; set; }
            public int MaxHealth { get; set; }
            public int Mana { get; set; }
            public int MaxMana { get; set; }
            public int Stamina { get; set; }
            public int MaxStamina { get; set; }
            public int Strength { get; set; }
            public int Agility { get; set; }
            public int Intelligence { get; set; }
            public string? ChampionClass { get; set; }
            public InventoryData? Inventory { get; set; }
            public PetData? Pet { get; set; }
            public SkillTreeData? SkillTree { get; set; }
        }

        internal class InventoryData
        {
            public int Gold { get; set; }
            public int ExtraSlots { get; set; }
            public ItemData? EquippedWeapon { get; set; }
            public ItemData? EquippedArmor { get; set; }
            public ItemData? EquippedAccessory { get; set; }
            public ItemData? EquippedNecklace { get; set; }
            public ItemData? EquippedRing1 { get; set; }
            public ItemData? EquippedRing2 { get; set; }
            public ItemData? EquippedOffHand { get; set; }
            public List<ItemData?>? Slots { get; set; }
        }

        internal class ItemData
        {
            public string Type { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public int Price { get; set; }
            public int Slots { get; set; }
            public string EquipmentType { get; set; } = string.Empty;
            public int Durability { get; set; }
            public int MaxDurability { get; set; }
            public bool IsLit { get; set; }
            public int BurnTimeHours { get; set; }
            // Equipment stat bonuses
            public int StrengthBonus { get; set; }
            public int AgilityBonus { get; set; }
            public int IntelligenceBonus { get; set; }
            public int MaxHPBonus { get; set; }
            public int MaxManaBonus { get; set; }
            public int MaxStaminaBonus { get; set; }
            public int ArmorBonus { get; set; }
        }

        internal class PetData
        {
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Ability { get; set; } = string.Empty;
            public int Level { get; set; }
            public int Experience { get; set; }
            public int Loyalty { get; set; }
        }

        internal class SkillTreeData
        {
            public int SkillPoints { get; set; }
            public List<LearnedSkillData> LearnedSkills { get; set; } = new List<LearnedSkillData>();
        }

        internal class LearnedSkillData
        {
            public string SkillName { get; set; } = string.Empty;
            public int CurrentRank { get; set; }
        }

        #endregion
    }
}
