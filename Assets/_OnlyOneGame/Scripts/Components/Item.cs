using ValueVariant;

namespace _OnlyOneGame.Scripts.Components
{
    [ValueVariant]
    public partial struct Item : IValueVariant<Item, ItemTypeDeployable, ItemTypeMinion, ItemTypeThrowable, ItemTypeResource>
    {
    };

    public enum ItemTypeResource
    {
        Metal,
        Energy,
    }
    
    public enum ItemTypeDeployable
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

    public enum ItemTypeThrowable
    {
        Grenade,
        Arrows,
        FlashBang,
    }
    
    public enum ItemTypeMinion
    {
        Melee,
        Ranged,
        Tank,
        Healer
    }
}