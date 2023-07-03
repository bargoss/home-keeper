using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerControlSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var playerActionRw in SystemAPI.Query<RefRW<PlayerAction>>())
            {
                var playerAction = playerActionRw.ValueRO;
                if (SystemAPI.Exists(playerAction.ItemOpt))
                {
                    var dragTargetPos = playerAction.CameraPosition + playerAction.MouseDirection * playerAction.GrabDistance;
                    var grabbedObjectPhysicsVelocityRw = SystemAPI.GetComponentRW<PhysicsVelocity>(playerAction.ItemOpt);
                    var grabbedObjectLocalTransformRw = SystemAPI.GetComponentRW<LocalTransform>(playerAction.ItemOpt);
                    
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
                        if (TryGetFirstItemSocket(
                                playerAction.CameraPosition,
                                playerAction.MouseDirection * playerAction.GrabDistance,
                                out var itemSocketEntity
                            ))
                        {
                            var item = SystemAPI.GetComponent<Item>(playerAction.ItemOpt);
                            var itemSocketRw = SystemAPI.GetComponentRW<ItemSocket>(itemSocketEntity);
                            var itemSocket = itemSocketRw.ValueRO;
                            if (SystemAPI.Exists(itemSocket.HeldItemOpt) == false)
                            {
                                if ((item.ItemType & itemSocket.AcceptedItemType) != 0)
                                {
                                    itemSocket.HeldItemOpt = playerAction.ItemOpt;
                                    playerAction.ItemOpt = Entity.Null;
                                }
                            }
                            itemSocketRw.ValueRW = itemSocket;
                        }
                        
                        playerAction.ItemOpt = Entity.Null;
                    }
                    
                    
                    grabbedObjectPhysicsVelocityRw.ValueRW = grabbedObjectPhysicsVelocity;
                    grabbedObjectLocalTransformRw.ValueRW = grabbedObjectLocalTransform;
                }
                else
                {
                    if (playerAction.Grab)
                    {
                        if (TryGetFirstItem(playerAction.CameraPosition, playerAction.MouseDirection, out var item))
                        {
                            GrabItem(ref playerAction, item);
                        }
                    }
                }
                
                
                playerActionRw.ValueRW = playerAction;
            }
        }

        private void MoveItem(ref PhysicsVelocity physicsVelocity, ref LocalTransform worldTransform, float3 targetPosition, quaternion targetRotation)
        {
            var delta = targetPosition - worldTransform.Position;
            delta = delta.ClampMagnitude(5);
            
            physicsVelocity.Linear = delta;
            physicsVelocity.Angular = float3.zero;
            
            worldTransform.Rotation = math.slerp(worldTransform.Rotation, targetRotation, SystemAPI.Time.DeltaTime * 5);
        }

        private void GrabItem(ref PlayerAction playerAction, Entity item)
        {
            playerAction.ItemOpt = item;
            playerAction.GrabDistance = math.distance(playerAction.CameraPosition, playerAction.MouseDirection);
        }
        private bool TryGetFirstItem(float3 origin, float3 end, out Entity grabItem)
        {
            grabItem = Entity.Null;
            var collisionWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
            var collisionFilter = CollisionFilter.Default;
            collisionFilter.BelongsTo = CollisionTags.Item;
            var raycastInput = new RaycastInput
            {
                Start = origin,
                End = end,
                Filter = collisionFilter
            };
            if (collisionWorld.CastRay(raycastInput, out var hit))
            {
                grabItem = hit.Entity;
                return true;
            }

            return false;
        }
        private bool TryGetFirstItemSocket(float3 origin, float3 end, out Entity itemSocket)
        {
            itemSocket = Entity.Null;
            var collisionWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
            var collisionFilter = CollisionFilter.Default;
            collisionFilter.BelongsTo = CollisionTags.ItemSocket;
            var raycastInput = new RaycastInput
            {
                Start = origin,
                End = end,
                Filter = collisionFilter
            };
            if (collisionWorld.CastRay(raycastInput, out var hit))
            {
                itemSocket = hit.Entity;
                return true;
            }

            return false;
        }
    }
}