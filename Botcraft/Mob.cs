using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class Mob
    {
        //Constructors & Related Methods
        public Mob(int startZ, int startX, int startY, char avatar, ConsoleColor color, String newName, Queue<MobCmd> initQueue)
        {
            lastCmd = MobCmd.Idle;
            name = newName;
            theWorld = Game.floors;
            x = startX;
            y = startY;
            z = startZ;
            dispChar = avatar;
            dispColor = color;
            setStats(new [] 
            { 
                30, //HP
                0,  //Armor
                3,  //Speed
                3,  //ScanRadius
                0,  //AttackPower
                0,  //MinePower
                30  //MaxCapacity
            });
            cmdStack = new Stack<MobCmd>();
            cmdQueue = new Queue<MobCmd>();
            addCmds(initQueue);
            

            theWorld[z].map[x, y].enter(this);
        }
        private void setStats(int[] statBlock)
        {
            for (int i = 0; i < Enum.GetNames(typeof (Stats)).Length; i++)
            {
                baseStats[i] = statBlock[i];
            }
        }

        public int x { get; private set; }
        public int y { get; private set; }
        public int z { get; private set; }
        public String name { get; private set; }
        public char dispChar { get; private set; }
        public ConsoleColor dispColor { get; set; }
        public char[,,] scanData { get; private set; }
        public MobCmd lastCmd {get; private set;}

        //Private Data
        private int[] baseStats = new int[Stats.GetNames(typeof(Stats)).Length];
        private Level[] theWorld;
        private Equipment[] equipArray = new Equipment[Enum.GetNames(typeof(EquipLoc)).Length];
        
        private List<ItemRecord> inventory = new List<ItemRecord>();
        private Stack<MobCmd> cmdStack;
        private Queue<MobCmd> cmdQueue;

        //Public Methods
        public int getStat(Stats stat)
        {
            int output = baseStats[(int)stat];
            foreach (Equipment slot in equipArray)
            {
                if (slot != null)
                {
                    output += slot.statBonus[(int)stat]; 
                }
            }
            return output;
        }
        public void tick()
        {
            MobCmd cmdToExec = MobCmd.Empty;
            Console.WriteLine("Mob.tick() from {0}", name);
            
            //cmdQueue holds AI-given instructions.  cmdStack holds rules-enforced
            //(mining might take multiple actions, idle delay after moving or other actions)

            if(cmdStack != null && cmdStack.Count > 0)
            {
                cmdToExec = cmdStack.Pop();
            }
            else if (cmdQueue != null && cmdQueue.Count > 0)
            {
                cmdToExec = cmdQueue.Dequeue();
            }
            
            lastCmd = cmdToExec;

            if (cmdToExec != MobCmd.Empty)
            {
                doCmd(cmdToExec);
            }
            
            addCmds(getAiInput());
            showQueue();
            
        }
        public void showQueue()
        {
            Console.Write("{0}'s Command Queue: ", name);
            foreach (var cmd in cmdQueue)
            {
                Console.Write("{0}, ",cmd);
            }
            Console.WriteLine();
        }
        public bool addCmd(MobCmd newCmd)
        {
            if (cmdQueue.Count >= 5)
                return false;
            cmdQueue.Enqueue(newCmd);
            Console.WriteLine("Added " + newCmd + " to cmdQueue");
            return true;
        }
        public void addCmds(Queue<MobCmd> incoming)
        {
            if (incoming == null) { return; }
            while (incoming.Count > 0)
            {

                cmdQueue.Enqueue(incoming.Dequeue());
            }
        }
        public void addCmds(Stack<MobCmd> incoming)
        {
            incoming.Reverse();
            while(incoming.Count>0)
            {
                cmdStack.Push(incoming.Pop());
            }
        }
        public int addItems(Item newItem, int quantityToAdd)
        {
            int leftoverItemCount = quantityToAdd;
            while (quantityToAdd > 0)
            {
                // See if the mob already has that item in its inventory and if there's room to stack more
                if (inventory.Exists(r => (r.item != null) && (r.item == newItem) && (r.quantity < newItem.maxQuantity)))
                {
                    ItemRecord target = inventory.First(x => (x.item == newItem) && (x.quantity < newItem.maxQuantity));

                    //how many more we can stack
                    int maxToAdd = newItem.maxQuantity - target.quantity;

                    //add to the stack without overflowing
                    int deltaQ = Math.Min(quantityToAdd, maxToAdd);
                    target.alterQuantity(deltaQ);

                    quantityToAdd -= deltaQ;
                    leftoverItemCount -= deltaQ;
                } 
                else
                {
                    //Item not found in inventory or stack is full
                    if(inventory.Count < getStat(Stats.MaxCapacity))
                    {
                        //Add the item to the inventory with a count of zero
                        //and let the loop handle getting the count
                        inventory.Add(new ItemRecord(newItem, 0));
                    }
                    else
                    {
                        Console.WriteLine("Inventory is full");
                        return leftoverItemCount;
                    }
                }
            }
            return 0;
        }
        public void unequip(EquipLoc location)
        {

            if (equipArray[(int)location] == null)  //already empty
                return;
            else
            {
                addItems(equipArray[(int)location], 1);
                equipArray[(int)location] = null;
            }
        }
        public void equip(EquipLoc location, Equipment item)
        {
            if(!item.fits(location))
            {
                Console.WriteLine("{0} doesn't fit {1}!", item.name, location);
                return; 
            }

            if(equipArray[(int)location] != null)
            {
                unequip(location);
            }
            equipArray[(int)location] = item;
        }
        
        //Private Methods
        private Queue<MobCmd> getAiInput(/*TODO: Implement AI*/)
        {
            if (cmdQueue.Count == 0)
            {

                Queue<MobCmd> newQueue = new Queue<MobCmd>(new[] { MobCmd.Idle, MobCmd.Scan, MobCmd.Pause });
                return newQueue;
            }
            else return null;
        }
        private void scan()
        {
            int scanRadius = getStat(Stats.ScanRadius);
            scanData = new char[3, 2 * scanRadius + 1, 2*scanRadius+1];
            Console.WriteLine("\n--Scanning--");
            int scanX = 0;
            int scanY = 0;
            int scanZ = 0;
            for (int targetZ = z - 1; targetZ <= z + 1; targetZ++)
            {
                
                for (int targetX = x - scanRadius; targetX <= x + scanRadius; targetX++)
                {
                    for (int targetY = y - scanRadius; targetY <= y + scanRadius; targetY++)
                    {
                        if (targetZ < 0 || targetZ > Game.MAP_DEPTH)
                        {
                            scanData[scanZ, scanX, scanY] = '#';
                            switch (scanZ)
                            {
                                case 0:
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    break;
                                case 2:
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    break;
                                default:
                                    Console.ForegroundColor = ConsoleColor.Magenta; //should never happen
                                    break;
                            }
                            Console.Write(scanData[scanZ, scanX, scanY]);
                            scanY++;
                            continue;
                        }
                        if (targetX >= 0 && targetX <= Game.MAP_HEIGHT && targetY >= 0 && targetY <= Game.MAP_WIDTH)
                        {
                            scanData[scanZ, scanX, scanY] = theWorld[targetZ].map[targetX, targetY].getDispChar();
                            Console.ForegroundColor = theWorld[targetZ].map[targetX, targetY].getDispColor();
                        }
                        else
                        {
                            scanData[scanZ, scanX, scanY] = '#';
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                        Console.Write(scanData[scanZ, scanX, scanY]);
                        scanY++;
                    }
                    Console.Write("\n");
                    scanX++;
                    scanY = 0;
                }
                Console.Write('\n');
                scanZ++;
                scanX = 0;
            }
            Console.ResetColor();
            Console.WriteLine("--End Scan--\n");
        }
        private void moveTo(int newX, int newY, int newZ)
        {
            //try to move to the location
            if (theWorld[newZ].map[newX, newY].enter(this))    //returns false if space is occupied by a mob or block
            {
                theWorld[z].map[x, y].leave();
                this.x = newX;
                this.y = newY;
                this.z = newZ;
                if(theWorld[z].map[x,y].items.Count > 0)
                    cmdStack.Push(MobCmd.GetItems);
            }
            if (theWorld[z+1].map[x,y].isMovable())
            {
                cmdStack.Push(MobCmd.MoveDown);     //Fall if possible; 
            }
        }
        private void mine(int targetX, int targetY, int targetZ)
        {
            theWorld[targetZ].map[targetX, targetY].mine(getStat(Stats.MinePower));
        }
        private void doCmd(MobCmd cmd)
        {
            Console.WriteLine("{0} is executing the {1} command", name, cmd);
            MobCmd replacementCmd = MobCmd.Empty;
            
            /*  |-------> y+
             *  |
             *  |
             *  v x+
             */
            switch (cmd)
            {
                case MobCmd.MoveWest:
                    moveTo(x, y - 1, z);
                    break;
                case MobCmd.MoveSouth:
                    moveTo(x+1, y, z);
                    break;
                case MobCmd.MoveEast:
                    moveTo(x, y + 1, z);
                    break;
                case MobCmd.MoveNorth:
                    moveTo(x-1, y, z);
                    break;
                case MobCmd.MoveUp:
                    moveTo(x, y, z - 1);
                    break;
                case MobCmd.MoveDown:
                    moveTo(x, y, z + 1);
                    break;
                case MobCmd.Idle:
                    break;
                case MobCmd.GetItems:
                    foreach (ItemRecord index in theWorld[z].map[x,y].items)
                    {
                        Console.Write(" | ");
                        int leftovers = addItems(index.item, index.quantity);
                        index.alterQuantity(leftovers - index.quantity);
                        Console.Write(" | ");
                    }
                    Console.WriteLine("");
                    theWorld[z].map[x, y].cleanupItems();
                    break;
                case MobCmd.ActNorth:
                    break;
                case MobCmd.ActSouth:
                    break;
                case MobCmd.ActEast:
                    break;
                case MobCmd.ActWest:
                    break;
                case MobCmd.ActUp:
                    break;
                case MobCmd.ActDown:
                    break;
                case MobCmd.Scan:
                    scan();
                    break;
                case MobCmd.Pause:
                    theWorld[z].showMap(z);
                    Console.WriteLine("Paused by {0}, displaying floor map", name);
                    Console.WriteLine("Press any key to continue\n");
                    Console.ReadKey(true);
                    break;
                case MobCmd.Quit:
                    Console.WriteLine("QUIT");
                    break;
                default:
                    break;
             
            }
            if (replacementCmd != MobCmd.Empty)
            {
                doCmd( replacementCmd );
            }
        }
    
    }
}
