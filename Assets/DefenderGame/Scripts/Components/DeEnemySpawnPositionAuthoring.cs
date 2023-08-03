using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DeEnemySpawnPositionAuthoring : MonoBehaviour
    {
        public class DeEnemySpawnPositionBaker : Baker<DeEnemySpawnPositionAuthoring>
        {
            public override void Bake(DeEnemySpawnPositionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DeEnemySpawnPosition());
            }
        }
    }
}