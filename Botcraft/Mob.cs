using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    class Mob
    {
        //Read-only values

        private int[] baseStats = new int[Stats.GetNames(typeof(Stats)).Length];
        // see Public Method getStat(Stats stat)
      

        
        //Other
        public int x { get; private set; }
        public int y { get; private set; }
        public int z { get; private set; }
        public String name { get; private set; }
        public char dispChar { get; private set; }
        public ConsoleColor dispColor { get; set; }
        public char[,,] scanData { get; private set; }
        
        //Constructors & Related Methods
        public Mob(Level[] world, int startZ, int startX, int startY, char avatar, ConsoleColor color, String newName, Queue<MobCmd> initQueue)
        {
            name = newName;
            theWorld = world;
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
            //DEBUG
            cmdQueue = initQueue;
            //END DEBUG

            theWorld[z].map[x, y].enter(this);
        }
        
        private void setStats(int[] statBlock)
        {
            for (int i = 0; i < Enum.GetNames(typeof (Stats)).Length; i++)
            {
                baseStats[i] = statBlock[i];
            }
        }
        
        //Private Data
        private Level[] theWorld;
        private Equipment[] equipArray = new Equipment[Enum.GetNames(typeof(EquipLoc)).Length];
        
        private List<Item> inventory;
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
            if (cmdToExec != MobCmd.Empty)
            {
                doCmd(cmdToExec);
            }
            
            addCmds(getAiInput());
            showQueue();
            
        }

        private void showQueue()
        {
            foreach (var cmd in cmdQueue)
            {
                Console.Write(cmd + ", ");
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
        public void unequip(EquipLoc location)
        {

            if (equipArray[(int)location] == null)  //already empty
                return;
            else
            {
                if(inventory.Count < getStat(Stats.MaxCapacity))   //inventory is not full
                {
                    inventory.Add(equipArray[(int)location]);
                    equipArray[(int)location] = null;
                }
                else
                {
                    // drop it on the ground or it will be lost
                }
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
            Console.WriteLine("--Scanning--");
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
            Console.WriteLine("--End Scan--");
        }
        private void moveTo(int newX, int newY, int depth)
        {
            //try to move to the location
            if (theWorld[depth].map[newX, newY].enter(this))    //returns false if space is occupied by a mob or block
            {
                theWorld[z].map[x, y].leave();
                this.x = newX;
                this.y = newY;
                this.z = depth;
            }
            if (theWorld[z+1].map[x,y].isMovable())
            {
                if (cmdStack == null)
                {
                    cmdStack = new Stack<MobCmd>();
                }
                cmdStack.Push(MobCmd.MoveDown);     //Fall if possible; 
            }
        }
        private void doCmd(MobCmd cmd)
        {
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
                    Console.WriteLine("Breakpoint");
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
