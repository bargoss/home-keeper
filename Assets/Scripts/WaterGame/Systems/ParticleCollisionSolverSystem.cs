using RunnerGame.Scripts.ECS.Components;
using SpacialIndexing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using WaterGame.Authoring;
using WaterGame.Components;

namespace WaterGame.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SpacialPartitioningSystem))]
    public partial struct ParticleCollisionSolverSystem : ISystem
    {
        NativeHashMap<Entity, float3> m_VelocityCache;
        NativeHashMap<Entity, float3> m_PositionsCache;
        
        public void OnCreate(ref SystemState state)
        {
            m_VelocityCache = new NativeHashMap<Entity, float3>(100000, Allocator.Persistent);
            m_PositionsCache = new NativeHashMap<Entity, float3>(100000, Allocator.Persistent);
        }
        public void OnDestroy(ref SystemState state)
        {
            m_VelocityCache.Dispose();
            m_PositionsCache.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spacialPartitioning = SystemAPI.GetSingletonRW<SpacialPartitioningSingleton>().ValueRO.Partitioning;
            if (!SystemAPI.TryGetSingleton<WaterGameConfig>(out var config))
            {
                config = new WaterGameConfig()
                {
                    Viscosity = 0.2f,
                    PushForce = 73.2f,
                    InnerRadius = 0.56f,
                    OuterRadius = 0.64f,
                    MaxForcePerFrame = 100f,
                };
            }
            
            m_VelocityCache.Clear();
            m_PositionsCache.Clear();
            
            foreach (var (physicsVelocity, localToWorld, particle, entity) in SystemAPI.Query<PhysicsVelocity, LocalToWorld, Particle>().WithEntityAccess())
            {
                m_VelocityCache.TryAdd(entity, physicsVelocity.Linear);
                m_PositionsCache.TryAdd(entity, localToWorld.Position);
            }
            
            state.Dependency = new SolveCollisionsJob()
            {
                SpacialPartitioning = spacialPartitioning,
                Config = config,
                Positions = m_PositionsCache,
                Velocities = m_VelocityCache,
                DeltaTime = 0.02f,
            }.ScheduleParallel(state.Dependency);
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

                //Debug.DrawLine(a.Position, b.Position, Color.green);
                return force;
            }
            else
            {
                //Debug.DrawLine(a.Position, b.Position, Color.magenta);
                return float3.zero;
            }
        }
    }
    
    [BurstCompile]
    public partial struct ApplyForcesJob : IJobEntity
    {
        [ReadOnly] public NativeParallelMultiHashMap<Entity, float3> Forces;
        
        public void Execute(Entity entity, ref PhysicsVelocity physicsVelocity)
        {
            if (!Forces.ContainsKey(entity))
                return;
        
            var totalForce = float3.zero;
            foreach (var force in Forces.GetValuesForKey(entity))
            {
                totalForce += force;
            }

            if (totalForce.Equals(float3.zero)) 
                return;
        
            physicsVelocity.Linear += totalForce;
        }
    }
}
