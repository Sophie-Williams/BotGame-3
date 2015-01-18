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
        public int dataVal;
        public char dispChar { get; set; }
        public ConsoleColor dispColor;
        public List<Item> items;

        //Constructors
        public Block(BlockID name, char texture, ConsoleColor color) : this(name, texture, color, 0) { }
        public Block(BlockID name, char texture, ConsoleColor color, int dVal)
        {
            dispChar = texture;
            dispColor = color;
            id = name;
            dataVal = dVal;
        }
        
        //Public Methods
       

    }
}
