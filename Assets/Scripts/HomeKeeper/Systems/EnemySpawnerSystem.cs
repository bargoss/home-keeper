using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemySpawnerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (enemySpawnerRw, localToWorld, spawnerEntity)
                     in SystemAPI.Query<RefRW<EnemySpawner>, LocalToWorld>().WithEntityAccess())
            {
                var enemySpawner = enemySpawnerRw.ValueRO;

                if (SystemAPI.Time.ElapsedTime > enemySpawner.LastSpawnTime + enemySpawner.SpawnInterval)
                {
                    enemySpawner.LastSpawnTime = (float)SystemAPI.Time.ElapsedTime;
                    var seed = (uint)SystemAPI.Time.ElapsedTime + (uint)spawnerEntity.Index * 7;
                    var offset = new float3(-2.5f,0,0) + new float3(5,0,0) * ((float)SystemAPI.Time.ElapsedTime % 1);
                    
                    var spawnPosition = localToWorld.Position + offset;

                    var enemy = commandBuffer.Instantiate(SystemAPI.GetSingleton<GameResourcesUnmanaged>().EnemyPrefab);
                    //commandBuffer.SetLocalPositionRotation(enemy, spawnPosition, quaternion.identity);
                    commandBuffer.SetLocalPositionRotationScale(enemy, spawnPosition, quaternion.identity, 1);
                    

                    enemySpawnerRw.ValueRW = enemySpawner;
                }
            }
            
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }

        private static float3 CalculateEnemySpawnPosition(EnemySpawner enemySpawner, float3 spawnerPosition, uint seed)
        {
            var random = Random.CreateFromIndex(seed);
            var angle = random.NextFloat() % enemySpawner.SpawnArcDegrees - enemySpawner.SpawnArcDegrees / 2.0f;
            var radius = enemySpawner.SpawnInnerRadius + random.NextFloat() %
                (enemySpawner.SpawnOuterRadius - enemySpawner.SpawnInnerRadius);

            var spawnOffset = math.mul(
                quaternion.Euler(new float3(0, 0, math.radians(angle))),
                enemySpawner.SpawnDirection * radius
            );

            return spawnerPosition + spawnOffset;
        }
    }
}