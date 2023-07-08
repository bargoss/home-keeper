using Components;
using DefaultNamespace;
using HomeKeeper.Components;
using Systems;
using Unity.Collections;
using Unity.Entities;
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
    public partial struct ProjectileEnemyHitSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (projectile, statefulCollisionEvents, entity) in SystemAPI.Query<Projectile, DynamicBuffer<StatefulCollisionEvent>>().WithEntityAccess())
            {
                foreach (var statefulCollisionEvent in statefulCollisionEvents)
                {
                    var otherEntity = statefulCollisionEvent.GetOtherEntity(entity);
                    if (
                        SystemAPI.GetComponentLookup<Health>().TryGetComponent(otherEntity, out var health) &&
                        SystemAPI.GetComponentLookup<LocalToWorld>().TryGetComponent(otherEntity, out var localToWorld)
                    )
                    {
                        var bloodEffect = commandBuffer.CreateEntity();
                        commandBuffer.SetLocalPositionRotation(bloodEffect, localToWorld.Position, localToWorld.Rotation);

                        if (health.IsDead)
                        {
                            commandBuffer.DestroyEntity(otherEntity);
                            
                            var dyingEnemy = commandBuffer.Instantiate(SystemAPI.GetSingleton<GameResourcesUnmanaged>().DyingEnemyPrefab);
                            commandBuffer.SetLocalPositionRotation(dyingEnemy, localToWorld.Position, localToWorld.Rotation);
                        }
                    }
                }
            }
            
            commandBuffer.Playback(state.EntityManager);
        }
    }

    public partial class MSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            

        }
    }
}