using ValueVariant;

namespace _OnlyOneGame.Scripts.Components
{
    
    public struct Item
    {
        public ItemType ItemType;
        
        public Item(ItemType itemType)
        {
            ItemType = itemType;
        }
    };

    public enum ItemType
    {
        Metal,
        Energy,
        Wall,
        Turret,
        AutoRepairModule,
        BubbleShieldModule,
        MiningModule,
        SpawnPoint,
        Landmine,
        BarbedWire,
        Grenade,
        Arrows,
        FlashBang,
        MinionMelee,
        MinionRanged,
        MinionTank,
        MinionHealer
    }
}