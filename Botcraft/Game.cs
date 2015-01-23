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
            Console.WriteLine("\n---\nGame.tick()\n---\n");
            foreach (Mob mob in moblist)
            {
                mob.Tick();
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

            Queue<MobAction> testQueue = new Queue<MobAction>();
            testQueue.Enqueue(new MobAction(MobActEnum.Pause));
            testQueue.Enqueue(new MobAction(MobActEnum.Move, "W"));
            testQueue.Enqueue(new MobAction(MobActEnum.Move, "S"));
            testQueue.Enqueue(new MobAction(MobActEnum.Scan));
            testQueue.Enqueue(new MobAction(MobActEnum.Move, "E"));
            testQueue.Enqueue(new MobAction(MobActEnum.Move, "N"));
            testQueue.Enqueue(new MobAction(MobActEnum.Pause));

            moblist.Add(new Mob(0, 2, 2, '$', ConsoleColor.Green, "Rob", testQueue));
           
        }

        /*
         * Need to rebuild the test queues using List<MobAction>
         * Figure out how to build it from an array or make a convenient constructor
         */

        private void itemTest()
        {
            floors[0].map[0, 2].items.Add(new ItemRecord(new Item(ItemID.Item1), 8));
            floors[0].map[0, 2].items.Add(new ItemRecord(new Item(ItemID.Item2), 2));
            floors[0].map[0, 2].items.Add(new ItemRecord(new Item(ItemID.Item1), 3));

            Queue<MobAction> itemTestQueue = new Queue<MobAction>();
            itemTestQueue.Enqueue(new MobAction(MobActEnum.Mine, "E", 9));
            itemTestQueue.Enqueue(new MobAction(MobActEnum.Pause));
            itemTestQueue.Enqueue(new MobAction(MobActEnum.Move, "E", 2));
            itemTestQueue.Enqueue(new MobAction(MobActEnum.Loot));
            itemTestQueue.Enqueue(new MobAction(MobActEnum.Pause));
            itemTestQueue.Enqueue(new MobAction(MobActEnum.Move, "S"));
            itemTestQueue.Enqueue(new MobAction(MobActEnum.Move, "W"));
            itemTestQueue.Enqueue(new MobAction(MobActEnum.Mine, "W", 6));

            moblist.Add(new Mob(0, 0, 0, '%', ConsoleColor.Red, "PickUp", itemTestQueue));
        }
    }
}
