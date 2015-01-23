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
        public Mob(int startZ, int startX, int startY, char avatar, ConsoleColor color, String newName, Queue<MobAction> initQueue)
        {
            lastCmd = new MobAction();
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
                30, //MaxCapacity
                10  //MaxQueue
            });
            cmdStack = new Stack<MobAction>();
            cmdQueue = new Queue<MobAction>();
            EnqueueCmds(initQueue);
            

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
        public MobAction lastCmd {get; private set;}

        //Private Data
        private int[] baseStats = new int[Stats.GetNames(typeof(Stats)).Length];
        private Level[] theWorld;
        private Equipment[] equipArray = new Equipment[Enum.GetNames(typeof(EquipLoc)).Length];
        
        private List<ItemRecord> inventory = new List<ItemRecord>();
        private Stack<MobAction> cmdStack;
        private Queue<MobAction> cmdQueue;

        //Public Methods
        public int GetStat(Stats stat)
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
        public void Tick()
        {
            MobAction cmdToExec = new MobAction();
            Console.WriteLine("\nMob.Tick() from {0}", name);
            
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
            DoCmd(cmdToExec);

            
            EnqueueCmds(GetAiInput());
            ShowQueue();
            
        }
        public void ShowQueue()
        {
            Console.Write("{0}'s Command Queue: ", name);
            foreach (var command in cmdQueue)
            {
                Console.Write("{0}, ", command.cmd);
            }
            Console.WriteLine();
        }
        public bool EnqueueCmd(MobAction newCmd)
        {
            if (cmdQueue.Count >= 5)
                return false;
            cmdQueue.Enqueue(newCmd);
            Console.WriteLine("Added " + newCmd + " to cmdQueue");
            return true;
        }
        public void EnqueueCmds(Queue<MobAction> incoming)
        {
            if (incoming == null) { return; }
            while (incoming.Count > 0)
            {

                cmdQueue.Enqueue(incoming.Dequeue());
            }
        }
        public void PushCmd(MobAction newCmd)
        {
            cmdStack.Push(newCmd);
        }
        public void PushCmds(Stack<MobAction> incoming)
        {
            incoming.Reverse();
            while(incoming.Count>0)
            {
                cmdStack.Push(incoming.Pop());
            }
        }
        public int GainItems(Item newItem, int quantityToAdd)
        {
            int leftoverItemCount = quantityToAdd;
            while (quantityToAdd > 0)
            {
                // See if the mob already has that item in its inventory and if there's room to stack more
                if (inventory.Exists(r => (r.item != null) && (r.item.id == newItem.id) && (r.quantity < newItem.maxQuantity)))
                {
                    ItemRecord target = inventory.First(x => (x.item.id == newItem.id) && (x.quantity < newItem.maxQuantity));

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
                    //Item not found in inventory or stack is full
                    if(inventory.Count < GetStat(Stats.MaxCapacity))
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
            return 0;   // No leftovers
        }
        public void Unequip(EquipLoc location)
        {

            if (equipArray[(int)location] == null)  //already empty
                return;
            else
            {
                GainItems(equipArray[(int)location], 1);
                equipArray[(int)location] = null;
            }
        }
        public void Equip(EquipLoc location, Equipment item)
        {
            if(!item.fits(location))
            {
                Console.WriteLine("{0} doesn't fit {1}!", item.id, location);
                return; 
            }

            if(equipArray[(int)location] != null)
            {
                Unequip(location);
            }
            equipArray[(int)location] = item;
        }
        
        //Private Methods
        private Queue<MobAction> GetAiInput(/*TODO: Implement AI*/)
        {
            if (cmdQueue.Count == 0)
            {
                Queue<MobAction> newQueue = new Queue<MobAction>();
                newQueue.Enqueue(new MobAction());
                newQueue.Enqueue(new MobAction(MobActEnum.Scan));
                newQueue.Enqueue(new MobAction(MobActEnum.Pause));

                return newQueue;
            }
            else return null;
        }
        private void Attack(int targetX, int targetY, int targetZ)
        {
            if (theWorld[z].map[x - 1, y].isMovable() == false)
            {
                if (theWorld[z].map[x - 1, y].mob != null)
                {
                    theWorld[z].map[x - 1, y].mob.TakeDamage(this);
                    Console.Write("TODO: Attack");
                }

            }
         }
        private void Mine(int targetX, int targetY, int targetZ)
        {
            theWorld[targetZ].map[targetX, targetY].mine(GetStat(Stats.MinePower));
        }
        private void MoveTo(int newX, int newY, int newZ)
        {
            //try to move to the location
            if (theWorld[newZ].map[newX, newY].enter(this))    //returns false if space is occupied by a mob or block
            {
                theWorld[z].map[x, y].leave();
                this.x = newX;
                this.y = newY;
                this.z = newZ;
                if(theWorld[z].map[x,y].items.Count > 0)
                    cmdStack.Push(new MobAction(MobActEnum.Loot));
            }
            if (theWorld[z+1].map[x,y].isMovable())
            {
                cmdStack.Push(new MobAction(MobActEnum.Move, "D"));     //Fall if possible; 
            }
        }
        private void Scan()
        {
            int scanRadius = GetStat(Stats.ScanRadius);
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
        private void Loot()
        {
            int available;

            Console.WriteLine("You have no loot function!");

        }

        private void DoCmd(MobAction cmdObj)//target is X,Y,Z
        {
            int targetX = x + Direction.convert(cmdObj.directionString)[0];
            int targetY = y + Direction.convert(cmdObj.directionString)[1];
            int targetZ = z + Direction.convert(cmdObj.directionString)[2];
            
            //Validate Target
            if (
                targetX < 0 || targetX > Game.MAP_HEIGHT ||
                targetY < 0 || targetY > Game.MAP_WIDTH ||
                targetZ < 0 || targetZ > Game.MAP_DEPTH ||
                cmdObj.repeat < 1
              ) { return; }
            if (cmdObj.repeat > 1)
            {
                cmdObj.repeat--;
                PushCmd(cmdObj);
            }
            Console.WriteLine("{0} is executing the {1} command", name, cmdObj.cmd);
           
            
            switch (cmdObj.cmd)
            {
                case MobActEnum.Move:
                    if (theWorld[targetZ].map[targetX, targetY].isMovable())
                    {
                        MoveTo(targetX, targetY, targetZ);
                    }
                    break;
                case MobActEnum.Attack:
                    if (theWorld[targetZ].map[targetX, targetY].mob != null)
                    {
                        Attack(targetX, targetY, targetZ);
                    }break;
                case MobActEnum.Mine:
                    if (theWorld[targetZ].map[targetX, targetY].block.id != BlockID.Air)
                    {
                        Mine(targetX, targetY, targetZ);
                    }break;
                case MobActEnum.Idle:
                    break;
                case MobActEnum.Scan:
                    Scan();
                    break;
                case MobActEnum.Quit:
                    break;
                case MobActEnum.Pause:
                    Console.WriteLine("Paused by {0}", this.name);
                    //Console.ReadLine();
                    Console.WriteLine("Breakpoint");
                    break;
                case MobActEnum.Loot:
                    Loot();
                    break;
                default:
                    break;
            }
            
        }
        private bool TakeDamage(Mob attacker)
        {
            baseStats[(int)Stats.HP] -= (5 + attacker.GetStat(Stats.AttackPower) - GetStat(Stats.Armor));
            if(baseStats[(int)Stats.HP] <= 0)
            {
                //Die, drop items
                Console.WriteLine("{0} died!  Oh, no!", name);
                return true;
            }
            else
            return false;
        }
       /*  |-------> y+
        *  |
        *  |
        *  v x+
        *
    
        private void doCmd(MobCmd cmd)
        {
            Console.WriteLine("{0} is executing the {1} command", name, cmd);
            MobCmd replacementCmd = MobCmd.Empty;
            
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
                    loot();
                    break;
                case MobCmd.ActNorth: // x-1
                    attackOrMine(x - 1, y, z);
                    break;
                case MobCmd.ActSouth:
                    attackOrMine(x + 1, y, z);
                    break;
                case MobCmd.ActEast: // y+1
                    attackOrMine(x, y + 1, z);
                    break;
                case MobCmd.ActWest:
                    attackOrMine(x, y - 1, z);
                    break;
                case MobCmd.ActUp:
                    attackOrMine(x, y, z-1);
                    break;
                case MobCmd.ActDown:
                    attackOrMine(x, y, z+1);
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
        */
    }
}