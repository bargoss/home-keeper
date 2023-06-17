using System.Linq;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(BuildPhysicsWorld))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct DamageOnCollisionSystem : ISystem
    {
        private NativeList<DistanceHit> m_Hits;
        private CollisionWorld GetCollisionWorld()
        {
            return SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
        }
        
        public void OnCreate(ref SystemState state)
        {
            m_Hits = new NativeList<DistanceHit>(Allocator.Persistent);
        }
        public void OnDestroy(ref SystemState state)
        {
            m_Hits.Dispose();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = 0.02f;
            
            var collisionWorld = GetCollisionWorld();
            
            m_Hits.Clear();
            
            
            collisionWorld.OverlapSphere(new float3(0, 0, 0), 1, ref m_Hits, CollisionFilter.Default);
            foreach (var valueTuple in SystemAPI.Query<RefRO<DamageOnCollision>, RefRO<Faction>, RefRO<LocalToWorld>>().WithEntityAccess())
            {
                var damageRate = valueTuple.Item1.ValueRO.DamageRate;
                var knockbackForce = valueTuple.Item1.ValueRO.KnockbackForce;
                var faction = valueTuple.Item2.ValueRO.Value;
                var position = valueTuple.Item3.ValueRO.Position;
                var entityA = new Entity(); //valueTuple.Item4;
                
                collisionWorld.OverlapSphere(position, 2, ref m_Hits, CollisionFilter.Default);
                foreach (var distanceHit in m_Hits)
                {
                    if(distanceHit.Entity == entityA) continue;
                    
                    var hitEntityPosition = distanceHit.Position;
                    var hitEntity = distanceHit.Entity;
                    var hitEntityVelocityComponent = state.World.EntityManager.GetComponentData<PhysicsVelocity>(hitEntity);
                    
                    var knockbackDirection = math.normalize(hitEntityPosition - position);
                    var knockbackVelocity = knockbackDirection * knockbackForce;
                    hitEntityVelocityComponent.Linear += knockbackVelocity;
                    state.World.EntityManager.SetComponentData(hitEntity, hitEntityVelocityComponent);
                    
                    if (state.World.EntityManager.HasComponent<Health>(hitEntity))
                    {
                        var healthComponent = state.World.EntityManager.GetComponentData<Health>(hitEntity);
                        if (state.World.EntityManager.HasComponent<Faction>(hitEntity))
                        {
                            var hitEntityFaction = state.World.EntityManager.GetComponentData<Faction>(hitEntity);
                            if (hitEntityFaction.Value != faction)
                            {
                                healthComponent.HitPoints -= damageRate;
                                state.World.EntityManager.SetComponentData(hitEntity, healthComponent);
                            }
                        }
                        else
                        {
                            healthComponent.HitPoints -= damageRate;
                            state.World.EntityManager.SetComponentData(hitEntity, healthComponent);
                        }
                    }
                }
                
                
            }
        }
    }
}