using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class PetShop
    {
        #region Fields

        private readonly List<(PetType type, PetAbility ability, int price, string description)> _availablePets;

        #endregion

        #region Constructor

        public PetShop()
        {
            _availablePets = new List<(PetType, PetAbility, int, string)>
            {
                (PetType.Wolf, PetAbility.DamageBoost, 150, "A fierce companion that boosts your attack power."),
                (PetType.Cat, PetAbility.LootBonus, 200, "A lucky cat that finds extra gold."),
                (PetType.Raven, PetAbility.ExperienceBoost, 180, "A wise raven that helps you learn faster."),
                (PetType.Spirit, PetAbility.ManaRegen, 220, "A mystical spirit that restores mana."),
                (PetType.Dragon, PetAbility.DefenseBoost, 300, "A baby dragon that provides protection."),
                (PetType.Familiar, PetAbility.HealthRegen, 170, "A magical familiar that aids in healing.")
            };
        }

        #endregion

        #region Shop Interface

        public void OpenPetShop(Character character)
        {
            if (character == null) return;

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════╗");
                Console.WriteLine("║       Pet Shop - Companions           ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine($"Your Gold: {character.Inventory.Gold}");

                if (character.Pet != null)
                {
                    Console.WriteLine($"\nYour companion: {character.Pet}");
                    Console.WriteLine("\n1) Rename Pet");
                    Console.WriteLine("2) Feed Pet (Costs 10g, increases loyalty)");
                    Console.WriteLine("3) Release Pet");
                    Console.WriteLine("0) Leave");
                }
                else
                {
                    Console.WriteLine("\nYou don't have a companion yet!");
                    Console.WriteLine("\nAvailable Companions:");
                    for (int i = 0; i < _availablePets.Count; i++)
                    {
                        var pet = _availablePets[i];
                        Console.WriteLine($"{i + 1}) {pet.type} - {pet.price}g");
                        Console.WriteLine($"   {pet.description}");
                        Console.WriteLine($"   Ability: {GetAbilityDesc(pet.ability)}");
                    }
                    Console.WriteLine("\n0) Leave");
                }

                Console.Write("Choice: ");
                var choice = Console.ReadLine() ?? string.Empty;

                if (choice.Trim() == "0") return;

                if (character.Pet != null)
                {
                    HandlePetManagement(character, choice);
                }
                else
                {
                    BuyPet(character, choice);
                }
            }
        }

        #endregion

        #region Helper Methods

        private void HandlePetManagement(Character character, string choice)
        {
            switch (choice.Trim())
            {
                case "1":
                    Console.Write("Enter new name for your pet: ");
                    var newName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        character.Pet!.Name = newName;
                        Console.WriteLine($"Pet renamed to {newName}!");
                    }
                    break;
                case "2":
                    if (character.Inventory.SpendGold(10))
                    {
                        character.Pet!.Feed();
                    }
                    else
                    {
                        Console.WriteLine("Not enough gold!");
                    }
                    break;
                case "3":
                    Console.Write("Are you sure you want to release your pet? (y/n): ");
                    var confirm = Console.ReadLine();
                    if (confirm?.ToLower() == "y")
                    {
                        Console.WriteLine($"{character.Pet!.Name} sadly leaves your side...");
                        character.Pet = null;
                    }
                    break;
            }
        }

        private void BuyPet(Character character, string choice)
        {
            if (!int.TryParse(choice, out var idx) || idx < 1 || idx > _availablePets.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            var selectedPet = _availablePets[idx - 1];
            if (!character.Inventory.SpendGold(selectedPet.price))
            {
                Console.WriteLine("Not enough gold!");
                return;
            }

            Console.Write($"What would you like to name your {selectedPet.type}? ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = selectedPet.type.ToString();

            character.Pet = new Pet(name, selectedPet.type, selectedPet.ability);
            Console.WriteLine($"\n🐾 You've adopted {name} the {selectedPet.type}!");
            Console.WriteLine($"Ability: {character.Pet.GetAbilityDescription()}");
        }

        private string GetAbilityDesc(PetAbility ability)
        {
            return ability switch
            {
                PetAbility.HealthRegen => "Regenerates 5% HP after combat",
                PetAbility.LootBonus => "+20% gold from enemies",
                PetAbility.ExperienceBoost => "+10% XP gain",
                PetAbility.DamageBoost => "+5% damage in combat",
                PetAbility.DefenseBoost => "+5% defense in combat",
                PetAbility.ManaRegen => "Regenerates 5% Mana after combat",
                _ => "Unknown"
            };
        }

        #endregion
    }
}
