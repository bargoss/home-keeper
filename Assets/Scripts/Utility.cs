﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using ValueVariant;
using Plane = UnityEngine.Plane;
using RaycastHit = Unity.Physics.RaycastHit;

namespace DefaultNamespace
{
    public static class Utility
    {
        public static float3 Up => new float3(0, 1, 0);
        public static float3 Right => new float3(1, 0, 0);
        public static float3 Forward => new float3(0, 0, 1);
        
        public static void ControlVelocity(float3 currentVelocity, float3 targetVelocity, float maxAcceleration, float deltaTime, out float3 newVelocity)
        {
            var deltaVelocity = targetVelocity - currentVelocity;
            var clampedDeltaVelocity = deltaVelocity.ClampMagnitude(maxAcceleration * deltaTime);
            newVelocity = currentVelocity + clampedDeltaVelocity;
        }

        public static bool Raycast(this BuildPhysicsWorldData buildPhysicsWorldData, float3 start, float3 end, out RaycastHit hit)
        {
            return Raycast(buildPhysicsWorldData, start, end, 0xffffffff, out hit);
        }
        public static bool Raycast(this BuildPhysicsWorldData buildPhysicsWorldData, float3 start, float3 end, uint tag, out RaycastHit hit)
        {
            var collisionWorld = buildPhysicsWorldData.PhysicsData.PhysicsWorld.CollisionWorld;
            var collisionFilter = CollisionFilter.Default;
            collisionFilter.BelongsTo = tag;
            var raycastInput = new RaycastInput
            {
                Start = start,
                End = end,
                Filter = collisionFilter
            };
            return collisionWorld.CastRay(raycastInput, out hit);
        }

        public static bool SphereCast(this BuildPhysicsWorldData buildPhysicsWorldData, float3 start, float3 end, float radius, out ColliderCastHit hit)
        {
            return SphereCast(buildPhysicsWorldData, start, end, radius, 0xffffffff, out hit);
        }
        
        public static bool SphereCast(this BuildPhysicsWorldData buildPhysicsWorldData, float3 start, float3 end, float radius, uint tag, out ColliderCastHit hit)
        {
            var collisionWorld = buildPhysicsWorldData.PhysicsData.PhysicsWorld.CollisionWorld;
            var collisionFilter = CollisionFilter.Default;
            collisionFilter.BelongsTo = tag;
            
            var direction = end - start;
            var distance = math.length(direction);
            return collisionWorld.SphereCast(start, radius, direction, distance, out hit, collisionFilter);
        }
        
        
        public static void CopyTRS(Transform source, Transform destination)
        {
            destination.position = source.position;
            destination.rotation = source.rotation;
            destination.localScale = source.localScale;
        }

        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        
        public static bool TryGetRw<T>(this ComponentLookup<T> lookup, Entity entity, out RefRW<T> rw) where T : unmanaged, IComponentData
        {
            rw = lookup.GetRefRWOptional(entity);
            return rw.IsValid;
        }
        
        // public static bool TryGetComponent<T>(this EntityManager entityManager, Entity entity, out T componentData) where T : unmanaged, IComponentData
        // {
        //     componentData = default;
        //     if(entityManager.HasComponent<T>(entity))
        //     {
        //         componentData = entityManager.GetComponentData<T>(entity);
        //         return true;
        //     }
        //     return false;
        // }
        
// add dynamic, interpolated rigidbody
        
        public static Entity CreateBody(Entity entity, float3 position, quaternion orientation, BlobAssetReference<Unity.Physics.Collider> collider, float3 linearVelocity, float3 angularVelocity, float mass, bool isDynamic, ref EntityCommandBuffer commandBuffer)
        {

            //Entity entity = commandBuffer.CreateEntity();

            //entityManager.AddComponent(entity, new LocalToWorld {});

            //entityManager.AddComponent(entity, LocalTransform.FromPositionRotation(position, orientation));


            var colliderComponent = new PhysicsCollider { Value = collider };
            commandBuffer.AddComponent(entity, colliderComponent);

            commandBuffer.AddSharedComponent(entity, new PhysicsWorldIndex());
            

            if (isDynamic)
            {
                commandBuffer.AddComponent(entity, PhysicsMass.CreateDynamic(colliderComponent.MassProperties, mass));

                float3 angularVelocityLocal = math.mul(math.inverse(colliderComponent.MassProperties.MassDistribution.Transform.rot), angularVelocity);
                commandBuffer.AddComponent(entity, new PhysicsVelocity
                {
                    Linear = linearVelocity,
                    Angular = angularVelocityLocal
                });
                commandBuffer.AddComponent(entity, new PhysicsDamping
                {
                    Linear = 0.01f,
                    Angular = 0.05f
                });
            }

            return entity;
        }

        
        public static float3 ClampMagnitude(this float3 vector, float maxLength)
        {
            var length = math.length(vector);
            
            if (float.IsNaN(length))
            {
                return float3.zero;
            }
            
            if (length > maxLength)
            {
                return (vector / length) * maxLength;
            }
            return vector;
        }

