using Components;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace.Authoring
{
    public class HealthAuthoring : MonoBehaviour
    {
        public float MaxHitPoints = 10;
        public float RegenerationRate = 1;
    }
    
    public class HealthBaker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health()
            {
                HitPoints = authoring.MaxHitPoints,
                MaxHitPoints = authoring.MaxHitPoints,
                RegenerationRate = authoring.RegenerationRate
            });
        }
    }
    
}