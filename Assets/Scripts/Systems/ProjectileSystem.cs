using System.Linq;
using Components;
using HomeKeeper.Components;
using HomeKeeper.Systems;
using Unity.Entities;
using Unity.Physics.Stateful;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ProjectileSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            Entity e;
            
            
            foreach (var (statefulCollisionEvents, projectile, entity) in SystemAPI.Query<DynamicBuffer<StatefulCollisionEvent>, Projectile>().WithEntityAccess())
            {
                foreach (var collision in statefulCollisionEvents)
                {
                    var other = collision.GetOtherEntity(entity);
                    var healthRw = SystemAPI.GetComponentLookup<Health>().GetRefRWOptional(other);
                    
                    if(healthRw.IsValid)
                    {
                        if (!collision.CollisionDetails.IsValid)
                        {
                            Debug.LogError("Invalid collision details, adjust the component so this is calculated");
                        }
                        
                        //var damage = projectile.BaseDamage * collision.CollisionDetails.EstimatedImpulse;
                        var damage = 1 * collision.CollisionDetails.EstimatedImpulse;
                        
                        var health = healthRw.ValueRO;
                        health.HitPoints -= damage;
                        healthRw.ValueRW = health;
                    }
                }
            }
        }
    }
}