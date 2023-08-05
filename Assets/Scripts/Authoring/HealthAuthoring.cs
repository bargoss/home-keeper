using Components;
using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace.Authoring
{
    public class HealthAuthoring : MonoBehaviour
    {
        public float MaxHitPoints = 10;
        public bool DestroyOnDeath = true;
    }
    
    public class HealthBaker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health(authoring.MaxHitPoints, authoring.DestroyOnDeath));
        }
    }
}