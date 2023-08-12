using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using ValueVariant;

namespace _OnlyOneGame.Scripts.Components
{
    public struct OnPlayerCharacter : IComponentData
    {
        // state:
        public FixedList512Bytes<Item> InventoryStack;
        public Option<OnGoingAction> OnGoingActionOpt;
        public FixedList128Bytes<PlayerEvent> Events;
        public float CommandsBlockedDuration;
        public float MovementBlockedDuration;

        // input:
        public float MovementInput;
        public float LookInput;
        public Option<ActionCommand> ActionCommandOpt;

        // stats:
        public int InventoryCapacity;
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
        , ActionDismantling
    > { }

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
    public struct ActionMeleeAttacking
    {
        public float3 Direction;
        
        public ActionMeleeAttacking(float3 direction)
        {
            Direction = direction;
        }
    }
    public struct EventMeleeAttackStarted
    {
        public float3 Direction;
    }

    public struct CommandDismantle { }

    public struct ActionDismantling
    {
        public Entity Target;

        public ActionDismantling(Entity target)
        {
            Target = target;
        }
    }
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