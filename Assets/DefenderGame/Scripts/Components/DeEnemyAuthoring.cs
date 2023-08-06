using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DeEnemyAuthoring : MonoBehaviour
    {
        public class DeEnemyBaker : Baker<DeEnemyAuthoring>
        {
            public override void Bake(DeEnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new DeEnemy());
            }
        }
    }
}