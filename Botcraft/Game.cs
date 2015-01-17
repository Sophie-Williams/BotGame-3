using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class Game
    {
        //Public Members
        public const int MAP_HEIGHT = 20;
        public const int MAP_WIDTH = 20;
        public const int MAP_DEPTH = 5;
        public List<Mob> moblist = new List<Mob>();
        public static Level[] floors = new Level[MAP_DEPTH];
                        
        //Constructor
        public Game()
        {
            init();
        }

        //Public Methods
        public void tick()
        {
            foreach (Mob mob in moblist)
            {
                Console.WriteLine("\nGame.tick()\n");
                mob.tick();
                floors[mob.z].showMap();
            }
        }
        
        //Private Methods
        private void init()
        {

            floors[0] = new Level(0, new BlockID[] { BlockID.Air, BlockID.Dirt });
            floors[0].showMap();
            Console.WriteLine("Floor: 0");
            for (int i = 1; i < MAP_DEPTH; i++)
    		    {
                    floors[i] = new Level(i, new BlockID[] { BlockID.Stone, BlockID.Dirt });
                    floors[i].showMap();
                    Console.WriteLine("Floor: " + i);
			    }
            
            moblist.Add(new Mob(floors, 0, 10, 10, '$',"Rob"));
            
        }


    }
}
