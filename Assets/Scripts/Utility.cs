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
    }
}