using SpacialIndexing;
using SwarmRunner.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using WaterGame.Components;

namespace WaterGame.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct SpacialPartitioningSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var e = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(e, new SpacialPartitioningSingleton()
            {
                
                Partitioning = new SpacialPartitioning<Entity>(0.36f, Allocator.Persistent)
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
                //spacialPartitioningRw.ValueRW.Partitioning.AddPoint(entity, localToWorld.Position);
                spacialPartitioningRw.ValueRW.Partitioning.AddCircle(entity, localToWorld.Position, 0.65f);
            }
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            var spacialPartitioningRw = SystemAPI.GetSingletonRW<SpacialPartitioningSingleton>();
            spacialPartitioningRw.ValueRW.Partitioning.Dispose();
            
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var e = SystemAPI.GetSingletonEntity<SpacialPartitioningSingleton>();
            commandBuffer.DestroyEntity(e);
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
    
}