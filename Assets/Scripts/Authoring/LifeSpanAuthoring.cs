using Components;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace DefaultNamespace.Authoring
{
    public class LifeSpanAuthoring : MonoBehaviour
    {
        public float SecondsToLive = 1;
    }
    
    public class LifeSpanBaker : Baker<LifeSpanAuthoring>
    {
        public override void Bake(LifeSpanAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new LifeSpan()
            {
                SecondsToLive = authoring.SecondsToLive
            });
        }
    }
}