using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class ItemRecord
    {
        public Item item { get; private set; }
        public int quantity { get; private set; }

        public ItemRecord(Item _item, int _quantity)
        {
            item = _item;
            quantity = _quantity;
        }
        public void alterQuantity(int deltaQ)
        {
            quantity += deltaQ;
        }
    }
    
    class Item
    {
        public String name;
        public int maxQuantity;

        public Item(String newName)
        {
            name = newName;
            maxQuantity = 10;
        }

        
    }

    //Stats { HP, Armor, Speed, ScanRadius, AttackPower, MinePower, MaxCapacity }
    class Equipment : Item
    {
        //Public Data
        public int[] statBonus = new int[Enum.GetNames(typeof(Stats)).Length];
        public bool[] fitsSlot = new bool[Enum.GetNames(typeof(EquipLoc)).Length];
        //Constructors
        public Equipment(String newName, bool[] slots, int[] stats)
            : base(newName)
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
