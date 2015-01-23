namespace Botcraft
{
    enum BlockID { Air, Dirt, Stone, Wall }

    enum EquipLoc { Body, Tool, Battery, Trinket1, Trinket2 }

    enum Stats { HP, Armor, Speed, ScanRadius, AttackPower, MinePower, MaxCapacity, MaxQueue }

    enum ItemID { Item1, Item2 }

    enum MobCmd1 {
        MoveNorth, MoveSouth, MoveEast, MoveWest, MoveUp, MoveDown,
        ActNorth, ActSouth, ActEast, ActWest, ActUp, ActDown,
        GetItems, Empty, Idle, Scan, Quit, Pause }

    enum MobActEnum { Move, Attack, Mine, Idle, Scan, Quit, Pause, Loot}

}