using Unity.Entities;
using UnityEngine;

namespace SwarmRunner.Components
{
    public class BrakeBeamsOnCollisionAuthoring : MonoBehaviour
    {
        class Baker : Baker<BrakeBeamsOnCollisionAuthoring>
        {
            public override void Bake(BrakeBeamsOnCollisionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BrakeBeamsOnCollision());
            }
        }
    }
    public struct BrakeBeamsOnCollision : IComponentData
    {
        
    }
}