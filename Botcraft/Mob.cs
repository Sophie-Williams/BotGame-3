using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botcraft
{
    enum MobCmd
	{
        MoveNorth, MoveSouth, MoveEast, MoveWest, MoveUp, MoveDown,
        ActNorth, ActSouth, ActEast, ActWest, ActUp, ActDown,
        GetItems, Empty, Idle, Scan, Quit, Pause
	}
    
    enum EquipLoc { Body, Tool, Battery, Trinket1, Trinket2 }
    
    class Mob
    {
        //Read-only values

        //Stats - Base
        private int _speed;
        private int _attackPower;
        private int _scanRadius;
        private int _minePower;
        private int _maxCapacity;

        // Stats - Calculated from base and all equipped items
        public int speed 
        { 
            get 
            { 
                int output = 
                    _speed;
                foreach (Equipment slot in equipArray)
                {
                    if (slot != null)
                        output += slot.speed;
                }
                return output;
            }
        }
        public int attackPower
        {
            get
            {
                int output = _attackPower;
                foreach (Equipment slot in equipArray)
                {
                    if (slot != null)
                        output += slot.attackPower;
                }
                return output;
            }
        }
        public int scanRadius
        {
            get
            {
                int output = _scanRadius;
                foreach (Equipment slot in equipArray)
                {
                    if(slot != null)
                        output += slot.scanRadius;
                }
                return output;
            }
        }
        public int minePower
        {
            get
            {
                int output = _minePower;
                foreach (Equipment slot in equipArray)
                {
                    if (slot != null)
                        output += slot.minePower;
                }
                return output;
            }
        }
        public int maxCapacity
        {
            get
            {
                int output = _maxCapacity;
                foreach (Equipment slot in equipArray)
                {
                    if (slot != null)
                        output += slot.maxCapacity;
                }
                return output;
            }
        }
        
        
        //Other
        public int x { get; private set; }
        public int y { get; private set; }
        public int z { get; private set; }
        public String name { get; private set; }
        public char dispChar { get; private set; }
        public char[,] scanData { get; private set; }
        
        //Constructors & Related Methods
        public Mob(Level[] world, char avatar) : this(world, 0,0,0, avatar, "Bot") { }
        public Mob(Level[] world, int startZ, int startX, int startY, char avatar, String newName)
        {
            name = newName;
            theWorld = world;
            x = startX;
            y = startY;
            z = startZ;
            dispChar = avatar;
            setStats(3, 3, 0, 0, 30);
            //DEBUG
            cmdQueue = testQueue;
            //END DEBUG

            theWorld[z].map[x, y].enter(this);
        }
        
        private void setStats(int setSpeed, int setScan, int setMine, int setAttack, int newMaxCap)
        {
            _speed = setSpeed;
            _scanRadius = setScan;
            _minePower = setMine;
            _attackPower = setAttack;
            _maxCapacity = newMaxCap;
        }
        
        //Private Data
        private Level[] theWorld;
        private Equipment[] equipArray = new Equipment[Enum.GetNames(typeof(EquipLoc)).Length];
        
        private List<Item> inventory;
        private Stack<MobCmd> cmdStack;
        private Queue<MobCmd> cmdQueue;
        private Queue<MobCmd> testQueue = new Queue<MobCmd>(new MobCmd[] {MobCmd.MoveWest, MobCmd.MoveSouth, MobCmd.Scan, MobCmd.MoveEast, MobCmd.MoveNorth, MobCmd.Pause, MobCmd.Pause });


        //Public Methods
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
            getAiInput();

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
            cmdQueue.Concat(incoming);
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
                if(inventory.Count < maxCapacity)   //inventory is not full
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
                return new Queue<MobCmd>(new[] { MobCmd.Idle, MobCmd.Scan });
            else return null;
        }
        private void scan()
        {
            scanData = new char[2 * scanRadius + 1, 2 * scanRadius + 1];
            Console.WriteLine("--Scanning--");
            int scanX = 0;
            int scanY = 0;
            for (int targetX = x - scanRadius; targetX <= x + scanRadius; targetX++)
            {
                for (int targetY = y - scanRadius; targetY <= y + scanRadius; targetY++)
                {
                    if (targetX > 0 && targetX < Game.MAP_HEIGHT && targetY > 0 && targetY < Game.MAP_WIDTH)
                        scanData[scanX, scanY] = theWorld[z].map[targetX, targetY].getDispChar();
                    else
                        scanData[scanX, scanY] = '#';
                    Console.Write(scanData[scanX, scanY]);
                    scanY++;
                }
                Console.Write("\n");
                scanX++;
                scanY = 0;
            }
            Console.WriteLine("--End Scan--");
        }
        private void moveTo(int newX, int newY, int depth)
        {
            if (theWorld[depth].map[newX, newY].enter(this))
            {
                theWorld[z].map[x, y].leave();
                this.x = newX;
                this.y = newY;
                this.z = depth;
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
                    Console.WriteLine("");
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
