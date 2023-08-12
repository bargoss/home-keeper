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
        public Option<OnGoingAction> OnGoingActionOpt;

        public FixedList128Bytes<PlayerEvent> Events;

        public float SilencedDuration;
    }

    [ValueVariant]
    public partial struct PlayerEvent : IValueVariant<PlayerEvent
        , EventMeleeAttackStarted
        , EventItemPickedUp
        , EventItemCrafted
        , EventUnbuilt
        , EventResourceGathered
        , EventItemStackChanged
        , EventDroppedItem
        , EventThrownItem
    >
    { }

    [ValueVariant]
    public partial struct OnGoingActionData : IValueVariant<OnGoingActionData
        , ActionMeleeAttacking
        , ActionUnbuilding
    > { }

    [ValueVariant]
    public partial struct ActionCommand : IValueVariant<ActionCommand,
        CommandUnbuild, 
        CommandPickupItem, 
        CommandCraftItem, 
        CommandMineResource, 
        CommandCycleStack
    > { }

    public struct OnGoingAction
    {
        public float StartTime;
        public float Duration;
        public OnGoingActionData Data;
    }

    public struct CommandMeleeAttack
    {
        public float3 Direction;
    }
    public struct ActionMeleeAttacking
    {
        public float3 Direction;
    }
    public struct EventMeleeAttackStarted
    {
        public float3 Direction;
    }

    public struct CommandUnbuild { }

    public struct ActionUnbuilding { }
    public struct EventUnbuilt { }
    
    public struct CommandPickupItem { }
    public struct EventItemPickedUp { public Item Item; }

    public struct CommandCraftItem
    {
        public Item ItemToCraft;
    }
    public struct EventItemCrafted
    {
        public Item CraftedItem;
    }

    public struct CommandMineResource { }
    public struct ActionMineResource { }
    public struct EventResourceGathered { }
    
    public struct CommandCycleStack { }
    public struct EventItemStackChanged { }

    public struct CommandDropItem { }
    public struct EventDroppedItem
    {
        public Item Item;
    }

    public struct CommandThrowItem
    {
        public float3 ThrowVelocity;
    }
    public struct EventThrownItem
    {
        public Item Item;
        public float3 ThrowVelocity;
    }
}