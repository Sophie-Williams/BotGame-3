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
        

        public void Building()
        {
            storage = new List<ItemRecord>();

        }

    }
}
