using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using ValueVariant;

namespace _OnlyOneGame.Scripts.Components
{
    public struct OnPlayerCharacter : IComponentData
    {
        public FixedList512Bytes<Item> InventoryStack;
        public Option<ActionCommand> ActionCommandOpt;
        public Option<OnGoingAction> OnGoingAction;

        public FixedList128Bytes<PlayerEvent> Events;

        public float SilencedDuration;
    }

    [ValueVariant]
    public partial struct PlayerEvent : IValueVariant<PlayerEvent
        , MeleeAttackStartedEvent
        , ItemPickedUpEvent
        , ItemCraftedEvent
        , UnBuiltEvent
        , ResourceGatheredEvent
        , ItemStackChangedEvent
        , DroppedItemEvent
        , ThrownItemEvent
    >
    { }

    [ValueVariant]
    public partial struct OnGoingActionData : IValueVariant<OnGoingActionData
        , MeleeAttackingAction
        , UnBuildingAction
    > { }

    [ValueVariant]
    public partial struct ActionCommand : IValueVariant<ActionCommand,
        UnBuildCommand, 
        PickupItemCommand, 
        CraftItemCommand, 
        MineResourceCommand, 
        CycleStackCommand
    > { }

    public struct OnGoingAction
    {
        public float StartTime;
        public float Duration;
        public OnGoingActionData Data;
    }

    public struct MeleeAttackCommand
    {
        public float3 Direction;
    }
    public struct MeleeAttackingAction
    {
        public float3 Direction;
    }
    public struct MeleeAttackStartedEvent
    {
        public float3 Direction;
    }

    public struct UnBuildCommand { }

    public struct UnBuildingAction { }
    public struct UnBuiltEvent { }
    
    public struct PickupItemCommand { }
    public struct ItemPickedUpEvent { public Item Item; }

    public struct CraftItemCommand
    {
        public Item ItemToCraft;
    }
    public struct ItemCraftedEvent
    {
        public Item CraftedItem;
    }

    public struct MineResourceCommand { }
    public struct MineResourceAction { }
    public struct ResourceGatheredEvent { }
    
    public struct CycleStackCommand { }
    public struct ItemStackChangedEvent { }

    public struct DropItemCommand { }
    public struct DroppedItemEvent
    {
        public Item Item;
    }

    public struct ThrowItemCommand
    {
        public float3 ThrowVelocity;
    }
    public struct ThrownItemEvent
    {
        public Item Item;
        public float3 ThrowVelocity;
    }
}