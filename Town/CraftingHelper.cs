using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal static class CraftingHelper
    {
        public static Character? SelectCrafter(List<Character> party)
        {
            Console.WriteLine("\n👥 Select crafter:");
            for (int i = 0; i < party.Count; i++)
            {
                var professions = ProfessionManager.GetCharacterProfessions(party[i]);
                var profText = professions.Count > 0
                    ? $" [{string.Join(", ", professions.Select(p => ProfessionManager.GetProfessionIcon(p)))}]"
                    : "";
                Console.WriteLine($"{i + 1}) {party[i].Name}{profText} - Gold: {party[i].Inventory.Gold}");
            }
            Console.Write("Choice: ");
            var s = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(s, out var idx) || idx < 1 || idx > party.Count)
            {
                Console.WriteLine("Invalid selection.");
                return null;
            }
            return party[idx - 1];
        }

        public static Character? SelectCrafterFromList(List<Character> crafters)
        {
            Console.WriteLine("\n👥 Select crafter:");
            for (int i = 0; i < crafters.Count; i++)
            {
                var professions = ProfessionManager.GetCharacterProfessions(crafters[i]);
                var profText = professions.Count > 0
                    ? $" [{string.Join(", ", professions.Select(p => ProfessionManager.GetProfessionIcon(p)))}]"
                    : "";
                Console.WriteLine($"{i + 1}) {crafters[i].Name}{profText} - Gold: {crafters[i].Inventory.Gold}");
            }
            Console.Write("Choice: ");
            var s = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(s, out var idx) || idx < 1 || idx > crafters.Count)
            {
                Console.WriteLine("Invalid selection.");
                return null;
            }
            return crafters[idx - 1];
        }

        public static bool CraftItem(Character crafter, (string name, int count)[] materials, Equipment result, string itemName, CraftingProfession requiredProfession)
        {
            if (requiredProfession != CraftingProfession.None &&
                !ProfessionManager.CanCraftWithProfession(crafter, requiredProfession))
            {
                Console.WriteLine($"\n⚠️ {crafter.Name} needs {requiredProfession} profession to craft this!");
                return false;
            }

            var slots = crafter.Inventory.Slots;
            var materialSlots = new Dictionary<string, List<int>>();

            foreach (var (name, count) in materials)
            {
                materialSlots[name] = new List<int>();
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i]?.Name == name)
                    {
                        materialSlots[name].Add(i);
                    }
                }

                if (materialSlots[name].Count < count)
                {
                    Console.WriteLine($"❌ Missing materials! Need {count}x {name}, have {materialSlots[name].Count}.");
                    return false;
                }
            }

            foreach (var (name, count) in materials)
            {
                var indices = materialSlots[name].Take(count).OrderByDescending(x => x).ToList();
                foreach (var slotIndex in indices)
                {
                    crafter.Inventory.RemoveItem(slotIndex);
                }
            }

            if (crafter.Inventory.AddItem(result))
            {
                return true;
            }
            else
            {
                Console.WriteLine("⚠️ Inventory full! Item crafted but dropped.");
                return false;
            }
        }

        public static bool CraftGenericItem(Character crafter, (string name, int count)[] materials, GenericItem result, string itemName, CraftingProfession requiredProfession, int craftAmount = 1)
        {
            if (requiredProfession != CraftingProfession.None &&
                !ProfessionManager.CanCraftWithProfession(crafter, requiredProfession))
            {
                Console.WriteLine($"\n⚠️ {crafter.Name} needs {requiredProfession} profession to craft this!");
                return false;
            }

            var slots = crafter.Inventory.Slots;
            var materialSlots = new Dictionary<string, List<int>>();

            foreach (var (name, count) in materials)
            {
                materialSlots[name] = new List<int>();
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i]?.Name == name)
                    {
                        materialSlots[name].Add(i);
                    }
                }

                if (materialSlots[name].Count < count)
                {
                    Console.WriteLine($"❌ Missing materials! Need {count}x {name}, have {materialSlots[name].Count}.");
                    return false;
                }
            }

            foreach (var (name, count) in materials)
            {
                var indices = materialSlots[name].Take(count).OrderByDescending(x => x).ToList();
                foreach (var slotIndex in indices)
                {
                    crafter.Inventory.RemoveItem(slotIndex);
                }
            }

            bool allAdded = true;
            for (int i = 0; i < craftAmount; i++)
            {
                var item = new GenericItem(result.Name, result.Price);
                if (!crafter.Inventory.AddItem(item))
                {
                    Console.WriteLine($"⚠️ Inventory full! Only {i} items added.");
                    allAdded = false;
                    break;
                }
            }

            return allAdded || craftAmount > 1;
        }

        public static bool BrewPotion(Character brewer, (string name, int count)[] materials, GenericItem result, int goldCost, string potionName, CraftingProfession requiredProfession)
        {
            if (requiredProfession == CraftingProfession.Alchemy &&
                !ProfessionManager.CanCraftWithProfession(brewer, CraftingProfession.Alchemy) &&
                !(brewer is Mage || brewer is Priest))
            {
                Console.WriteLine($"\n⚠️ {brewer.Name} needs Alchemy profession (or be a Mage/Priest) to brew this!");
                return false;
            }

            if (brewer.Inventory.Gold < goldCost)
            {
                Console.WriteLine($"❌ Not enough gold! Need {goldCost} gold.");
                return false;
            }

            var slots = brewer.Inventory.Slots;
            var materialSlots = new Dictionary<string, List<int>>();

            foreach (var (name, count) in materials)
            {
                materialSlots[name] = new List<int>();
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i]?.Name == name)
                    {
                        materialSlots[name].Add(i);
                    }
                }

                if (materialSlots[name].Count < count)
                {
                    Console.WriteLine($"❌ Missing ingredients! Need {count}x {name}, have {materialSlots[name].Count}.");
                    return false;
                }
            }

            brewer.Inventory.SpendGold(goldCost);

            foreach (var (name, count) in materials)
            {
                var indices = materialSlots[name].Take(count).OrderByDescending(x => x).ToList();
                foreach (var slotIndex in indices)
                {
                    brewer.Inventory.RemoveItem(slotIndex);
                }
            }

            if (brewer.Inventory.AddItem(result))
            {
                return true;
            }
            else
            {
                Console.WriteLine("⚠️ Inventory full! Potion brewed but spilled.");
                return false;
            }
        }

        public static bool CraftTorchItem(Character crafter, (string name, int count)[] materials, Torch result, string itemName, CraftingProfession requiredProfession)
        {
            if (requiredProfession != CraftingProfession.None &&
                !ProfessionManager.CanCraftWithProfession(crafter, requiredProfession))
            {
                Console.WriteLine($"\n⚠️ {crafter.Name} needs {requiredProfession} profession to craft this!");
                return false;
            }

            var slots = crafter.Inventory.Slots;
            var materialSlots = new Dictionary<string, List<int>>();

            foreach (var (name, count) in materials)
            {
                materialSlots[name] = new List<int>();
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i]?.Name == name)
                    {
                        materialSlots[name].Add(i);
                    }
                }

                if (materialSlots[name].Count < count)
                {
                    Console.WriteLine($"❌ Missing materials! Need {count}x {name}, have {materialSlots[name].Count}.");
                    return false;
                }
            }

            foreach (var (name, count) in materials)
            {
                var indices = materialSlots[name].Take(count).OrderByDescending(x => x).ToList();
                foreach (var slotIndex in indices)
                {
                    crafter.Inventory.RemoveItem(slotIndex);
                }
            }

            if (crafter.Inventory.AddItem(result))
            {
                return true;
            }
            else
            {
                Console.WriteLine("⚠️ Inventory full! Torch crafted but dropped.");
                return false;
            }
        }

        public static bool CraftBackpackItem(Character crafter, (string name, int count)[] materials, Backpack result, string itemName, CraftingProfession requiredProfession)
        {
            if (requiredProfession != CraftingProfession.None &&
                !ProfessionManager.CanCraftWithProfession(crafter, requiredProfession))
            {
                Console.WriteLine($"\n⚠️ {crafter.Name} needs {requiredProfession} profession to craft this!");
                return false;
            }

            var slots = crafter.Inventory.Slots;
            var materialSlots = new Dictionary<string, List<int>>();

            foreach (var (name, count) in materials)
            {
                materialSlots[name] = new List<int>();
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i]?.Name == name)
                    {
                        materialSlots[name].Add(i);
                    }
                }

                if (materialSlots[name].Count < count)
                {
                    Console.WriteLine($"❌ Missing materials! Need {count}x {name}, have {materialSlots[name].Count}.");
                    return false;
                }
            }

            foreach (var (name, count) in materials)
            {
                var indices = materialSlots[name].Take(count).OrderByDescending(x => x).ToList();
                foreach (var slotIndex in indices)
                {
                    crafter.Inventory.RemoveItem(slotIndex);
                }
            }

            if (crafter.Inventory.AddItem(result))
            {
                return true;
            }
            else
            {
                Console.WriteLine("⚠️ Inventory full! Backpack crafted but dropped.");
                return false;
            }
        }
    }
}
