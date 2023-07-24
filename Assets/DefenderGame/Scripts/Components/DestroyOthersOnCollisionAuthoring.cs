using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DestroyOthersOnCollisionAuthoring : MonoBehaviour
    {
        public class DestroyOthersOnCollisionBaker : Baker<DestroyOthersOnCollisionAuthoring>
        {
            public override void Bake(DestroyOthersOnCollisionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DestroyOthersOnCollision());
                
            }
        }
    }
    public struct DestroyOthersOnCollision : IComponentData
    {
        
    }
}