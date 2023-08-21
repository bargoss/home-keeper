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
        [GhostField] public BytesAs<Option<OnGoingAction>, Data32Bytes> OnGoingActionOpt;
        [GhostField] public BytesAs<FixedList128Bytes<PlayerEvent>, Data128Bytes> Events;

        [GhostField] public float CommandsBlockedDuration;
        [GhostField] public float MovementBlockedDuration;


        // these fields can be removed if we be careful about execution order
        // input:
        [GhostField] public float2 MovementInput;
        [GhostField] public float2 LookInput;
        [GhostField] public BytesAs<Option<ActionCommand>, Data32Bytes> ActionCommandOpt;
        
        public void SetMovementInput(float2 movementInput)
        {
            MovementInput = movementInput;
        }
        
        public void SetLookInput(float2 lookInput)
        {
            LookInput = lookInput;
        }
        
        public void SetActionCommandOpt(Option<ActionCommand> actionCommandOpt)
        {
            ActionCommandOpt = actionCommandOpt;
        }
        
        

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