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
            basicTest();
            itemTest();
        }

        //Public Methods
        public void tick()
        {
            foreach (Mob mob in moblist)
            {
                Console.WriteLine("\n---\nGame.tick()\n---\n");
                mob.tick();
            }
        }
        
        //Private Methods
        private void init()
        {

            floors[0] = new Level(0, new BlockID[] { BlockID.Air, BlockID.Dirt });
            floors[0].showMap(0);
            for (int i = 1; i < MAP_DEPTH; i++)
    		    {
                    floors[i] = new Level(i, new BlockID[] { BlockID.Stone, BlockID.Dirt });
                    floors[i].showMap(i);
			    }
            
            
        }

        private void basicTest()
        {
            Queue<MobCmd> testQueue = new Queue<MobCmd>(new MobCmd[] { MobCmd.Pause, MobCmd.MoveWest, MobCmd.MoveSouth, MobCmd.Scan, MobCmd.MoveEast, MobCmd.MoveNorth, MobCmd.Pause });
            moblist.Add(new Mob(0, 2, 2, '$', ConsoleColor.Green, "Rob", testQueue));

        }

        private void itemTest()
        {
            floors[0].map[0, 2].items.Add(new ItemRecord(new Item(ItemID.Item1), 8));
            floors[0].map[0, 2].items.Add(new ItemRecord(new Item(ItemID.Item2), 2));
            floors[0].map[0, 2].items.Add(new ItemRecord(new Item(ItemID.Item1), 3));
            Queue<MobCmd> itemTestQueue = new Queue<MobCmd>(new MobCmd[] { MobCmd.MoveWest, MobCmd.MoveWest, MobCmd.MoveEast, MobCmd.Pause });
            moblist.Add(new Mob(0, 0, 4, '%', ConsoleColor.Red, "PickUp", itemTestQueue));
        }
    }
}
