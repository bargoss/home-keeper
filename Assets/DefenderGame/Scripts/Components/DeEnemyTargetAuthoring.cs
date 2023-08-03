using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DeEnemyTargetAuthoring : MonoBehaviour
    {
        public class DeEnemyTargetBaker : Baker<DeEnemyTargetAuthoring>
        {
            public override void Bake(DeEnemyTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DeEnemyTarget());
            }
        }
    }
}