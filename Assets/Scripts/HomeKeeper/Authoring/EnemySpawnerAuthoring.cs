using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace HomeKeeper.Authoring
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        public float SpawnInterval = 1;
        public float SpawnInnerRadius = 30;
        public float SpawnOuterRadius = 50;
        public float3 SpawnDirection = new float3(0,1,0);
        public float SpawnArcDegrees = 120;
    }
    
    public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemySpawner()
            {
                SpawnInterval = authoring.SpawnInterval,
                LastSpawnTime = 0,
                SpawnInnerRadius = authoring.SpawnInnerRadius,
                SpawnOuterRadius = authoring.SpawnOuterRadius,
                SpawnDirection = authoring.SpawnDirection,
                SpawnArcDegrees = authoring.SpawnArcDegrees
            });
        }
    }
}