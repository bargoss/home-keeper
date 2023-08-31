using System;
using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct GroundItemSystems : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<OnPrefabs>();
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var prefabs = SystemAPI.GetSingleton<OnPrefabs>();
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            var tick = networkTime.ServerTick;
            
            // handle deploying items
            foreach (var (groundItem, localTransform, physicsVelocity, activatedItem, entity) in SystemAPI
                         .Query<RefRO<GroundItem>, LocalTransform, RefRO<PhysicsVelocity>, ActivatedItem>()
                         .WithAll<Simulate>().WithEntityAccess())
            {

                if (tick.TicksSince(activatedItem.ActivatedTick) > activatedItem.ActivationDurationTicks)
                {
                    groundItem.ValueRO.Item.Get().Switch(
                        deployable =>
                        {
                            switch (deployable)
                            {
                                case ItemTypeDeployable.Turret:
                                {
                                    var turret = ecb.Instantiate(prefabs.TurretPrefab);
                                    ecb.SetLocalPositionRotation(turret, localTransform.Position,
                                        quaternion.LookRotationSafe(activatedItem.Direction, Utility.Forward));
                                    break;
                                }
                                case ItemTypeDeployable.Wall:
                                case ItemTypeDeployable.AutoRepairModule:
                                case ItemTypeDeployable.BubbleShieldModule:
                                case ItemTypeDeployable.MiningModule:
                                case ItemTypeDeployable.SpawnPoint:
                                case ItemTypeDeployable.Landmine:
                                case ItemTypeDeployable.BarbedWire:
                                    Debug.Log("Deployable not implemented: " + deployable);
                                    break;
                                default:
                                    Debug.Log("Unknown deployable type: " + deployable);
                                    break;
                            }
                        },
                        minion => { Debug.Log("Minion not implemented: " + minion); },
                        throwable => { Debug.Log("Throwable not implemented: " + throwable); },
                        resource => { Debug.Log("Resource not implemented: " + resource); }
                    );
                    ecb.DestroyEntity(entity);
                }
            }
            
            if (!ecb.IsEmpty)
            {
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }
            
        }
        
    }
}