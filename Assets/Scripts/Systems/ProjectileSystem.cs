using System.Linq;
using Components;
using HomeKeeper.Components;
using HomeKeeper.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(HealthSystem))]
    public partial struct ProjectileSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Projectile>();
        }
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (statefulCollisionEvents, projectile, projectilePhysicsVelocity, localTransform, entity) in SystemAPI.Query<DynamicBuffer<StatefulCollisionEvent>, Projectile, PhysicsVelocity, LocalTransform>().WithAll<Simulate>().WithEntityAccess())
            {
                foreach (var collision in statefulCollisionEvents)
                {
                    var other = collision.GetOtherEntity(entity);
                    var healthRwOpt = SystemAPI.GetComponentLookup<Health>().GetRefRWOptional(other);
                    var physicsVelocityRwOpt = SystemAPI.GetComponentLookup<PhysicsVelocity>().GetRefRWOptional(other);
                    
                    
                    if(healthRwOpt.IsValid)
                    {
                        if (!collision.CollisionDetails.IsValid)
                        {
                            Debug.LogError("Invalid collision details, adjust the component so this is calculated");
                        }
                        
                        //var damage = projectile.BaseDamage * collision.CollisionDetails.EstimatedImpulse;
                        var damage = projectile.BaseDamage; // * collision.CollisionDetails.EstimatedImpulse;
                        
                        var health = healthRwOpt.ValueRO;
                        health.HandleDamage(damage, collision.CollisionDetails.AverageContactPointPosition, collision.Normal);
                        healthRwOpt.ValueRW = health;
                        
                        if(physicsVelocityRwOpt.IsValid)
                        {
                            var physicsVelocity = physicsVelocityRwOpt.ValueRO;
                            physicsVelocity.Linear += projectilePhysicsVelocity.Linear * 0.25f;
                            physicsVelocityRwOpt.ValueRW = physicsVelocity;
                        }
                    }
                }
            }
        }
        
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}