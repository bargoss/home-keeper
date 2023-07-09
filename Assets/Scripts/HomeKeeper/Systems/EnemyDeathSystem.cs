using Components;
using DefaultNamespace;
using HomeKeeper.Components;
using Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;
using UnityEngine.Rendering;

namespace HomeKeeper.Systems
{
    /*
     * When projectile hits enemy it makes blood effect,
     * If it kills the enemy, its dead entity is created
     */
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(HealthSystem))]
    [UpdateAfter(typeof(DamageOnCollisionSystem))]
    public partial struct EnemyDeathSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var gameResources = SystemAPI.GetSingleton<GameResourcesUnmanaged>();
            
            foreach (var (enemy, health, localToWorld, physicsVelocity, entity) in SystemAPI.Query<Enemy, Health, LocalToWorld, PhysicsVelocity>().WithEntityAccess())
            {
                if (health.IsDead)
                {
                    commandBuffer.DestroyEntity(entity);

                    var dyingEnemyPrefab = gameResources.DyingEnemyPrefab;
                    var dyingEnemy = commandBuffer.Instantiate(dyingEnemyPrefab);
                    commandBuffer.SetLocalPositionRotation(dyingEnemy, localToWorld.Position, localToWorld.Rotation);
                    commandBuffer.SetComponent(dyingEnemy, physicsVelocity);
                }                
            }
            
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}