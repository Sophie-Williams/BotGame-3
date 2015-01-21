using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class ItemRecord
    {
        public Item item { get; set; }
        public int quantity { get; set; }

        public ItemRecord(Item _item, int _quantity)
        {
            item = _item;
            quantity = _quantity;
        }

    }
}
