using ValueVariant;

namespace _OnlyOneGame.Scripts.Components
{
    [ValueVariant]
    public partial struct Item : IValueVariant<Item, DeployableItemType, MinionType, ThrowableType>
    {
    };
    
    public enum DeployableItemType
    {
        Wall,
        Turret,
        AutoRepairModule,
        BubbleShieldModule,
        MiningModule,
        SpawnPoint,
        Landmine,
        BarbedWire,
    }

    public enum ThrowableType
    {
        Grenade,
        Arrows,
        FlashBang,
    }
    
    public enum MinionType
    {
        Melee,
        Ranged,
        Tank,
        Healer
    }
}