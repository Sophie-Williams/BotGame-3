using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    //Stats { HP, Armor, Speed, ScanRadius, AttackPower, MinePower, MaxCapacity }
    class Equipment : Item
    {
        //Public Data
        public int[] statBonus = new int[Enum.GetNames(typeof(Stats)).Length];
        public bool[] fitsSlot = new bool[Enum.GetNames(typeof(EquipLoc)).Length];
        //Constructors
        public Equipment(ItemID id, bool[] slots, int[] stats)
            : base(id)
        {
            maxQuantity = 1;
            for (int i = 0; i < Enum.GetNames(typeof(EquipLoc)).Length; i++)
            {
                fitsSlot[i] = slots[i];
            }
        }

        //Public Methods
        public bool fits(EquipLoc slot)
        {
            return fitsSlot[(int)slot];
        }

    }
}
