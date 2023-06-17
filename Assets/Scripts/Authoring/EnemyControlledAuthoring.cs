using Components;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace.Authoring
{
    public class EnemyControlledAuthoring : MonoBehaviour { }
    
    public class EnemyControlledBaker : Baker<EnemyControlledAuthoring>
    {
        public override void Bake(EnemyControlledAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyControlled());
        }
    }
}