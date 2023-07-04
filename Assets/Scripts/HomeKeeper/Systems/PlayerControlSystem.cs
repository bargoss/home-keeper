using System;
using System.Net.Sockets;
using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace HomeKeeper.Systems
{
    /*
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerControlSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer();
            foreach (var playerActionRw in SystemAPI.Query<RefRW<PlayerAction>>())
            {
                var playerAction = playerActionRw.ValueRO;
                if (SystemAPI.Exists(playerAction.ItemEntityOpt))
                {
                    var item = SystemAPI.GetComponent<Item>(playerAction.ItemEntityOpt);
                    
                    var dragTargetPos = playerAction.CameraPosition + playerAction.MouseDirection * playerAction.GrabDistance;
                    var grabbedObjectPhysicsVelocityRw = SystemAPI.GetComponentRW<PhysicsVelocity>(playerAction.ItemEntityOpt);
                    var grabbedObjectLocalTransformRw = SystemAPI.GetComponentRW<LocalTransform>(playerAction.ItemEntityOpt);
                    
                    var grabbedObjectPhysicsVelocity = grabbedObjectPhysicsVelocityRw.ValueRO;
                    var grabbedObjectLocalTransform = grabbedObjectLocalTransformRw.ValueRO;
                    
                    MoveItem(
                        ref grabbedObjectPhysicsVelocity,
                        ref grabbedObjectLocalTransform,
                        dragTargetPos, quaternion.identity
                    );
                    
                    
                    // drop or insert the item into a socket
                    if (playerAction.Drop)
                    {
                        // private bool TryGetItem(float3 origin, float3 end, out Entity socketEntity, out Entity itemEntity, out Item item)
                        if (TryGetSocket(
                                playerAction.CameraPosition,
                                playerAction.MouseDirection * playerAction.GrabDistance,
                                out var itemSocketEntity,
                                out var itemSocket
                            ))
                        {
                            var socketAcceptsItem = (item.ItemType & itemSocket.AcceptedItemType) != 0;
                            var socketIsEmpty = GetItemFromSocket(itemSocketEntity, out _, out _) == false; 
                            
                            if(socketIsEmpty && socketAcceptsItem)
                            {
                                PutItemInSocket(playerAction.ItemEntityOpt, itemSocketEntity, ref entityCommandBuffer);
                            }
                        }
                        
                        playerAction.ItemEntityOpt = Entity.Null;
                    }
                    else
                    {
                        grabbedObjectPhysicsVelocityRw.ValueRW = grabbedObjectPhysicsVelocity;
                        grabbedObjectLocalTransformRw.ValueRW = grabbedObjectLocalTransform;
                    }
                }
                else
                {
                    if (playerAction.Grab)
                    {
                        // private bool TryGetSocket(float3 origin, float3 end, out Entity itemSocketEntity, out ItemSocket itemSocket)
                        if(TryGetSocket(
                            playerAction.CameraPosition,
                            playerAction.MouseDirection * playerAction.GrabDistance,
                            out var itemSocketEntity,
                            out var itemSocket
                        ))
                        {
                            if (GetItemFromSocket(itemSocketEntity, out var itemEntity, out var item))
                            {
                                playerAction.ItemEntityOpt = itemEntity;
                                playerAction.GrabDistance = math.distance(playerAction.CameraPosition, playerAction.MouseDirection);

                                throw new NotImplementedException();
                                //GrabItemFromGroundItem();
                            }
                        }
                    }
                }
                
                
                playerActionRw.ValueRW = playerAction;
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
        }

        private Entity PutItemInGroundItem(Entity itemEntity, ref EntityCommandBuffer entityCommandBuffer)
        {
            throw new NotImplementedException("dont know how to get children yet");
        }
        private void PutItemInSocket(Entity itemEntity, Entity socketEntity, ref EntityCommandBuffer entityCommandBuffer)
        {
            throw new NotImplementedException("dont know how to get children yet");
        }
        private bool GetItemFromSocket(Entity socketEntity, out Entity itemEntity, out Item item)
        {
            throw new NotImplementedException("dont know how to get children yet");
        }
        private void MoveItem(ref PhysicsVelocity physicsVelocity, ref LocalTransform worldTransform, float3 targetPosition, quaternion targetRotation)
        {
            var delta = targetPosition - worldTransform.Position;
            delta = delta.ClampMagnitude(5);
            
            physicsVelocity.Linear = delta;
            physicsVelocity.Angular = float3.zero;
            
            worldTransform.Rotation = math.slerp(worldTransform.Rotation, targetRotation, SystemAPI.Time.DeltaTime * 5);
        }

        private void GrabItemFromSocket(
                ref PlayerAction playerAction,
                Entity itemEntity,
                Entity itemSocketEntity,
                ItemSocket itemSocket,
                ref EntityCommandBuffer entityCommandBuffer
            )
        {
            if (itemSocket.TempSocket)
            {
                entityCommandBuffer.DestroyEntity(itemSocketEntity);
            }

            throw new NotImplementedException();
            //playerAction.ItemEntityOpt = item;
            playerAction.GrabDistance = math.distance(playerAction.CameraPosition, playerAction.MouseDirection);
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
        private void DropItemOnTheGround(
                ref PlayerAction playerAction,
                Entity itemEntity,
                ref EntityCommandBuffer entityCommandBuffer
        )
        {
            var itemSocketPrefab = SystemAPI.GetSingleton<GameResources>().ItemSocketPrefab;
            var itemSocketEntity = entityCommandBuffer.Instantiate(itemSocketPrefab);
            
            var dropPosition = GetDropPosition(playerAction);
            
            entityCommandBuffer.SetLocalPositionRotation(itemSocketEntity, dropPosition, quaternion.identity);
        }
        private bool TryGetSocket(float3 origin, float3 end, out Entity itemSocketEntity, out ItemSocket itemSocket)
        {
            itemSocketEntity = default;
            itemSocket = default;
            
            if (TryRaycastGetFirst(origin, end, CollisionTags.ItemSocket, out itemSocketEntity))
            {
                itemSocket = SystemAPI.GetComponent<ItemSocket>(itemSocketEntity);
                return true;
            }
            return false;
        }
        private bool TryGetItemFromItemSocket(Entity socketEntity, out Entity itemEntity, out Item item)
        {
            itemEntity = default;
            item = default;
            
            if (GetItemFromSocket(socketEntity, out itemEntity, out item))
            {
                return true;
            }
            return false;
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
    */

    public readonly partial struct ItemAspect : IAspect
    {
        public readonly Entity Self;
        
        readonly RefRO<Item> m_Item;
        [Optional] private readonly RefRW<Parent> m_Parent;
        
        
        public ItemType ItemType => m_Item.ValueRO.ItemType;

        public Entity Parent
        {
            get => m_Parent.ValueRO.Value;
            set => m_Parent.ValueRW.Value = value;
        }
    }
    public readonly partial struct ItemSocketAspect : IAspect
    {
        // An Entity field in an Aspect gives access to the Entity itself.
        // This is required for registering commands in an EntityCommandBuffer for example.
        public readonly Entity Self;
        public readonly RefRW<ItemSocket> ItemSocket;
        private readonly RefRO<LocalToWorld> LocalToWorld;
        public float3 WorldPosition => LocalToWorld.ValueRO.Position;
        // todo add the children info here somewhere
        
        private readonly ComponentLookup<Item> m_ItemLookup;

        public bool TryGetItem(out ItemAspect itemAspect)
        {
            itemAspect = default;
            var itemEntity = GetFirstChild();
            //SystemAPI.GetAspect<ItemAspect>(itemEntity);
            //var a  = SystemAPI.GetComponentLookup<Item>()[Self].ItemType;
            var a  = m_ItemLookup[Self].ItemType;
            TransformHelpers.ComputeWorldTransformMatrix();

            return false;
        }
        private Entity GetFirstChild()
        {
            // todo
            throw new NotImplementedException("dont know how to get children yet");
        }
    }
}