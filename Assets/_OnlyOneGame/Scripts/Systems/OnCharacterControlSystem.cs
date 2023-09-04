using System.Linq;
using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateBefore(typeof(OnPlayerCharacterSystem))]
    public partial class OnCharacterControlSystem : SystemBase
    {
        private NativeList<(float3, Entity)> m_OverlapSphereResultBuffer;
        private ComponentLookup<LocalTransform> m_LocalTransformLookup;
        protected override void OnCreate()
        {
            RequireForUpdate<OnPlayerInput>();
            RequireForUpdate<SyncedIdToEntityMap>();
            RequireForUpdate<BuildPhysicsWorldData>();
            RequireForUpdate<LocalTransform>();
            m_OverlapSphereResultBuffer = new NativeList<(float3, Entity)>(Allocator.Persistent);
            m_LocalTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        }
        
        protected override void OnDestroy()
        {
            m_OverlapSphereResultBuffer.Dispose();
        }

        protected override void OnUpdate()
        {
            CompleteDependency();
            
            m_LocalTransformLookup.Update(this);
            var buildPhysicsWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>();
            var syncedIdToEntityMap = SystemAPI.ManagedAPI.GetSingleton<SyncedIdToEntityMap>();

            foreach (var (playerInput, playerRo, entity)
                     in SystemAPI.Query<OnPlayerInput, RefRO<OnPlayer>>().WithAll<Simulate>().WithEntityAccess())
            {
                if (syncedIdToEntityMap.TryGet(playerRo.ValueRO.ControlledCharacterSyncedId, out var controlledCharacterEntity))
                {
                    if (SystemAPI.GetComponentLookup<OnPlayerCharacter>().TryGetRw(controlledCharacterEntity, out var controlledCharacterRw))
                    {
                        controlledCharacterRw.ValueRW.Input = new OnPlayerCharacterInput
                        {
                            Movement = playerInput.MovementInput,
                            Look = playerInput.LookInput,
                            PickupButtonTap = playerInput.PickupButtonTap.IsSet,
                            PickupButtonReleasedFromHold = playerInput.PickupButtonReleasedFromHold.IsSet,
                            DropButtonTap = playerInput.DropButtonTap.IsSet,
                            DropButtonReleasedFromHold = playerInput.DropButtonReleasedFromHold.IsSet,
                            ActionButton0Tap = playerInput.Action0.IsSet,
                        };
                    }
                }
            }
        }
    }
}