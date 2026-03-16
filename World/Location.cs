using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    #region Location Category Enum

    internal enum LocationCategory
    {
        Town,           // Major hub with all services
        Settlement,     // Medium location with limited services (always has inn)
        Camp,          // Small location with basic needs only
        Dungeon,       // Combat area
        QuestSpot      // Quest location
    }

    #endregion

    #region Base Location Class

    internal abstract class Location
    {
        #region Properties

        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public LocationCategory Type { get; protected set; }
        public int RequiredLevel { get; protected set; }
        public bool IsDiscovered { get; set; }
        public Dictionary<string, int> DistancesToOtherLocations { get; }

        #endregion

        #region Constructor

        protected Location(string name, string description, LocationCategory type, int requiredLevel = 1)
        {
            Name = name;
            Description = description;
            Type = type;
            RequiredLevel = requiredLevel;
            IsDiscovered = false;
            DistancesToOtherLocations = new Dictionary<string, int>();
        }

        #endregion

        #region Methods

        public virtual void DisplayInfo()
        {
            string icon = GetLocationIcon();
            Console.WriteLine($"\n{icon} {Name}");
            Console.WriteLine($"   Type: {Type}");
            Console.WriteLine($"   {Description}");
            if (RequiredLevel > 1)
            {
                Console.WriteLine($"   Required Level: {RequiredLevel}");
            }
        }

        protected string GetLocationIcon()
        {
            return Type switch
            {
                LocationCategory.Town => "🏰",
                LocationCategory.Settlement => "🏘️",
                LocationCategory.Camp => "⛺",
                LocationCategory.Dungeon => "🗿",
                LocationCategory.QuestSpot => "📍",
                _ => "📍"
            };
        }

        public abstract void Enter(List<Character> party, GameState gameState);

        #endregion
    }

    #endregion

    #region GameState Helper

    internal class GameState
    {
        public QuestBoard? QuestBoard { get; set; }
        public BountyBoard? BountyBoard { get; set; }
        public AchievementTracker? AchievementTracker { get; set; }
        public Journal? Journal { get; set; }
        public Weather? Weather { get; set; }
        public TimeOfDay? TimeTracker { get; set; }
        public NPCManager? NPCManager { get; set; }
        public MainStoryline? MainStoryline { get; set; }
        public FogOfWarMap? FogOfWarMap { get; set; }
        public int? WorldSeed { get; set; }
    }

    #endregion
}