        public static void SetVelocity(this ref EntityCommandBuffer commandBuffer, Entity entity, float3 velocity)
        {
            commandBuffer.SetComponent(entity, new PhysicsVelocity()
            {
                Linear = velocity,
                Angular = float3.zero
            });
        }
        
        public static void SetLocalPositionRotation(this ref EntityCommandBuffer commandBuffer, Entity entity, float3 position, quaternion rotation)
        {
            commandBuffer.SetComponent(entity, new LocalTransform()
            {
                Position = position,
                Rotation = rotation,
                Scale = 1
            });
            
            commandBuffer.SetComponent(entity,new LocalToWorld()
            {
                Value = float4x4.TRS(position, rotation, 1)
            });
            
        }
        public static void SetLocalPositionRotationScale(this ref EntityCommandBuffer commandBuffer, Entity entity, float3 position, quaternion rotation, float scale)
        {
            commandBuffer.SetComponent(entity, new LocalTransform()
            {
                Position = position,
                Rotation = rotation,
                Scale = scale
            });
            
            commandBuffer.SetComponent(entity,new LocalToWorld()
            {
                Value = float4x4.TRS(position, rotation, scale)
            });
            
        }

        public static float3 GoTowardsWithClampedMag(float3 start, float3 target, float maxMovement)
        {
            var direction = target - start;
            var distance = math.length(direction);
            var normalizedDirection = math.normalize(direction);
            var movement = math.clamp(distance, 0, maxMovement);
            return start + normalizedDirection * movement;
        }
        
        public static float3 GetInputDirection()
        {
            var horizontal = UnityEngine.Input.GetAxisRaw("Horizontal");
            var vertical = UnityEngine.Input.GetAxisRaw("Vertical");
            
            return new float3(horizontal, 0, vertical);
        }
        
        public static float3 GetMousePositionInWorldSpaceXY()
        {
            var mouseRayIntoWorld = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.forward, Vector3.zero);
            if (plane.Raycast(mouseRayIntoWorld, out var distance))
            {
                var mousePositionInWorldSpace = mouseRayIntoWorld.GetPoint(distance);
                return mousePositionInWorldSpace;
            }

            return Vector3.zero;
        }
        public static float3 GetMousePositionInWorldSpaceXZ()
        {
            var mouseRayIntoWorld = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(mouseRayIntoWorld, out var distance))
            {
                var mousePositionInWorldSpace = mouseRayIntoWorld.GetPoint(distance);
                return mousePositionInWorldSpace;
            }

            return Vector3.zero;
        }
        
        private static bool TryRaycastGetFirst(this BuildPhysicsWorldData buildPhysicsWorldData, float3 origin, float3 end, CollisionFilter collisionFilter, out Entity entity)
        {
            entity = Entity.Null;
            var collisionWorld = buildPhysicsWorldData.PhysicsData.PhysicsWorld.CollisionWorld;
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

        private static bool TryRaycastGetDistance(this BuildPhysicsWorldData buildPhysicsWorldData, float3 origin, float3 end, CollisionFilter collisionFilter, out float hitDistance)
        {
            hitDistance = 0;
            var raycastInput = new RaycastInput
            {
                Start = origin,
                End = end,
                Filter = collisionFilter
            };
            var collisionWorld = buildPhysicsWorldData.PhysicsData.PhysicsWorld.CollisionWorld;
            if (collisionWorld.CastRay(raycastInput, out var hit))
            {
                hitDistance = hit.Fraction;
                return true;
            }
            
            return false;
        }

        public static void TranslateLEG(DynamicBuffer<LinkedEntityGroup> leg, float4x4 translation, ref ComponentLookup<LocalToWorld> localToWorldLookup,ref EntityCommandBuffer entityCommandBuffer)
        {
            foreach (var e in leg)
            {
                var entity = e.Value;
                var localToWorld = localToWorldLookup.GetRefRO(entity).ValueRO;
                var newTransform = LocalTransform.FromMatrix(math.mul(translation, localToWorld.Value));
                entityCommandBuffer.SetComponent(entity, newTransform);
                entityCommandBuffer.SetComponent(entity, new LocalToWorld()
                {
                    Value = float4x4.TRS(newTransform.Position, newTransform.Rotation, newTransform.Scale)
                });
                
                //if (localTransformLookup.GetRefRWOptional(entity) is var localTransformRwOptional && localTransformRwOptional.IsValid)
                //{
                    //var localTransform = localTransformRwOptional.ValueRO;
                    //var currentTransform = float4x4.TRS(localTransform.Position, localTransform.Rotation, localTransform.Scale);
                    //var newTransform = math.mul(translation, currentTransform);
                    //localTransformRwOptional.ValueRW = new LocalTransform()
                    //{
                    //    Position = newTransform.c3.xyz,
                    //    Rotation = quaternion.LookRotationSafe(newTransform.c2.xyz, newTransform.c1.xyz),
                    //    Scale = newTransform.c0.x
                    //};
                //}
            }
        }
    }
}