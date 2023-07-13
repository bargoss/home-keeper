using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using WaterGame.Components;
using WaterGame.Systems;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SpacialPartitioningSystem))]
    public partial struct ParticleCollisionSolverSystem : ISystem
    {
        NativeList<Entity> m_Neighbours;
        NativeHashMap<Entity, float3> m_VelocityCache;

        public void OnCreate(ref SystemState state)
        {
            m_Neighbours = new NativeList<Entity>(Allocator.Persistent);
            m_VelocityCache = new NativeHashMap<Entity, float3>(0, Allocator.Persistent);
        }
        public void OnDestroy(ref SystemState state)
        {
            m_Neighbours.Dispose();
            m_VelocityCache.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            var partitioning = SystemAPI.GetSingletonRW<SpacialPartitioningSingleton>().ValueRO.Partitioning;
            var config = SystemAPI.GetSingleton<WaterGameConfig>();
            //const float outerRadius = 3f;
            //const float innerRadius = 2.5f;

            var physicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>();
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            
            m_VelocityCache.Clear();
            foreach (var (physicsVelocity, localToWorld, particle, entity) in SystemAPI.Query<PhysicsVelocity, LocalToWorld, Particle>().WithEntityAccess())
            {
                m_VelocityCache.TryAdd(entity, physicsVelocity.Linear);
            }
            


            foreach (var (physicsVelocityRw, localToWorld, particle, entity) in SystemAPI.Query<RefRW<PhysicsVelocity>, LocalToWorld, Particle>().WithEntityAccess())
            {
                var physicsVelocityLinear = m_VelocityCache[entity];
                // public void OverlapCircle(float3 center, float radius, ref NativeList<T> buffer)
                
                m_Neighbours.Clear();
                partitioning.OverlapCircle(localToWorld.Position, config.OuterRadius, ref m_Neighbours);
                
                foreach (var neighbour in m_Neighbours)
                {
                    if (neighbour == entity) continue;

                    if (
                        localToWorldLookup.TryGetComponent(neighbour, out var neighbourLocalToWorld) &&
                        m_VelocityCache.TryGetValue(neighbour, out var neighbourPhysicsVelocityLinear)
                    )
                    {
                        var force = CalculateCollisionForce(
                            new ForcePoint()
                            {
                                Position = localToWorld.Position,
                                Velocity = physicsVelocityLinear,
                                OuterRadius = config.OuterRadius,
                                InnerRadius = config.InnerRadius
                            },
                            new ForcePoint()
                            {
                                Position = neighbourLocalToWorld.Position,
                                Velocity = neighbourPhysicsVelocityLinear,
                                OuterRadius = config.OuterRadius,
                                InnerRadius = config.InnerRadius
                            },
                            SystemAPI.Time.DeltaTime,
                            config.PushForce,
                            config.Viscosity
                        );
                        physicsVelocityLinear += force;
                    }
                }

                var physicsVelocity = physicsVelocityRw.ValueRO;
                physicsVelocity.Linear = physicsVelocityLinear;
                physicsVelocityRw.ValueRW = physicsVelocity;
            }
        }
        
        public struct ForcePoint
        {
            public float3 Position;
            public float3 Velocity;
            public float OuterRadius;
            public float InnerRadius;
        }

        public static float3 CalculateCollisionForce(
            ForcePoint a, ForcePoint b,
            float deltaTime, float pushMultiplier, float viscosity
        )
        {
            var radiusSum = a.OuterRadius + b.OuterRadius;
            var targetDistance = a.InnerRadius + b.InnerRadius;

            var delta = a.Position - b.Position;
            //var sqrMag = delta.sqrMagnitude;
            var sqrMag = math.lengthsq(delta);
            if (sqrMag < radiusSum * radiusSum)
            {
                //var magnitude = delta.magnitude;
                var magnitude = math.length(delta);
                var error = targetDistance - magnitude;
                //var dir = delta.normalized;
                var dir = math.normalize(delta);
                var forcePush = dir * (error * pushMultiplier * deltaTime);
                //var vOnDirA = Vector3.Dot(a.Velocity, dir);
                var vOnDirA = math.dot(a.Velocity, dir);
                //var vOnDirB = Vector3.Dot(b.Velocity, dir);
                var vOnDirB = math.dot(b.Velocity, dir);
                var vDeltaOnDir = vOnDirA - vOnDirB;
                var forceViscosity = -dir * vDeltaOnDir * viscosity;

                var force = forcePush + forceViscosity;
                //a.Velocity += force;
                //b.Velocity -= force;

                Debug.DrawLine(a.Position, b.Position, Color.green);
                return force;
            }
            else
            {
                Debug.DrawLine(a.Position, b.Position, Color.magenta);
                return float3.zero;
            }
        }
    }
}