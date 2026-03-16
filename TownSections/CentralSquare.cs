using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class CentralSquare
    {
        #region Fields

        private Weather? _weather;
        private TimeOfDay? _timeTracker;

        #endregion

        #region Setup Methods

        public void SetWeather(Weather? weather)
        {
            _weather = weather;
        }

        public void SetTimeTracker(TimeOfDay? timeTracker)
        {
            _timeTracker = timeTracker;
        }

        #endregion

        #region Public Methods

        // Allow passing a Journal so dialogue actions can update quests
        public void ShowCentralSquare(List<Character> party, Journal? journal = null)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         Town Square - GreyWolf        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");

            // Show current time in town
            if (_timeTracker != null)
            {
                Console.WriteLine($"Current time: {_timeTracker.GetTimeDescription()}");
            }

            // Show current weather in town
            if (_weather != null)
            {
                Console.WriteLine($"The weather today: {_weather.GetWeatherDescription()}");
            }

            Console.WriteLine("\nA bustling fountain sits at the center of town.");
            Console.WriteLine("Paths lead to different districts in all directions.\n");

            ShowParty(party);

            // Simple interaction option to speak with Old Tom (example)
            Console.WriteLine("\nOptions: \n1) Speak to Old Tom (Fountain Keeper)\n0) Continue");
            Console.Write("Your choice: ");
            var input = Console.ReadLine()?.Trim() ?? "";
            if (input == "1")
            {
                Console.WriteLine("\nYou approach Old Tom at the fountain...");
                // Use shared NPC manager if available
                var npcManager = Rpg_Dungeon.Systems.GameServices.NPCManager ?? new NPCManager();
                DialogueManager.StartExampleDialogue(journal, npcManager);
            }
        }

        #endregion

        #region Private Methods

        private void ShowParty(List<Character> party)
        {
            Console.WriteLine("\n--- Your Party ---");
            foreach (var member in party)
            {
                if (member == null) continue;
                Console.WriteLine($"- {member.Name} [{member.GetType().Name}] Lv {member.Level}");
                Console.WriteLine($"  HP: {member.Health}/{member.MaxHealth} | Mana: {member.Mana}/{member.MaxMana}");
                Console.WriteLine($"  Gold: {member.Inventory.Gold} | {(member.IsAlive ? "Alive" : "Down")}");
            }
        }

        #endregion
    }
}
