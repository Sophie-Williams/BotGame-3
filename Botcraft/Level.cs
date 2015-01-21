using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    
    class Level
    {
        //Static Random number generator, so we keep using the same one instead of re-seeding them
        public static Random rng = new Random();

        //Public Members
        /* map[x,y]
        * 
        * |-------> y+
        * |
        * |
        * v x+
        */
        public BlockSpace[,] map = new BlockSpace[Game.MAP_WIDTH, Game.MAP_HEIGHT];
        
        //Constructors
        public Level(int depth) : this(depth, new BlockID[] { BlockID.Dirt, BlockID.Stone }) { }
        public Level(int depth, BlockID[] fillers)
        {
            rndFill(fillers);
            if(depth == 0)
            {
                for (int i = 0; i < Game.MAP_WIDTH; i++)
                {
                    for (int j = 0; j < Game.MAP_HEIGHT; j++)
                    {
                        map[i, j].block = BlockFactory.Get(BlockID.Air);
                    }
                }
                map[0, 1].block = BlockFactory.Get(BlockID.Stone);
                map[1, 0].block = BlockFactory.Get(BlockID.Dirt);
            }
        }

        //Public Methods
        public void showMap(int floorNumToDisplay)
        {
            /*  |-------> y+
             *  |
             *  |
             *  v x+
             */
            Console.WriteLine("Floor {0}:",floorNumToDisplay);
            for (int i = 0; i < Game.MAP_HEIGHT; i++)
            {
                for (int j = 0; j < Game.MAP_WIDTH; j++)
                {
                    Console.ForegroundColor = map[i, j].getDispColor();
                    Console.Write(map[i, j].getDispChar());
                }
                Console.Write("\n");
            }
            Console.ResetColor();
            Console.Write("\n");
        }
        //Private Methods
        private void rndFill(BlockID[] fillers)
        {
            for (int i = 0; i < Game.MAP_WIDTH; i++)
            {
                for (int j = 0; j < Game.MAP_HEIGHT; j++)
                {
                    BlockID fillerSample = fillers[rng.Next(0, fillers.Length)];
                    if(map[i,j] == null)
                        map[i,j] = new BlockSpace();
                    map[i, j].block = BlockFactory.Get(fillerSample);
                }
            }
            
        }
    }
}
