using Unity.Collections;
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
                AddComponent(entity,
                    new OnPlayerCharacter
                    {
                        InventoryStack = new FixedList512Bytes<Item>()
                        {
                            new Item(DeployableItemType.Landmine),
                            new Item(MinionType.Healer),
                            new Item(ThrowableType.FlashBang)
                        },
                        ActionCommandOpt = Option<ActionCommand>.Some(new ActionCommand(new CommandCraftItem(){ItemToCraft = new Item(MinionType.Melee)})),
                        OnGoingActionOpt = Option<OnGoingAction>.Some(new OnGoingAction(){Duration = 1.19f, StartTime = 0.12f, Data = new OnGoingActionData(new ActionUnbuilding(){})}),
                        Events = new FixedList128Bytes<PlayerEvent>()
                        {
                            new PlayerEvent(new EventMeleeAttackStarted(){Direction = new float3(1.1f,2.1f,3.1f)}),
                            new PlayerEvent(new EventThrownItem(){Item = new Item(DeployableItemType.Landmine), ThrowVelocity = new float3(5.1f,6.1f,7.1f)}),
                        },
                        SilencedDuration = 999.999f
                    });
            }
        }
    }
}