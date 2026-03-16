using Night.Characters;
using Rpg_Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Night.Shops
{
    #region Stock Entry

    internal class StockEntry<T>
    {
        public T Item { get; }
        public int Price { get; }
        public int Quantity { get; set; }

        public StockEntry(T item, int price, int quantity)
        {
            Item = item;
            Price = price;
            Quantity = quantity;
        }
    }

    #endregion

    #region Base Shop Class

    internal abstract class Shop
    {
        public abstract string ShopName { get; }
        public abstract void OpenShop(List<Character> party);

        protected void BuyFromStock(List<Character> party, List<StockEntry<GenericItem>> stock, string stockName)
        {
            Console.WriteLine($"Available {stockName}:");
            for (int i = 0; i < stock.Count; i++)
            {
                var e = stock[i];
                Console.WriteLine($"{i + 1}) {e.Item.Name} - Price {e.Price} (On hand: {e.Quantity})");
            }
            Console.WriteLine("Choose item number to buy or 0 to cancel:");
            var s = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(s, out var idx) || idx < 0 || idx > stock.Count) { Console.WriteLine("Invalid."); return; }
            if (idx == 0) return;
            var entry = stock[idx - 1];

            Console.WriteLine("Which party member will buy it?");
            for (int i = 0; i < party.Count; i++) Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) { Console.WriteLine("Invalid."); return; }
            var buyer = party[whoIdx - 1];

            if (entry.Quantity <= 0) { Console.WriteLine("Out of stock."); return; }
            if (!buyer.Inventory.SpendGold(entry.Price)) { Console.WriteLine("Not enough gold."); return; }

            entry.Quantity--;
            buyer.Inventory.AddItem(new GenericItem(entry.Item.Name, entry.Price));
            Console.WriteLine($"{buyer.Name} bought {entry.Item.Name} for {entry.Price} gold.");
        }

        protected void SellToStock(List<Character> party, List<StockEntry<GenericItem>> stock, string shopName)
        {
            Console.WriteLine("Select seller (party member):");
            for (int i = 0; i < party.Count; i++) Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) { Console.WriteLine("Invalid."); return; }
            var seller = party[whoIdx - 1];

            var slots = seller.Inventory.Slots;
            Console.WriteLine("Select slot number of item to sell:");
            for (int i = 0; i < slots.Count; i++) Console.WriteLine($"{i + 1}) {(slots[i] == null ? "(empty)" : slots[i]!.Name)}");
            var s = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(s, out var slotIdx) || slotIdx < 1 || slotIdx > slots.Count) { Console.WriteLine("Invalid."); return; }
            var item = slots[slotIdx - 1];
            if (item == null) { Console.WriteLine("No item."); return; }

            var shopEntry = stock.FirstOrDefault(o => string.Equals(o.Item.Name, item.Name, StringComparison.OrdinalIgnoreCase));
            if (shopEntry == null)
            {
                Console.WriteLine($"{shopName} is not interested in this item.");
                return;
            }

            int sellPrice = (int)Math.Ceiling(shopEntry.Price * 0.75);
            seller.Inventory.AddGold(sellPrice);
            seller.Inventory.RemoveItem(slotIdx - 1);
            shopEntry.Quantity++;
            Console.WriteLine($"{seller.Name} sold {item.Name} for {sellPrice} gold.");
        }

        protected void BuyEquipment(List<Character> party, List<StockEntry<Equipment>> equipStock)
        {
            Console.WriteLine("Available equipment:");
            for (int i = 0; i < equipStock.Count; i++)
            {
                var e = equipStock[i];
                string stats = $"STR+{e.Item.StrengthBonus} AGI+{e.Item.AgilityBonus} INT+{e.Item.IntelligenceBonus} HP+{e.Item.MaxHPBonus} Mana+{e.Item.MaxManaBonus} AR+{e.Item.ArmorBonus}";
                Console.WriteLine($"{i + 1}) {e.Item.Name} [{e.Item.Type}] - Price {e.Price} (Dur {e.Item.MaxDurability}) [{stats}] (On hand: {e.Quantity})");
            }
            Console.WriteLine("Choose equipment number to buy or 0 to cancel:");
            var s = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(s, out var idx) || idx < 0 || idx > equipStock.Count) { Console.WriteLine("Invalid."); return; }
            if (idx == 0) return;
            var entry = equipStock[idx - 1];

            Console.WriteLine("Which party member will buy it?");
            for (int i = 0; i < party.Count; i++) Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) { Console.WriteLine("Invalid."); return; }
            var buyer = party[whoIdx - 1];

            Equipment? currentEquip = entry.Item.Type switch
            {
                EquipmentType.Weapon => buyer.Inventory.EquippedWeapon,
                EquipmentType.Armor => buyer.Inventory.EquippedArmor,
                EquipmentType.Accessory => buyer.Inventory.EquippedAccessory,
                EquipmentType.Necklace => buyer.Inventory.EquippedNecklace,
                EquipmentType.Ring => buyer.Inventory.EquippedRing1,
                EquipmentType.OffHand => buyer.Inventory.EquippedOffHand,
                _ => null
            };

            if (currentEquip != null)
            {
                Console.WriteLine($"\n=== EQUIPMENT COMPARISON ===");
                Console.WriteLine($"Currently Equipped: {currentEquip.Name}");
                Console.WriteLine($"  STR: {currentEquip.StrengthBonus} | AGI: {currentEquip.AgilityBonus} | INT: {currentEquip.IntelligenceBonus} | HP: {currentEquip.MaxHPBonus} | Mana: {currentEquip.MaxManaBonus} | AR: {currentEquip.ArmorBonus}");
                Console.WriteLine($"New Item: {entry.Item.Name}");
                Console.WriteLine($"  STR: {entry.Item.StrengthBonus} | AGI: {entry.Item.AgilityBonus} | INT: {entry.Item.IntelligenceBonus} | HP: {entry.Item.MaxHPBonus} | Mana: {entry.Item.MaxManaBonus} | AR: {entry.Item.ArmorBonus}");
                Console.WriteLine($"Difference:");
                Console.WriteLine($"  STR: {entry.Item.StrengthBonus - currentEquip.StrengthBonus:+#;-#;0} | AGI: {entry.Item.AgilityBonus - currentEquip.AgilityBonus:+#;-#;0} | INT: {entry.Item.IntelligenceBonus - currentEquip.IntelligenceBonus:+#;-#;0} | HP: {entry.Item.MaxHPBonus - currentEquip.MaxHPBonus:+#;-#;0} | Mana: {entry.Item.MaxManaBonus - currentEquip.MaxManaBonus:+#;-#;0} | AR: {entry.Item.ArmorBonus - currentEquip.ArmorBonus:+#;-#;0}");
                Console.WriteLine("============================\n");
            }
            else
            {
                Console.WriteLine($"\n{buyer.Name} has no {entry.Item.Type} equipped. This will be a new addition!\n");
            }

            if (entry.Quantity <= 0) { Console.WriteLine("Out of stock."); return; }
            if (!buyer.Inventory.SpendGold(entry.Price)) { Console.WriteLine("Not enough gold."); return; }

            entry.Quantity--;
            var newEquip = new Equipment(entry.Item.Name, entry.Item.Type, entry.Item.MaxDurability, entry.Price,
                                        entry.Item.StrengthBonus, entry.Item.AgilityBonus, entry.Item.IntelligenceBonus,
                                        entry.Item.MaxHPBonus, entry.Item.MaxManaBonus, entry.Item.MaxStaminaBonus, entry.Item.ArmorBonus);
            buyer.Inventory.AddItem(newEquip);
            Console.WriteLine($"{buyer.Name} bought {newEquip.Name} for {entry.Price} gold.");
        }

        protected void RepairEquipmentByType(List<Character> party, EquipmentType? typeFilter, string typeName)
        {
            Console.WriteLine($"\n=== {typeName.ToUpper()} REPAIR SERVICE ===");
            Console.WriteLine($"Select who needs {typeName} repair:");
            for (int i = 0; i < party.Count; i++) Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) { Console.WriteLine("Invalid."); return; }
            var c = party[whoIdx - 1];

            var repairableItems = new List<(string location, Equipment eq)>();

            if (typeFilter == null || c.Inventory.EquippedWeapon != null && (typeFilter == null || c.Inventory.EquippedWeapon.Type == typeFilter))
            {
                if (c.Inventory.EquippedWeapon != null)
                {
                    var eq = c.Inventory.EquippedWeapon;
                    repairableItems.Add(("Equipped Weapon", eq));
                }
            }

            if (typeFilter == null || c.Inventory.EquippedArmor != null && (typeFilter == null || c.Inventory.EquippedArmor.Type == typeFilter))
            {
                if (c.Inventory.EquippedArmor != null)
                {
                    var eq = c.Inventory.EquippedArmor;
                    repairableItems.Add(("Equipped Armor", eq));
                }
            }

            if (typeFilter == null || c.Inventory.EquippedAccessory != null && (typeFilter == null || c.Inventory.EquippedAccessory.Type == typeFilter))
            {
                if (c.Inventory.EquippedAccessory != null)
                {
                    var eq = c.Inventory.EquippedAccessory;
                    repairableItems.Add(("Equipped Accessory", eq));
                }
            }

            if (typeFilter == null || c.Inventory.EquippedNecklace != null && (typeFilter == null || c.Inventory.EquippedNecklace.Type == typeFilter))
            {
                if (c.Inventory.EquippedNecklace != null)
                {
                    var eq = c.Inventory.EquippedNecklace;
                    repairableItems.Add(("Equipped Necklace", eq));
                }
            }

            if (typeFilter == null || c.Inventory.EquippedRing1 != null && (typeFilter == null || c.Inventory.EquippedRing1.Type == typeFilter))
            {
                if (c.Inventory.EquippedRing1 != null)
                {
                    var eq = c.Inventory.EquippedRing1;
                    repairableItems.Add(("Equipped Ring 1", eq));
                }
            }

            if (typeFilter == null || c.Inventory.EquippedRing2 != null && (typeFilter == null || c.Inventory.EquippedRing2.Type == typeFilter))
            {
                if (c.Inventory.EquippedRing2 != null)
                {
                    var eq = c.Inventory.EquippedRing2;
                    repairableItems.Add(("Equipped Ring 2", eq));
                }
            }

            if (typeFilter == null || c.Inventory.EquippedOffHand != null && (typeFilter == null || c.Inventory.EquippedOffHand.Type == typeFilter))
            {
                if (c.Inventory.EquippedOffHand != null)
                {
                    var eq = c.Inventory.EquippedOffHand;
                    repairableItems.Add(("Equipped Off-Hand", eq));
                }
            }

            var slots = c.Inventory.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] is Equipment eq && (typeFilter == null || eq.Type == typeFilter))
                {
                    repairableItems.Add(($"Inventory Slot {i + 1}", eq));
                }
            }

            if (repairableItems.Count == 0)
            {
                Console.WriteLine($"No {typeName} equipment to repair.");
                return;
            }

            for (int i = 0; i < repairableItems.Count; i++)
            {
                var item = repairableItems[i];
                string durStatus = item.eq.Durability <= item.eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{i + 1}) [{item.location}] {item.eq.Name} (Dur: {item.eq.Durability}/{item.eq.MaxDurability}){durStatus} - Repair: {item.eq.RepairCost()}g");
            }

            Console.Write("\nChoose equipment number to repair (or 0 to cancel): ");
            var sel = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(sel, out var selIdx) || selIdx < 1 || selIdx > repairableItems.Count) { Console.WriteLine("Cancelled."); return; }

            var chosen = repairableItems[selIdx - 1];
            int cost = chosen.eq.RepairCost();

            if (cost <= 0)
            {
                Console.WriteLine("Item is already at full durability!");
                return;
            }

            if (!c.Inventory.SpendGold(cost))
            {
                Console.WriteLine($"Not enough gold. Repair costs {cost}g but you have {c.Inventory.Gold}g.");
                return;
            }

            chosen.eq.Repair();
            Console.WriteLine($"🔨 {chosen.eq.Name} repaired to full durability! Gold remaining: {c.Inventory.Gold}");
        }

        protected void RepairAllEquipment(List<Character> party)
        {
            Console.WriteLine("\n=== EQUIPMENT REPAIR SERVICE ===");
            Console.WriteLine("Select who needs a repair:");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            }
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) { Console.WriteLine("Invalid."); return; }
            var c = party[whoIdx - 1];

            Console.WriteLine($"\n{c.Name}'s Equipment:");
            var repairableItems = new List<(string location, Equipment eq)>();
            int itemNum = 1;

            if (c.Inventory.EquippedWeapon != null)
            {
                var eq = c.Inventory.EquippedWeapon;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Weapon: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("Weapon", eq));
                itemNum++;
            }

            if (c.Inventory.EquippedArmor != null)
            {
                var eq = c.Inventory.EquippedArmor;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Armor: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("Armor", eq));
                itemNum++;
            }

            if (c.Inventory.EquippedAccessory != null)
            {
                var eq = c.Inventory.EquippedAccessory;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Accessory: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("Accessory", eq));
                itemNum++;
            }

            if (c.Inventory.EquippedNecklace != null)
            {
                var eq = c.Inventory.EquippedNecklace;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Necklace: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("Necklace", eq));
                itemNum++;
            }

            if (c.Inventory.EquippedRing1 != null)
            {
                var eq = c.Inventory.EquippedRing1;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Ring 1: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("Ring1", eq));
                itemNum++;
            }

            if (c.Inventory.EquippedRing2 != null)
            {
                var eq = c.Inventory.EquippedRing2;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Ring 2: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("Ring2", eq));
                itemNum++;
            }

            if (c.Inventory.EquippedOffHand != null)
            {
                var eq = c.Inventory.EquippedOffHand;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Off-Hand: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("OffHand", eq));
                itemNum++;
            }

            var slots = c.Inventory.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] is Equipment eq)
                {
                    string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                    Console.WriteLine($"{itemNum}) [Inventory Slot {i + 1}]: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                    repairableItems.Add(($"Slot{i}", eq));
                    itemNum++;
                }
            }

            if (repairableItems.Count == 0)
            {
                Console.WriteLine("No equipment to repair.");
                return;
            }

            Console.WriteLine("\nOptions:");
            Console.WriteLine("R) Repair All (pay full cost for all items)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose equipment number to repair or R for all: ");
            var sel = Console.ReadLine() ?? string.Empty;

            if (sel.Trim().Equals("R", StringComparison.OrdinalIgnoreCase))
            {
                int totalCost = repairableItems.Sum(item => item.eq.RepairCost());
                if (totalCost <= 0)
                {
                    Console.WriteLine("All equipment is already at full durability!");
                    return;
                }

                Console.WriteLine($"\nTotal repair cost for all equipment: {totalCost} gold");
                Console.Write("Proceed with repair all? (y/n): ");
                var confirm = Console.ReadLine() ?? string.Empty;
                if (!confirm.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Repair cancelled.");
                    return;
                }

                if (!c.Inventory.SpendGold(totalCost))
                {
                    Console.WriteLine($"Not enough gold. You have {c.Inventory.Gold}g but need {totalCost}g.");
                    return;
                }

                foreach (var item in repairableItems)
                {
                    if (item.eq.RepairCost() > 0)
                    {
                        item.eq.Repair();
                        Console.WriteLine($"✓ {item.eq.Name} repaired to full durability.");
                    }
                }

                Console.WriteLine($"\n🔨 All equipment repaired! Gold remaining: {c.Inventory.Gold}");
            }
            else if (int.TryParse(sel, out var selIdx) && selIdx >= 1 && selIdx <= repairableItems.Count)
            {
                var chosen = repairableItems[selIdx - 1];
                int cost = chosen.eq.RepairCost();
                if (cost <= 0) { Console.WriteLine("Item does not need repair."); return; }
                if (!c.Inventory.SpendGold(cost))
                {
                    Console.WriteLine($"Not enough gold. Repair costs {cost}g but you have {c.Inventory.Gold}g.");
                    return;
                }
                chosen.eq.Repair();
                Console.WriteLine($"🔨 {chosen.eq.Name} repaired to full durability! Gold remaining: {c.Inventory.Gold}");
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }
    }

    #endregion

    #region Shop Manager

    internal class Shops
    {
        private readonly BlacksmithShop _blacksmith;
        private readonly LeatherWorkerShop _leatherWorker;
        private readonly TailorShop _tailor;
        private readonly MageShop _mageShop;
        private readonly ApothecaryShop _apothecary;
        private readonly JewelerShop _jeweler;
        private readonly MerchantShop _merchant;

        public Shops()
        {
            _blacksmith = new BlacksmithShop();
            _leatherWorker = new LeatherWorkerShop();
            _tailor = new TailorShop();
            _mageShop = new MageShop();
            _apothecary = new ApothecaryShop();
            _jeweler = new JewelerShop();
            _merchant = new MerchantShop();
        }

        public void OpenBlacksmith(List<Character> party) => _blacksmith.OpenShop(party);
        public void OpenLeatherWorker(List<Character> party) => _leatherWorker.OpenShop(party);
        public void OpenTailor(List<Character> party) => _tailor.OpenShop(party);
        public void OpenMageShop(List<Character> party) => _mageShop.OpenShop(party);
        public void OpenApothecary(List<Character> party) => _apothecary.OpenShop(party);
        public void OpenJeweler(List<Character> party) => _jeweler.OpenShop(party);
        public void OpenMerchant(List<Character> party) => _merchant.OpenShop(party);
    }

    #endregion
}
