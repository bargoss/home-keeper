using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DeGameDataAuthoring : MonoBehaviour
    {
        public float EnemySpawnRate = 0.5f;
        public class DeGameDataBaker : Baker<DeGameDataAuthoring>
        {
            public override void Bake(DeGameDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new DeGameData()
                {
                    EnemySpawnRate = authoring.EnemySpawnRate,
                    PlayerInput = new PlayerInput(),
                    LastEnemySpawnTime = 0
                });
            }
        }
    }
}