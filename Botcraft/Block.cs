using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{

    static class BlockFactory
    {
        public static Block Get(BlockID blockName)
        {

            switch (blockName)
            {
                case BlockID.Stone:
                    return new Block(BlockID.Stone, 'S', ConsoleColor.Gray);
                case BlockID.Dirt:
                    return new Block(BlockID.Dirt, 'D', ConsoleColor.DarkYellow);
                case BlockID.Wall:
                    return new Block(BlockID.Wall, '#', ConsoleColor.DarkGray);
                case BlockID.Air:
                default:
                    return new Block(BlockID.Air, '_', ConsoleColor.DarkGray);
            }
        }
    }

    class Block
    {
        //Public Members
        public BlockID id;
        public int health;
        public int mineLevel; // Minimum minePower for mob to break brick
        public int dataVal;
        public char dispChar { get; set; }
        public ConsoleColor dispColor;
        
        //Constructors
        public Block(BlockID name, char texture, ConsoleColor color) : this(name, texture, color, 0, 0) { }
        public Block(BlockID name, char texture, ConsoleColor color, int dVal, int mineLvl)
        {
            dispChar = texture;
            dispColor = color;
            id = name;
            dataVal = dVal;
            health = 30;
            mineLevel = mineLvl;

        }
        
        //Public Methods
        public bool mine(int minePower, ref bool broken)
        {
            if (id == BlockID.Air)
                return false;   //nothing to mine

            if (minePower >= mineLevel)     //mine power is damage per hit _and_ minimum power to mine (for now)
            {
                health -= 5 * (minePower + 1);
                if (health == 0)
                {
                    broken = true;
                }
                return true;
            }
            else return false;
        }

        //Private Methods

    }
}
