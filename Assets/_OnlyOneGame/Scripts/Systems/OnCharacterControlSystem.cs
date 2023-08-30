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
    [UpdateBefore(typeof(OnPlayerSystem))]
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
                        // reset:
                        controlledCharacterRw.ValueRW.SetActionCommandOpt(Option<ActionCommand>.None());
                        
                        var characterPosition = m_LocalTransformLookup[controlledCharacterEntity].Position;
                        
                        //input is not correct here, its just the default input
                        controlledCharacterRw.ValueRW.SetMovementInput(playerInput.MovementInput);
                        controlledCharacterRw.ValueRW.SetLookInput(playerInput.LookInput);
                        if (playerInput.Action0.IsSet)
                        {
                            controlledCharacterRw.ValueRW.SetActionCommandOpt(Option<ActionCommand>.Some(new CommandMeleeAttack(playerInput.MovementInput.X0Y())));                            
                        }
                        
                        
                        /*
                        public static void GetAllOverlapSphereNoAlloc(
                            this ref BuildPhysicsWorldData buildPhysicsWorldData,
                            float3 point,
                            float radius,
                            ref NativeList<(float3, Entity)> results
                        )
                        */

                        bool itemNearby = false;
                        bool hasAnyItem = controlledCharacterRw.ValueRO.InventoryStack.Get().Length > 0;
                        
                        m_OverlapSphereResultBuffer.Clear();
                        buildPhysicsWorld.GetAllOverlapSphereNoAlloc(characterPosition, 1.5f, ref m_OverlapSphereResultBuffer);
                        foreach (var (qPos, qEntity) in m_OverlapSphereResultBuffer)
                        {
                            if (SystemAPI.HasComponent<GroundItem>(qEntity))
                            {
                                itemNearby = true;
                            }
                        }

                        if (playerInput.PickupButtonTap.IsSet)
                        {
                            if (itemNearby)
                            {
                                controlledCharacterRw.ValueRW.SetActionCommandOpt(Option<ActionCommand>.Some(new CommandPickupItem()));
                            }
                        }

                        if (playerInput.DropButtonTap.IsSet)
                        {
                            if (hasAnyItem)
                            {
                                controlledCharacterRw.ValueRW.SetActionCommandOpt(Option<ActionCommand>.Some(new CommandDropItem()));
                            }
                        }
                        
                        if (playerInput.DropButtonReleasedFromHold.IsSet)
                        {
                            if (hasAnyItem)
                            {
                                controlledCharacterRw.ValueRW.SetActionCommandOpt(Option<ActionCommand>.Some(new CommandThrowItem(playerInput.LookInput.X0Y())));
                            }
                        }
                    }
                }
            }
        }
    }
}