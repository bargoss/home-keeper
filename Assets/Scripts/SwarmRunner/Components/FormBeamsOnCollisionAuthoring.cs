using Unity.Entities;
using UnityEngine;

namespace SwarmRunner.Components
{
    public class FormBeamsOnCollisionAuthoring : MonoBehaviour 
    {
        class Baker : Baker<FormBeamsOnCollisionAuthoring>
        {
            public override void Bake(FormBeamsOnCollisionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FormBeamsOnCollision());
            }
        }
    }
    
    public struct FormBeamsOnCollision : IComponentData
    {
        
    }
}