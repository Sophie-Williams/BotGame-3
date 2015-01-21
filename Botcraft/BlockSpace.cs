using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class BlockSpace
    {
        public Block block;
        public Mob mob;
        public List<ItemRecord> items = new List<ItemRecord>();
        public Building building;

        public static implicit operator Block(BlockSpace bs)
        { return bs.block; }
        public static implicit operator Building(BlockSpace bs)
        { return bs.building; }
        public static implicit operator Mob(BlockSpace bs)
        { return bs.mob; }

        public BlockSpace()
        {
            block = null;
            mob = null;
            building = null;
        }

        
        public int takeItemByItemName(ItemRecord itemByName)
        {
            int output;

            ItemRecord target = items.Find(r => itemByName.item.id == r.item.id);
            output = target.quantity;
            items.Remove(target);
            return output;
        }
        public void giveItemRecord(ItemRecord given)
        {
            items.Add(given);
            cleanupItems();
        }
        public bool mine(int minePower)
        {
            bool broken = false;
            bool outBool = block.mine(minePower, ref broken);
            if (broken)
            {
                this.block = BlockFactory.Get(BlockID.Air);
            }
            return outBool;

        }
        public ConsoleColor getDispColor()
        {
            if (mob != null)
            {
                return mob.dispColor;
            }
            else
                return block.dispColor;
        }
        public char getDispChar()
        {
            if (mob != null)
            {
                return mob.dispChar;
            }
            else
                return block.dispChar;
        }
        public bool enter(Mob newMob)
        {
            if (mob == null && block.id == BlockID.Air)
            {
                mob = newMob;
                return true;
            }
            else return false;
        }
        public bool isMovable()
        {
            if (block.id == BlockID.Air && mob == null)
                return true;
            else
                return false;
        }
        public void leave()
        {
            mob = null;
        }


        public void cleanupItems()
        {
            items.RemoveAll(r => (r.quantity == 0));
            //TODO: stack items
        }
    }
}
