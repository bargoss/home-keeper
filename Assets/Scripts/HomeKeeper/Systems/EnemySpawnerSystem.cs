using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemySpawnerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer();

            foreach (var (enemySpawnerRw, localToWorld, spawnerEntity)
                     in SystemAPI.Query<RefRW<EnemySpawner>, LocalToWorld>().WithEntityAccess())
            {
                var enemySpawner = enemySpawnerRw.ValueRO;

                if (SystemAPI.Time.ElapsedTime > enemySpawner.LastSpawnTime + enemySpawner.SpawnInterval)
                {
                    enemySpawner.LastSpawnTime = (float)SystemAPI.Time.ElapsedTime;
                    var seed = (uint)SystemAPI.Time.ElapsedTime + (uint)spawnerEntity.Index * 7;
                    var spawnPosition = CalculateEnemySpawnPosition(enemySpawner, localToWorld.Position, seed);
                    var enemy = commandBuffer.Instantiate(SystemAPI.GetSingleton<GameResourcesUnmanaged>().EnemyPrefab);
                    commandBuffer.SetLocalPositionRotation(enemy, spawnPosition, quaternion.identity);

                    enemySpawnerRw.ValueRW = enemySpawner;
                }
            }
            
            commandBuffer.Playback(state.EntityManager);
        }

        private static float3 CalculateEnemySpawnPosition(EnemySpawner enemySpawner, float3 spawnerPosition, uint seed)
        {
            var random = Random.CreateFromIndex(seed);
            var angle = random.NextFloat() % enemySpawner.SpawnArcDegrees - enemySpawner.SpawnArcDegrees / 2.0f;
            var radius = enemySpawner.SpawnInnerRadius + random.NextFloat() %
                (enemySpawner.SpawnOuterRadius - enemySpawner.SpawnInnerRadius);

            var spawnOffset = math.mul(
                quaternion.Euler(new float3(0, 0, angle)),
                enemySpawner.SpawnDirection * radius
            );

            return spawnerPosition + spawnOffset;
        }
    }
}