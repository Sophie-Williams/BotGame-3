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
        public int speed { get; private set; }
        public int scanRadius { get; private set; }  
        public int minePower { get; private set; }   
        public int attackPower { get; private set; } 
        public int maxCapacity { get;  private set; }
        


        //Constructors
        public Equipment(String newName, bool battery, bool body, bool tool, bool trinket)
            : base(newName, 1)
        {
            fitsBattery = battery;
            fitsBody = body;
            fitsTool = tool;
            fitsTrinket = trinket;
        }
        //Private Data
        private bool fitsBody;
        private bool fitsTool;
        private bool fitsBattery;
        private bool fitsTrinket;

        //Public Methods
        public bool fits(EquipLoc slot)
        {
            switch (slot)
            {
                case EquipLoc.Body:
                    return fitsBody;
                case EquipLoc.Tool:
                    return fitsTool;
                case EquipLoc.Battery:
                    return fitsBattery;
                case EquipLoc.Trinket1:
                case EquipLoc.Trinket2:
                    return fitsTrinket;
                default:
                    return false;

            }
        }    

    }
}
