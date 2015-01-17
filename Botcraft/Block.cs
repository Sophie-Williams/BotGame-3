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
                    return new Block('S', BlockID.Stone);
                case BlockID.Dirt:
                    return new Block('D', BlockID.Dirt);
                case BlockID.Wall:
                    return new Block('#', BlockID.Wall);
                case BlockID.Air:
                default:
                    return new Block('_', BlockID.Air);
            }
        }
    }
    class Block
    {
        //Public Members
        public BlockID id;
        public int dataVal;
        public char dispChar { get; set; }
        public List<Item> items;

        //Constructors
        public Block(char texture, BlockID name) : this(texture, name, 0) { }
        public Block(char texture, BlockID name, int dVal)
        {
            dispChar = texture;
            id = name;
            dataVal = dVal;
        }
        
        //Public Methods
       

    }
}
