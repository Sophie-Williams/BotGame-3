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

        
        public int LoseItems(ItemRecord itemByName)
        {
            int output;

            ItemRecord target = items.Find(r => itemByName.item.id == r.item.id);
            output = target.quantity;
            items.Remove(target);
            return output;
        }
        public void GainItems(ItemRecord itemRecord)
        {
            GainItems(itemRecord.item, itemRecord.quantity);
        }
        public void GainItems(Item newItem, int quantityToAdd)
        {
            int leftoverItemCount = quantityToAdd;
            while (quantityToAdd > 0)
            {
                // See if the BlockSpace already has that item in its items array and if there's room to stack more
                if (items.Exists(r => (r.item != null) && (r.item.id == newItem.id) && (r.quantity < newItem.maxQuantity)))
                {
                    ItemRecord target = items.First(x => (x.item.id == newItem.id) && (x.quantity < newItem.maxQuantity));

                    //how many more we can stack
                    int maxToAdd = newItem.maxQuantity - target.quantity;

                    //add to the stack without overflowing
                    int deltaQ = Math.Min(quantityToAdd, maxToAdd);
                    target.quantity += deltaQ;
                    
                    quantityToAdd -= deltaQ;
                    leftoverItemCount -= deltaQ;
                    
                } 
                else
                {
                    items.Add(new ItemRecord(newItem, 0));
                    
                }
            }
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
            //Consolidate stacks
        }
    }
}
