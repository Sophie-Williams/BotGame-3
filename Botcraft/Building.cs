using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class Building
    {
        //Main base, repair stations, any other fixed building.
        //Moving buildings should probably just be mobs.
        public List<ItemRecord> storage;
        public char dispChar;
        public ConsoleColor dispColor;

        public Building() : this('@', ConsoleColor.Yellow) { }
        public Building(char avatar, ConsoleColor _color)
        {
            storage = new List<ItemRecord>();
            dispChar = avatar;
            dispColor = _color;

        }

    }
}
