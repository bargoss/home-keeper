using _OnlyOneGame.Scripts.Components.Data;
using DefaultNamespace;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using ValueVariant;

namespace _OnlyOneGame.Scripts.Components
{
    public struct OnPlayerCharacter : IComponentData
    {
        // state:
        [GhostField] public BytesAs<FixedList128Bytes<Item>, Data128Bytes> InventoryStack;
        [GhostField] public Option<OnGoingAction> OnGoingActionOpt;
        
        [GhostField] public int CommandsBlockedDuration;
        [GhostField] public int MovementBlockedDuration;
        [GhostField] public float2 LookDirection;
        
        // events
        public FixedList128Bytes<PlayerEvent> Events; // [GhostField]

        // input
        public OnPlayerCharacterInput Input;
        
        
        // stats:
        public int InventoryCapacity;
    }
    
    public struct OnPlayerCharacterInput
    {
        public float2 Movement;
        public float2 Look;
        public bool DropButtonTap;
        public bool DropButtonReleasedFromHold;
        
        public bool PickupButtonTap;
        public bool PickupButtonReleasedFromHold;
        
        public bool ActionButton0Tap;
        public bool ActionButton1Tap;
        public bool ActionButton2Tap;
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

    //[ValueVariant]
    //public partial struct OnGoingActionData : IValueVariant<OnGoingActionData
    //    , ActionMeleeAttacking
    //    , ActionDismantling
    //> { }

    [ValueVariant]
    public partial struct ActionCommand : IValueVariant<ActionCommand,
        CommandDismantle, 
        CommandPickupItem, 
        CommandCraftItem, 
        CommandMineResource, 
        CommandCycleStack,
        CommandMeleeAttack,
        CommandThrowItem,
        CommandDropItem
    > { }

    public struct OnGoingAction
    {
        public float StartTime;
        public float Duration;
        public OnGoingActionData Data;

        public OnGoingAction(float startTime, float duration, OnGoingActionData data)
        {
            StartTime = startTime;
            Duration = duration;
            Data = data;
        }
    }

    public struct CommandMeleeAttack
    {
        public float3 Direction;
        
        public CommandMeleeAttack(float3 direction)
        {
            Direction = direction;
        }
    }
    
    public struct EventMeleeAttackStarted
    {
        public float3 Direction;
        
        public EventMeleeAttackStarted(float3 direction)
        {
            Direction = direction;
        }
    }

    public struct CommandDismantle { }

    public struct EventUnbuilt { }
    
    public struct CommandPickupItem { }

    public struct EventItemPickedUp
    {
        public Item Item;
        
        public EventItemPickedUp(Item item)
        {
            Item = item;
        }
    }

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
        
        public EventDroppedItem(Item item)
        {
            Item = item;
        }
    }

    public struct CommandThrowItem
    {
        public float3 ThrowVelocity;
        
        public CommandThrowItem(float3 throwVelocity)
        {
            ThrowVelocity = throwVelocity;
        }
    }
    public struct EventThrownItem
    {
        public Item Item;
        public float3 ThrowVelocity;
        
        public EventThrownItem(Item item, float3 throwVelocity)
        {
            Item = item;
            ThrowVelocity = throwVelocity;
        }
    }
}