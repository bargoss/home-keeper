using System;
using System.Net.Sockets;
using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [RequireMatchingQueriesForUpdate]
    public partial struct PlayerControlSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var gameResources = SystemAPI.GetSingleton<GameResourcesUnmanaged>();
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var playerActionRw in SystemAPI.Query<RefRW<PlayerAction>>())
            {
                var playerAction = playerActionRw.ValueRO;

                var origin = playerAction.CameraPosition;
                var dropPosition = GetDropPosition(playerAction);
                var targetPosition = playerAction.CameraPosition + playerAction.MouseDirection * playerAction.GrabDistance;
                Debug.DrawLine(origin, dropPosition);

                if (playerAction.HoldsItem)
                {
                    var item = playerAction.HeldItem;
                    
                    // drop or insert the item into a socket
                    if (playerAction.Drop)
                    {
                        // dropping to existing socket
                        if (TryGetItemSocket(origin, targetPosition, out var itemSocketEntity))
                        {
                            var itemSocketAspect = SystemAPI.GetAspect<ItemSocketAspect>(itemSocketEntity);
                            if (itemSocketAspect.TryGetItem(out _) == false)
                            {
                                entityCommandBuffer.AddComponent<Item>(itemSocketEntity, item);
                            }
                        }
                        // dropping on the ground
                        else
                        {
                            var freeItemSocket = entityCommandBuffer.Instantiate(gameResources.FreeItemSocketPrefab);
                            entityCommandBuffer.SetLocalPositionRotation(freeItemSocket, dropPosition, quaternion.identity);
                            entityCommandBuffer.AddComponent(freeItemSocket, item);
                        }
                        
                        playerAction.HoldsItem = false;
                    }
                }
                else
                {
                    if (playerAction.Grab)
                    {
                        if (TryGetItemSocket(origin, targetPosition, out var itemSocketEntity))
                        {
                            var itemSocketAspect = SystemAPI.GetAspect<ItemSocketAspect>(itemSocketEntity);
                            if (itemSocketAspect.TryGetItem(out var item))
                            {
                                playerAction.HoldsItem = true;
                                playerAction.HeldItem = item;
                                entityCommandBuffer.RemoveComponent<Item>(itemSocketEntity);

                                if (itemSocketAspect.DestroyedIfEmpty)
                                {
                                    entityCommandBuffer.DestroyEntity(itemSocketEntity);
                                }
                            }
                        }
                    }
                }
                
                playerActionRw.ValueRW = playerAction;
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
        }
        
        private float3 GetDropPosition(PlayerAction playerAction)
        {
            var dropPosition = playerAction.CameraPosition + playerAction.MouseDirection * playerAction.GrabDistance;
            if(TryRaycastGetDistance(playerAction.CameraPosition, dropPosition, CollisionTags.Default,out var distance))
            {
                dropPosition = playerAction.CameraPosition + playerAction.MouseDirection * (distance - 0.5f);
            }

            return dropPosition;
        }
        
        private bool TryGetItemSocket(float3 origin, float3 end, out Entity itemSocketEntity)
        {
            itemSocketEntity = default;
            return TryRaycastGetFirst(origin, end, CollisionTags.ItemSocket, out itemSocketEntity);
        }
        
        private bool TryRaycastGetFirst(float3 origin, float3 end, uint tag, out Entity entity)
        {
            entity = Entity.Null;
            var collisionWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
            var collisionFilter = CollisionFilter.Default;
            collisionFilter.BelongsTo = tag;
            var raycastInput = new RaycastInput
            {
                Start = origin,
                End = end,
                Filter = collisionFilter
            };
            if (collisionWorld.CastRay(raycastInput, out var hit))
            {
                entity = hit.Entity;
                return true;
            }

            return false;
        }

        private bool TryRaycastGetDistance(float3 origin, float3 end, uint tag, out float hitDistance)
        {
            hitDistance = 0;
            
            var collisionWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
            var collisionFilter = CollisionFilter.Default;
            collisionFilter.BelongsTo = tag;
            var raycastInput = new RaycastInput
            {
                Start = origin,
                End = end,
                Filter = collisionFilter
            };
            if (collisionWorld.CastRay(raycastInput, out var hit))
            {
                hitDistance = math.distance(origin, hit.Position);
                return true;
            }

            return false;
            
        }
    }
}