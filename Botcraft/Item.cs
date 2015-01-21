using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class Item
    {
        public int maxQuantity;
        public ItemID id;

        public Item(ItemID _id)
        {
            maxQuantity = 10;
            id = _id;
        }

        
    }
}
