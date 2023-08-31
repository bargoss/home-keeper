using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    public class OnPlayerCharacterAuthoring : MonoBehaviour
    {
        public class OnPlayerCharacterBaker : Baker<OnPlayerCharacterAuthoring>
        {
            public override void Bake(OnPlayerCharacterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var events = new FixedList128Bytes<PlayerEvent>();
                var event0 = new PlayerEvent(new EventMeleeAttackStarted(){Direction = new float3(1.1f,2.1f,3.1f)});
                var event1 = new PlayerEvent(new EventThrownItem(){Item = new Item(ItemTypeDeployable.Landmine), ThrowVelocity = new float3(5.1f,6.1f,7.1f)});
                var sizeOfEvent0A = UnsafeUtility.SizeOf<PlayerEvent>();
                var sizeOfEvent0B = Marshal.SizeOf<PlayerEvent>();
                var sizeOfEvent1A = UnsafeUtility.SizeOf<PlayerEvent>();
                var sizeOfEvent1B = Marshal.SizeOf<PlayerEvent>();
                
                events.Add(event0);
                events.Add(event1);
                
                AddComponent(entity,
                    new OnPlayerCharacter
                    {
                        InventoryStack = new FixedList128Bytes<Item>()
                        {
                            new Item(ItemTypeDeployable.Landmine),
                            new Item(ItemTypeMinion.Healer),
                            new Item(ItemTypeThrowable.FlashBang)
                        },
                        ActionCommandOpt = Option<ActionCommand>.Some(new ActionCommand(new CommandCraftItem(){ItemToCraft = new Item(ItemTypeMinion.Melee)})),
                        OnGoingActionOpt = Option<OnGoingAction>.Some(new OnGoingAction(){Duration = 1.19f, StartTime = 0.12f, Data = new OnGoingActionData(new ActionDismantling(){})}),
                        Events = events,
                        CommandsBlockedDuration = 0,
                        MovementBlockedDuration = 0,
                    });
            }
        }
    }
}