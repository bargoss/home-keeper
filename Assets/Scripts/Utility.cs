﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Plane = UnityEngine.Plane;

namespace DefaultNamespace
{
    public static class Utility
    {
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
            if (length > maxLength)
            {
                return vector / length * maxLength;
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
                Value = float4x4.TRS(position, rotation, 1)
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
        
        public static float3 GetMousePositionInWorldSpace()
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