using Unity.Entities;
using Unity.Mathematics;

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
    }
}