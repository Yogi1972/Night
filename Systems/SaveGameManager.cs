using Night.Characters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Rpg_Dungeon
{
    internal static class SaveGameManager
    {
        public static List<Character>? LoadSpecificSaveFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"❌ Save file not found: {filePath}");
                    return null;
                }

                var json = File.ReadAllText(filePath, Encoding.UTF8);
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var save = JsonSerializer.Deserialize<Options.SaveFile>(json, opts);
                if (save == null)
                {
                    Console.WriteLine("❌ Failed to deserialize save file.");
                    return null;
                }

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

                    ch.RestoreProgress(cd.Level, cd.Experience, cd.Health, cd.MaxHealth, cd.Mana, cd.MaxMana,
                        cd.Stamina, cd.MaxStamina, cd.Strength, cd.Agility, cd.Intelligence);

                    if (!string.IsNullOrEmpty(cd.ChampionClass))
                    {
                        ch.ChampionClass = cd.ChampionClass;
                    }

                    RestorePet(ch, cd);
                    RestoreSkillTree(ch, cd);
                    RestoreInventory(ch, cd);

                    party.Add(ch);
                }

                return party;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to load save file: {ex.Message}");
                return null;
            }
        }

        private static void RestorePet(Character character, Options.CharacterData data)
        {
            if (data.Pet != null)
            {
                var petType = Enum.TryParse<PetType>(data.Pet.Type, out var type) ? type : PetType.Wolf;
                var petAbility = Enum.TryParse<PetAbility>(data.Pet.Ability, out var ability) ? ability : PetAbility.DamageBoost;
                var pet = new Pet(data.Pet.Name, petType, petAbility);

                for (int i = 1; i < data.Pet.Level; i++)
                {
                    pet.GainExperience(pet.ExperienceToLevel);
                }
                pet.GainExperience(data.Pet.Experience);
                pet.IncreaseLoyalty(data.Pet.Loyalty - 50);
                character.Pet = pet;
            }
        }

        private static void RestoreSkillTree(Character character, Options.CharacterData data)
        {
            if (data.SkillTree != null && character.SkillTree != null)
            {
                for (int i = 0; i < data.SkillTree.SkillPoints; i++)
                {
                    character.SkillTree.AddSkillPoint();
                }

                foreach (var learnedSkill in data.SkillTree.LearnedSkills)
                {
                    if (character.SkillTree.AvailableSkills.TryGetValue(learnedSkill.SkillName, out var skill))
                    {
                        for (int r = 0; r < learnedSkill.CurrentRank; r++)
                        {
                            skill.IncreaseRank();
                        }

                        if (!character.SkillTree.LearnedSkills.ContainsKey(learnedSkill.SkillName))
                        {
                            character.SkillTree.LearnedSkills.Add(learnedSkill.SkillName, skill);
                        }
                    }
                }
            }
        }

        private static void RestoreInventory(Character character, Options.CharacterData data)
        {
            if (data.Inventory != null)
            {
                character.Inventory.AddGold(data.Inventory.Gold);

                if (data.Inventory.Slots != null)
                {
                    foreach (var it in data.Inventory.Slots)
                    {
                        if (it == null) continue;
                        Item item;
                        if (it.Type == "Backpack")
                        {
                            item = new Backpack(it.Name, it.Slots, it.Price);
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
                        {
                            item = new GenericItem(it.Name, it.Price);
                        }
                        character.Inventory.AddItem(item);
                    }
                }

                RestoreEquippedItems(character, data.Inventory);

                if (data.Inventory.ExtraSlots > 0)
                {
                    character.Inventory.EquipBackpack(new Backpack("Loaded Backpack", data.Inventory.ExtraSlots, 0));
                }
            }
        }

        private static void RestoreEquippedItems(Character character, Options.InventoryData inventory)
        {
            if (inventory.EquippedWeapon != null)
            {
                var weapon = CreateEquipmentFromData(inventory.EquippedWeapon);
                character.Inventory.EquipItem(weapon, EquipmentSlot.Weapon);
            }

            if (inventory.EquippedArmor != null)
            {
                var armor = CreateEquipmentFromData(inventory.EquippedArmor);
                character.Inventory.EquipItem(armor, EquipmentSlot.Armor);
            }

            if (inventory.EquippedAccessory != null)
            {
                var accessory = CreateEquipmentFromData(inventory.EquippedAccessory);
                character.Inventory.EquipItem(accessory, EquipmentSlot.Accessory);
            }

            if (inventory.EquippedNecklace != null)
            {
                var necklace = CreateEquipmentFromData(inventory.EquippedNecklace);
                character.Inventory.EquipItem(necklace, EquipmentSlot.Necklace);
            }

            if (inventory.EquippedRing1 != null)
            {
                var ring1 = CreateEquipmentFromData(inventory.EquippedRing1);
                character.Inventory.EquipItem(ring1, EquipmentSlot.Ring1);
            }

            if (inventory.EquippedRing2 != null)
            {
                var ring2 = CreateEquipmentFromData(inventory.EquippedRing2);
                character.Inventory.EquipItem(ring2, EquipmentSlot.Ring2);
            }
        }

        private static Equipment CreateEquipmentFromData(Options.ItemData data)
        {
            var eqType = Enum.TryParse<EquipmentType>(data.EquipmentType, out var type) ? type : EquipmentType.Weapon;
            var equipment = new Equipment(data.Name, eqType, data.MaxDurability, data.Price,
                data.StrengthBonus, data.AgilityBonus, data.IntelligenceBonus,
                data.MaxHPBonus, data.MaxManaBonus, data.MaxStaminaBonus, data.ArmorBonus);
            if (data.Durability < equipment.MaxDurability)
                equipment.Damage(equipment.MaxDurability - data.Durability);
            return equipment;
        }

        private static List<string> GetPartyDescriptions(List<Character> party)
        {
            var descriptions = new List<string>();
            foreach (var c in party)
            {
                descriptions.Add($"{c.Name} ({c.GetType().Name}) - Gold: {c.Inventory.Gold}");
            }
            return descriptions;
        }
    }
}
