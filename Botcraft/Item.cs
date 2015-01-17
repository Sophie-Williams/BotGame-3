using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class Item
    {
        public String name;
        public int stackSize;

        public Item(String newName, int size)
        {
            name = newName;
            stackSize = size;
        }

    }
	    
    //Stats: Speed, Mining power, attack power, defense/armor, scan radius
    class Equipment : Item
    {
        //Public Data
        public int[] statBonus = new int[Enum.GetNames(typeof(Stats)).Length];
        public bool[] fitsSlot = new bool[Enum.GetNames(typeof(EquipLoc)).Length];
        //Constructors
        public Equipment(String newName, bool[] slots, int[] stats)
            : base(newName, 1)
        {
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
