using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SwarmRunner.Components
{
    public class SwarmUnitAuthoring : MonoBehaviour
    {
        
        class Baker : Baker<SwarmUnitAuthoring>
        {
            public override void Bake(SwarmUnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SwarmUnit()
                {
                    Normal = new float3(0,1,1),
                });
            }
        }
    }
    
    public struct SwarmUnit : IComponentData
    {
        public float3 Normal;
    }
}