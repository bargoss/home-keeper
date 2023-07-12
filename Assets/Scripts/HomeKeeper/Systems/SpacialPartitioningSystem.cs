using System;
using System.Numerics;
using HomeKeeper.Components;
using SpacialIndexing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct SpacialPartitioningSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var e = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(e, new SpacialPartitioningSingleton()
            {
                Partitioning = new SpacialPartitioning<Entity>(10, Allocator.Persistent)
            });
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spacialPartitioningRw = SystemAPI.GetSingletonRW<SpacialPartitioningSingleton>();
            spacialPartitioningRw.ValueRW.Partitioning.Clear();
            foreach (var (localToWorld, spacialPartitioningEntry, entity) in SystemAPI.Query<LocalToWorld, SpacialPartitioningEntry>().WithEntityAccess())
            {
                spacialPartitioningRw.ValueRW.Partitioning.AddPoint(entity, localToWorld.Position);
            }
        }
        public void OnDestroy(ref SystemState state)
        {
            var spacialPartitioningRw = SystemAPI.GetSingletonRW<SpacialPartitioningSingleton>();
            spacialPartitioningRw.ValueRW.Partitioning.Dispose();
            
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var e = SystemAPI.GetSingletonEntity<SpacialPartitioningSingleton>();
            commandBuffer.DestroyEntity(e);
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
                var forceViscosity = dir * vDeltaOnDir * viscosity;

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