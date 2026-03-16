using System;

namespace Rpg_Dungeon
{
    internal class Torch : Equipment
    {
        #region Properties

        public int BurnTimeHours { get; private set; }
        public int MaxBurnTimeHours { get; }
        public bool IsLit { get; private set; }
        public bool IsBurnedOut => BurnTimeHours <= 0;

        #endregion

        #region Constructor

        public Torch(string name, int maxBurnTime, int price = 10)
            : base(name, EquipmentType.OffHand, maxDurability: maxBurnTime, price: price)
        {
            MaxBurnTimeHours = maxBurnTime;
            BurnTimeHours = maxBurnTime;
            IsLit = false;
        }

        #endregion

        #region Methods

        public void Light()
        {
            if (IsBurnedOut)
            {
                Console.WriteLine($"🔥 {Name} is burned out and cannot be lit!");
                return;
            }

            if (IsLit)
            {
                Console.WriteLine($"🔥 {Name} is already lit.");
                return;
            }

            IsLit = true;
            Console.WriteLine($"🔥 You light the {Name}. It illuminates the darkness!");
        }

        public void Extinguish()
        {
            if (!IsLit)
            {
                Console.WriteLine($"{Name} is not lit.");
                return;
            }

            IsLit = false;
            Console.WriteLine($"💨 You extinguish the {Name}.");
        }

        public void Burn(int hours)
        {
            if (!IsLit || IsBurnedOut) return;

            BurnTimeHours = Math.Max(0, BurnTimeHours - hours);

            // Also damage the durability
            Damage(hours);

            if (IsBurnedOut)
            {
                IsLit = false;
                Console.WriteLine($"💨 Your {Name} has burned out completely!");
            }
            else if (BurnTimeHours <= 2)
            {
                Console.WriteLine($"⚠️ Your {Name} is flickering... ({BurnTimeHours} hours remaining)");
            }
        }

        public string GetStatusDescription()
        {
            if (IsBurnedOut)
                return $"{Name} [Burned Out]";

            if (IsLit)
                return $"{Name} [🔥 Lit - {BurnTimeHours}h remaining]";

            return $"{Name} [Unlit - {BurnTimeHours}h fuel]";
        }

        #endregion
    }
}
