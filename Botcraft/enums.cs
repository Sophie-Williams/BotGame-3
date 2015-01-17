namespace Botcraft
{
    enum BlockID { Air, Dirt, Stone, Wall }

    enum EquipLoc { Body, Tool, Battery, Trinket1, Trinket2 }

    enum Stats { HP, Armor, Speed, ScanRadius, AttackPower, MinePower, MaxCapacity }
    
    enum MobCmd {
        MoveNorth, MoveSouth, MoveEast, MoveWest, MoveUp, MoveDown,
        ActNorth, ActSouth, ActEast, ActWest, ActUp, ActDown,
        GetItems, Empty, Idle, Scan, Quit, Pause }

}