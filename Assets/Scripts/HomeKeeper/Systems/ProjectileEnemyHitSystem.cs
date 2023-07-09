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
    public partial struct ProjectileEnemyHitSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var localToWorldLookUp = SystemAPI.GetComponentLookup<LocalToWorld>();
            
            var gameResources = SystemAPI.GetSingleton<GameResourcesUnmanaged>();
            
            foreach (var (projectile, statefulCollisionEvents, entity) in SystemAPI.Query<Projectile, DynamicBuffer<StatefulCollisionEvent>>().WithEntityAccess())
            {
                foreach (var statefulCollisionEvent in statefulCollisionEvents)
                {
                    var otherEntity = statefulCollisionEvent.GetOtherEntity(entity);
                    if (
                        SystemAPI.GetComponentLookup<Health>().TryGetComponent(otherEntity, out var health) &&
                        localToWorldLookUp.TryGetComponent(otherEntity, out var localToWorld)
                    )
                    {
                        if (health.IsDead)
                        {
                            commandBuffer.DestroyEntity(otherEntity);

                            var dyingEnemyPrefab = gameResources.DyingEnemyPrefab;
                            var dyingEnemy = commandBuffer.Instantiate(dyingEnemyPrefab);
                            commandBuffer.SetLocalPositionRotation(dyingEnemy, localToWorld.Position, localToWorld.Rotation);
                            
                            //commandBuffer.SetLocalPositionRotation(dyingEnemy, localToWorld.Position, localToWorld.Rotation);
                            // public static void TranslateLEG(DynamicBuffer<LinkedEntityGroup> leg, float4x4 translation, ref ComponentLookup<LocalToWorld> localToWorldLookup,ref EntityCommandBuffer entityCommandBuffer)
                            //var leg = SystemAPI.GetBuffer<LinkedEntityGroup>(dyingEnemy);
                            //Utility.TranslateLEG(leg, localToWorld.Value, ref localToWorldLookUp, ref commandBuffer);
                        }
                    }
                }
            }
            
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}