using System.Numerics;
using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ShooterSystem: ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer();
            foreach (var (shooterRw, localToWorld, localTransformRw,entity) in SystemAPI.Query<RefRW<Shooter>, LocalToWorld, RefRW<LocalTransform>>().WithEntityAccess())
            {
                var shooter = shooterRw.ValueRO;
                var localTransform = localTransformRw.ValueRO;

                
                
                // look
                var lookInput = shooter.LookInput;
                if (math.lengthsq(lookInput) < 0.0001f) // if its zero, make it forward
                {
                    lookInput = localToWorld.Forward;
                }
                var targetRotation = quaternion.LookRotationSafe(
                    shooter.LookInput + localToWorld.Forward * 0.001f,
                    new float3(0, 1, 0)
                );
                localTransform.Rotation = math.slerp(localTransform.Rotation, targetRotation, 1.0f * SystemAPI.Time.DeltaTime);
                
                // shoot
                if (shooter.ShootInput)
                {
                    var cooldown = 1.0f / shooter.Stats.FireRate;
                    if ((float)SystemAPI.Time.ElapsedTime > shooter.LastShotTime + cooldown)
                    {
                        var shootPosition = SystemAPI.GetComponent<LocalToWorld>(shooter.Stats.ShootPositionEntity).Position;
                        var shootVelocity = localToWorld.Forward * shooter.Stats.MuzzleVelocity; 
                        CreateProjectile(
                            shootPosition,
                            shootVelocity,
                            SystemAPI.GetSingleton<GameResources>().ProjectilePrefab,
                            ref commandBuffer
                        );
                        shooter.LastShotTime = (float)SystemAPI.Time.ElapsedTime;
                    }
                }
                
                
                shooterRw.ValueRW = shooter;
                localTransformRw.ValueRW = localTransform;
            }
            
            commandBuffer.Playback(state.EntityManager);
        }

        private static Entity CreateProjectile(float3 position, float3 velocity, Entity prefab, ref EntityCommandBuffer entityCommandBuffer)
        {
            var projectile = entityCommandBuffer.Instantiate(prefab);
            
            
            entityCommandBuffer.SetComponent(projectile, new PhysicsVelocity()
            {
                Linear =  velocity,
                Angular = float3.zero
            });
            var newRotation = quaternion.LookRotationSafe(math.normalizesafe(velocity), new float3(0,1,0));
            
            entityCommandBuffer.SetComponent(projectile, new LocalTransform()
            {
                Position = position,
                Rotation = newRotation
            });
            
            entityCommandBuffer.SetComponent(projectile, new LocalToWorld()
            {
                Value = float4x4.TRS(position, newRotation, new float3(1,1,1))
            });

            return projectile;
        }
    }

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
                    var enemy = commandBuffer.Instantiate(SystemAPI.GetSingleton<GameResources>().EnemyPrefab);
                    commandBuffer.SetLocalPositionRotation(spawnPosition, quaternion.identity, enemy);

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